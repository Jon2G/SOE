using HtmlAgilityPack;
using Kit;
using Kit.Services.Web;
using Kit.Sql.Sqlite;
using Microsoft.Extensions.Logging;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace SOEWeb.Shared.Processors
{
    public static class GradesDigester
    {
        public static Grade PostGrade(GradePartial partial, string text_score, int numeric_score, string group, string User, string subject_name = null)
        {
            Grade grade = null;
            WebData.Connection.Read("SP_UPDATE_GRADES",
                (reader) =>
                {
                    if (reader.FieldCount < 5)
                    {
                        return;
                    }

                    reader.Read();

                    grade = new Grade()
                    {
                        Id = Convert.ToInt32(reader[0]),
                        NumericScore = Convert.ToInt32(reader[2]),
                        TextScore = Convert.ToString(reader[3]),
                        Partial = (GradePartial)Convert.ToInt32(reader[4]),
                        SubjectId = Convert.ToInt32(reader[5])
                    };
                }
                , new CommandConfig() { CommandType = CommandType.StoredProcedure, ManualRead = true }
                , new SqlParameter("PARTIAL", (int)partial)
                , new SqlParameter("TEXT_SCORE", text_score)
                , new SqlParameter("NUMERIC_SCORE", numeric_score)
                , new SqlParameter("GROUP", group)
                , new SqlParameter("USER", User)
                , new SqlParameter("SUBJECT_NAME", Kit.Sql.Helpers.Sqlh.IsNull(subject_name) ? DBNull.Value : subject_name)
            );
            return grade;
        }
        public static Response Digest(byte[] HTML, string user, ILogger Log)
        {
            return GradesDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), user, Log, true);
        }
        public static Response<string> Digest(string HTML, string User, ILogger Log, bool Online)
        {
            Regex justNumbersRegex = new Regex(@"(?<number>\d+)");
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

                        GradePartial partial = (GradePartial)j - 1;
                        string text_score = score;
                        int numeric_score = -1;

                        var justNumbersMatch = justNumbersRegex.Match(text_score);
                        if (justNumbersMatch.Success)
                        {
                            text_score = justNumbersMatch.Groups["number"]?.Value ?? "-";
                        }

                        if (string.IsNullOrEmpty(text_score))
                        {
                            text_score = "-";
                        }
                        int.TryParse(text_score, out numeric_score);

                        string subjectName = null;

                        subjectName = row[1].ToUpper()?.Trim();
                        Regex subjectNameRegex = new Regex(@"(?<Prefix>.+-+)(?<SubjectName>(\w| )+)");
                        var match = subjectNameRegex.Match(subjectName);
                        if (match.Success)
                        {
                            subjectName = match.Groups["SubjectName"].Value.ToUpper().Trim();
                        }


                        if (Online)
                        {
                            row_grades[index] = PostGrade(partial, text_score, numeric_score, group, User, subjectName);
                        }
                        else
                        {
                            Subject local_subject = null;
                            using (var lite = new SQLiteConnection(WebData.LiteDbPath, 0))
                            {

                                local_subject = lite.Table<Subject>().FirstOrDefault(x => x.Name.ToUpper() == subjectName);
                                //local_subject = lite.Table<Subject>().FirstOrDefault(x => x.Group == group);

                            }
                            if (local_subject is not null)
                                row_grades[index] = new Grade()
                                {
                                    //Id = OfflineConstants.IdBase + index,
                                    SubjectId = local_subject.Id,
                                    NumericScore = numeric_score,
                                    TextScore = text_score,
                                    Partial = partial,
                                    IsOffline = true
                                };
                        }
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
                        return new Response<string>(APIResponseResult.OK, "OK", stringWriter.GetStringBuilder().ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Log?.Log(LogLevel.Error, ex, "At gradedigester");
                return new Response<string>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
