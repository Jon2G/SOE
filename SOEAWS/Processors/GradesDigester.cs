using HtmlAgilityPack;
using Kit.Sql.Readers;
using Kit.Sql.SqlServer;
using Microsoft.Extensions.Logging;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SOEAWS.Processors
{
    public static class GradesDigester
    {
        private static Grade GradeFrom(IReader reader)
        {
            if (reader.FieldCount < 5)
            {
                return null;
            }

            reader.Read();
            Grade grade= new Grade()
            {
                Id = Convert.ToInt32(reader[0]),
                SubjectId = Convert.ToInt32(reader[1]),
                NumericScore =Convert.ToInt32(reader[2]),
                TextScore = Convert.ToString(reader[3]),
                Partial = (GradePartial)Convert.ToInt32(reader[4])
            };
            reader.Dispose();
            return grade;
        }
        public static string Digest(string HTML,string User, SQLServerConnection connection,ILogger Log)
        {
            string digested_xml = string.Empty;
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(HTML);
                HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                    .ToList();
                List<Grade> grades = new List<Grade>();
                foreach (var row in table)
                {
                    string group = row[0];
                    Grade[] row_grades = new Grade[5];
                    for (int j = 2; j <= 6; j++)
                    {
                        int index = j - 2;
                        string score = row[j];
                        if (score == "&nbsp;")
                        {
                            score = "-";
                        }

                        GradePartial partial = (GradePartial)j-1;
                        string text_score = score;
                        int numeric_score = -1;
                        int.TryParse(text_score, out numeric_score);


                        row_grades[index] = GradeFrom(
                            connection.Read("SP_UPDATE_GRADES"
                                ,CommandType.StoredProcedure
                                ,new SqlParameter("PARTIAL",(int)partial)
                                ,new SqlParameter("TEXT_SCORE",text_score)
                                ,new SqlParameter("NUMERIC_SCORE",numeric_score)
                                ,new SqlParameter("GROUP",group)
                                ,new SqlParameter("USER", User)
                                ));
                    }
                    grades.AddRange(row_grades);
                }

                XmlDocument db_doc = new XmlDocument();
                XmlNode root = db_doc.AppendChild(db_doc.CreateElement("root"));
                XmlSerializer xmlserializer = new XmlSerializer(typeof(Grade));
                XmlNode root_node = root.AppendChild(db_doc.CreateElement("Grades"));
                var nav = root_node.CreateNavigator();
                foreach (Grade grade in grades)
                {
                    using (var writer = nav.AppendChild())
                    {
                        writer.WriteWhitespace("");
                        xmlserializer.Serialize(writer, grade);
                        writer.Close();
                    }
                }
                using (var stringWriter = new StringWriter())
                {
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        db_doc.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        return stringWriter.GetStringBuilder().ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Log(LogLevel.Error, ex, "At gradedigester");
            }
            return digested_xml;
        }
    }
}
