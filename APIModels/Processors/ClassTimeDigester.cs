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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Kit;
using SOEWeb.Shared.Interfaces;

namespace SOEWeb.Shared.Processors
{
    public static class ClassTimeDigester
    {
        private static Subject PostSubject(int UserId, string Group, string Suffix, int TeacherId, string SubjectName)
        {
            using (SqlConnection con = WebData.Connection)
            {
                con.Open();
                using (SqlCommand cmd = new("SP_GET_ADD_SUBJECT", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.Parameters.AddRange(new SqlParameter[]
                    {
                        new ("USER_ID", UserId),
                        new ("GROUP_NAME", Group),
                        new ("SUFFIX ", Suffix),
                        new ("TEACHER_ID", TeacherId),
                        new ("SUBJECT_NAME", SubjectName)
                    });
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Subject()
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Guid = Guid.Parse(reader[1].ToString()),
                                IdTeacher = Convert.ToInt32(reader[2]),
                                Name = Convert.ToString(reader[3]),
                                Group = Convert.ToString(reader[4]),
                                Color = Convert.ToString(reader[5]),
                                ColorDark = Convert.ToString(reader[6]),
                                GroupId = Convert.ToInt32(reader[7])
                            };
                        }
                    }
                }
            }
            return null;
        }
        private static Teacher PostTeacher(string TeacherName)
        {
            using (SqlConnection con = WebData.Connection)
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SP_GET_ADD_TEACHER", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.Parameters.Add(new SqlParameter("NAME", TeacherName));
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Teacher() { Id = Convert.ToInt32(reader[0]), Name = Convert.ToString(reader[1]) };
                        }
                    }
                }
            }
            return null;
        }
        private static ClassTime PostClassTimeFrom(int TeacherId, int SubjectId, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            using (SqlConnection con = WebData.Connection)
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SP_GET_ADD_CLASSTIME", con)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    cmd.Parameters.AddRange(new SqlParameter[]
                    {
                        new ("TEACHER_ID", TeacherId),
                        new ("SUBJECT_ID", SubjectId),
                        new ("DAY_ID", (int)Day),
                        new ("BEGIN_TIME", Begin),
                        new ("END_TIME", End)
                    });
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ClassTime()
                            {
                                Id = Convert.ToInt32(reader[0]),
                                IdSubject = Convert.ToInt32(reader[1]),
                                Day = (DayOfWeek)Convert.ToInt32(reader[2]),
                                Begin = (TimeSpan)reader[3],
                                End = (TimeSpan)reader[4],
                                Group = Convert.ToString(reader[5])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static Response Digest(byte[] HTML, string user, ILogger Log)
        {
            DigesterResult<string> result = ClassTimeDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), user, Log);
            return result.ToResponse();
        }

        public static DigesterResult<string> Digest(string HTML, string user, ILogger Log)
        {
            int UserId = -1;
            using (var con = WebData.Connection)
            {
                UserId = UserBase.GetId(user, con);
            }
            if (UserId <= 0)
            {
                return new DigesterResult<string>($"User : [{user}] not found", APIResponseResult.NOT_EXECUTED);
            }

            return Digest(HTML, UserId, Log, true);
        }
        public static DigesterResult<string> Digest(string HTML, int userId, ILogger Log, bool Online)
        {
            string digested_xml = string.Empty;
            try
            {
                OfflineColors offlineColors = Online ? null : new OfflineColors();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(HTML);

                HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");

                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td")
                        .Select(td => td.InnerText.Trim()).ToList())
                    .ToList();
                Regex teacher_name = new Regex(@"//");

                List<Teacher> teachers = new();
                foreach (List<string> row in table)
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

                    Teacher teacher =
                        Online ? PostTeacher(TeacherName) :
                            new Teacher() { Id = OfflineConstants.IdBase + teachers.Count, Name = TeacherName, IsOffline = true };

                    if (teacher is null)
                    {
                        return new DigesterResult<string>(
                                ResponseResult: APIResponseResult.INTERNAL_ERROR,
                                Value: null,
                                Extra: "Teacher not read")
                            .Log(Log);
                    }
                    teachers.Add(teacher);
                }

                List<Subject> subjects = new();
                int suffixfixer = 0;
                for (var i = 0; i < table.Count; i++)
                {
                    List<string> row = table[i];
                    string TeacherName = row[3];
                    string group = row[0];
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }
                    if (TeacherName == "&nbsp;")
                    {
                        continue;
                    }

                    Teacher teacher = teachers[i];
                    string suffix = "10";
                    Regex lastDigitsRegex = new(@"(\d+)(?!.*\d)");
                    Match suffixMatch = Linq.Last((IList<Match>)lastDigitsRegex.Matches(@group));// lastDigitsRegex.Matches(@group) .Last();
                    if (suffixMatch.Success)
                    {
                        suffix = suffixMatch.Value;
                        if (suffix.Length > 2)
                        {
                            suffix = suffix.Substring(suffix.Length - 2);
                        }
                        else if (suffix.Length < 2)
                        {
                            suffix += suffixfixer.ToString();
                            suffixfixer++;
                        }
                    }

                    string SubjectName = row[2];
                    Subject subject = Online
                        ? PostSubject(userId, group, suffix, teacher.Id, SubjectName)
                        : new Subject(OfflineConstants.IdBase + subjects.Count, teacher.Id, SubjectName, offlineColors.Get(subjects.Count), group)
                        { IsOffline = true, GroupId = OfflineConstants.IdBase + subjects.Count, Guid = Guid.NewGuid() };
                    if (subject is null)
                    {
                        return new DigesterResult<string>(
                                ResponseResult: APIResponseResult.INTERNAL_ERROR,
                                Value: null,
                                Extra: "Subject not read")
                            .Log(Log);
                    }

                    subjects.Add(subject);
                }

                Regex time =
                    new Regex(
                        @"(?<begin_hour>\d\d):(?<begin_minutes>\d\d)\s*-\s*(?<end_hour>\d\d):(?<end_minutes>\d\d)");
                List<ClassTime> classTimes = new List<ClassTime>();

                for (var index = 0; index < table.Count; index++)
                {
                    List<string> row = table[index];
                    string TeacherName = row[3];
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }

                    if (TeacherName == "&nbsp;")
                    {
                        continue;
                    }

                    Teacher teacher = teachers[index];
                    Subject subject = subjects[index];
                    for (int i = 6; i < 12; i++)
                    {
                        var match = time.Match(row[i]);
                        if (match.Success)
                        {
                            int begin_hour = Convert.ToInt32(match.Groups["begin_hour"].Value);
                            int begin_minutes = Convert.ToInt32(match.Groups["begin_minutes"].Value);
                            int end_hour = Convert.ToInt32(match.Groups["end_hour"].Value);
                            int end_minutes = Convert.ToInt32(match.Groups["end_minutes"].Value);
                            TimeSpan begin = TimeSpan.FromHours(begin_hour).Add(TimeSpan.FromMinutes(begin_minutes));
                            TimeSpan end = TimeSpan.FromHours(end_hour).Add(TimeSpan.FromMinutes(end_minutes));
                            DayOfWeek Day = (DayOfWeek)i - 5;

                            ClassTime classTime = Online
                                ? PostClassTimeFrom(teacher.Id, subject.Id, Day, begin, end)
                                : new ClassTime()
                                {
                                    Id = OfflineConstants.IdBase + classTimes.Count,
                                    IdSubject = subject.Id,
                                    Day = Day,
                                    Begin = begin,
                                    End = end,
                                    Group = subject.Group,
                                    IsOffline = true
                                };

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
                        return new DigesterResult<string>(
                            Value: stringWriter.GetStringBuilder().ToString(),
                            ResponseResult: APIResponseResult.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Log(LogLevel.Error, ex, "At classtimedigester");
                return new DigesterResult<string>(ResponseResult: APIResponseResult.INTERNAL_ERROR, Value: null,
                    Extra: ex.ToString()).Log(Log);
            }
        }
    }
}
