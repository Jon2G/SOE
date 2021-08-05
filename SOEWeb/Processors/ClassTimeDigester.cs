using HtmlAgilityPack;
using Kit.Sql.Readers;
using Kit.Sql.SqlServer;
using Microsoft.Extensions.Logging;
using SOEWeb.Models;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace SOEWeb.Processors
{
    internal static class ClassTimeDigester
    {
        private static Teacher TeacherFrom(IReader reader)
        {
            if (reader.FieldCount < 2)
            {
                return null;
            }

            reader.Read();
            Teacher teacher = new Teacher()
            {
                Id = Convert.ToInt32(reader[0]),
                Name = Convert.ToString(reader[1])
            };
            reader.Dispose();
            return teacher;
        }
        private static Subject SubjectFrom(IReader reader)
        {
            if (reader.FieldCount < 7)
            {
                throw new Exception("Subject not read");
            }

            reader.Read();
            Subject subject = new Subject()
            {
                Id = Convert.ToInt32(reader[0]),
                Guid = Guid.Parse(reader[1].ToString()),
                IdTeacher = Convert.ToInt32(reader[2]),
                Name = Convert.ToString(reader[3]),
                Group = Convert.ToString(reader[4]),
                Color = Convert.ToString(reader[5]),
                ColorDark = Convert.ToString(reader[6])
            };
            reader.Dispose();
            return subject;
        }
        private static ClassTime ClassTimeFrom(IReader reader)
        {
            if (reader.FieldCount < 6)
            {
                return null;
            }

            if (reader.Read())
            {
                ClassTime classTime = new ClassTime()
                {
                    Id = Convert.ToInt32(reader[0]),
                    IdSubject = Convert.ToInt32(reader[1]),
                    Day = (DayOfWeek)Convert.ToInt32(reader[2]),
                    Begin = (TimeSpan)reader[3],
                    End = (TimeSpan)reader[4],
                    Group = Convert.ToString(reader[5])
                };
                reader.Dispose();
                return classTime;
            }
            else
            {
                throw new Exception("ClassTime not read");
            }


        }

        public static string Digest(string HTML, string user, SQLServerConnection connection, ILogger Log)
        {
            string digested_xml = string.Empty;
            try
            {
                int UserId = User.GetId(connection, user);
                HtmlAgilityPack.HtmlDocument doc
                    = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(HTML);

                HtmlNode htable =
                    doc.DocumentNode.SelectSingleNode("//table");

                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td")
                        .Select(td => td.InnerText.Trim()).ToList())
                    .ToList();


                Regex time =
                    new Regex(
                        @"(?<begin_hour>\d\d):(?<begin_minutes>\d\d)\s*-\s*(?<end_hour>\d\d):(?<end_minutes>\d\d)");
                Regex teacher_name = new Regex(@"//");

                List<Teacher> teachers = new List<Teacher>();
                List<Subject> subjects = new List<Subject>();
                List<ClassTime> classTimes = new List<ClassTime>();

                foreach (var row in table)
                {
                    string TeacherName = row[3];
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }

                    if (TeacherName == "&nbsp;")
                    {
                        continue;
                    }

                    Teacher teacher = TeacherFrom(connection.Read("SP_GET_ADD_TEACHER"
                            , CommandType.StoredProcedure
                            , new SqlParameter("NAME", TeacherName)));
                    if (teacher is null)
                    {
                        continue;
                    }
                    teachers.Add(teacher);
                    for (int i = 6; i < 12; i++)
                    {
                        var match = time.Match(row[i]);
                        if (match.Success)
                        {
                            string group = row[0];
                            string name = row[2];
                            Subject subject = SubjectFrom(connection.Read("SP_GET_ADD_SUBJECT"
                                , CommandType.StoredProcedure
                                , new SqlParameter("USER_ID", UserId)
                                , new SqlParameter("GROUP_NAME", group)
                                , new SqlParameter("TEACHER_ID", teacher.Id)
                                , new SqlParameter("SUBJECT_NAME", name)
                                ));
                            subjects.Add(subject);

                            int begin_hour = Convert.ToInt32(match.Groups["begin_hour"].Value);
                            int begin_minutes = Convert.ToInt32(match.Groups["begin_minutes"].Value);
                            int end_hour = Convert.ToInt32(match.Groups["end_hour"].Value);
                            int end_minutes = Convert.ToInt32(match.Groups["end_minutes"].Value);
                            TimeSpan begin = TimeSpan.FromHours(begin_hour).Add(TimeSpan.FromMinutes(begin_minutes));
                            TimeSpan end = TimeSpan.FromHours(end_hour).Add(TimeSpan.FromMinutes(end_minutes));
                            DayOfWeek Day = (DayOfWeek)i - 5;

                            ClassTime classTime = ClassTimeFrom(connection.Read("SP_GET_ADD_CLASSTIME"
                                    , CommandType.StoredProcedure
                                    , new SqlParameter("TEACHER_ID", teacher.Id)
                                    , new SqlParameter("SUBJECT_ID", subject.Id)
                                    , new SqlParameter("DAY_ID", (int)Day)
                                    , new SqlParameter("BEGIN_TIME", begin)
                                    , new SqlParameter("END_TIME", end)));
                            classTimes.Add(classTime);
                        }
                    }
                }
                XmlDocument db_doc = new XmlDocument();
                XmlNode root = db_doc.AppendChild(db_doc.CreateElement("root"));
                XmlSerializer xmlserializer = null;
                XmlNode root_node = null;

                root_node = root.AppendChild(db_doc.CreateElement("Teachers"));
                xmlserializer = new XmlSerializer(typeof(Teacher));
                var nav = root_node.CreateNavigator();
                foreach (Teacher teacher in teachers)
                {
                    using (var writer = nav.AppendChild())
                    {
                        writer.WriteWhitespace("");
                        xmlserializer.Serialize(writer, teacher);
                        writer.Close();
                    }
                }

                root_node = root.AppendChild(db_doc.CreateElement("Subjects"));
                xmlserializer = new XmlSerializer(typeof(Subject));
                nav = root_node.CreateNavigator();
                foreach (Subject subject in subjects)
                {
                    using (var writer = nav.AppendChild())
                    {
                        writer.WriteWhitespace("");
                        xmlserializer.Serialize(writer, subject);
                        writer.Close();
                    }
                }

                root_node = root.AppendChild(db_doc.CreateElement("ClassTimes"));
                xmlserializer = new XmlSerializer(typeof(ClassTime));
                nav = root_node.CreateNavigator();
                foreach (ClassTime classTime in classTimes)
                {
                    using (var writer = nav.AppendChild())
                    {
                        writer.WriteWhitespace("");
                        xmlserializer.Serialize(writer, classTime);
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
                Log.Log(LogLevel.Error, ex, "At classtimedigester");
            }
            connection.Close();
            return digested_xml;



        }
    }
}
