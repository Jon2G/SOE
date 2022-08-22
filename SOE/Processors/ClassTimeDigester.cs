using HtmlAgilityPack;
using Kit;
using SOE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = SOE.Models.Group;

namespace SOEWeb.Shared.Processors
{
    public static class ClassTimeDigester
    {
        private static string GetGroupSuffix(string group, ref int suffixfixer)
        {
            string suffix = "10";
            Regex lastDigitsRegex = new(@"(\d+)(?!.*\d)");
            Match suffixMatch = ((IList<Match>)lastDigitsRegex.Matches(group)).GetLast();
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

            return suffix;
        }

        public static async Task<List<Subject>> Digest(string HTML)
        {
            await Task.Yield();
            try
            {
                ClassColors offlineColors = new ClassColors();
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
                    string TeacherName = row[2];
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }

                    if (TeacherName == "&nbsp;")
                    {
                        continue;
                    }

                    Teacher teacher = await new Teacher() { Name = TeacherName }.Save();
                    teachers.Add(teacher);
                }

                List<Group> groups = new();
                List<Subject> subjects = new();
                int suffixfixer = 0;
                for (int i = 0; i < table.Count; i++)
                {
                    List<string> row = table[i];
                    string TeacherName = row[2];
                    string groupName = row[0];
                    Group group = await new Group() { Name = groupName }.Save();
                    groups.Add(group);
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }
                    if (TeacherName == "&nbsp;")
                    {
                        continue;
                    }
                    Teacher teacher = teachers[i];
                    string suffix = GetGroupSuffix(group.Name, ref suffixfixer);
                    string? subjectName = row[1]?.Trim().ToUpper()??"";
                    if (string.IsNullOrEmpty(subjectName)) { continue; }
                    Regex regex = new Regex(".+ - (?<SubjectName>.+)");
                    System.Text.RegularExpressions.Group? match = regex.Match(subjectName).Groups["SubjectName"];
                    if (match.Success)
                    {
                        subjectName = match.Value;
                    }
                    if (string.IsNullOrEmpty(subjectName)) continue;
                    Subject subject = await new Subject()
                    {
                        TeacherId = teacher.GetDocumentId(),
                        Name = subjectName,
                        GroupId = group.GetDocumentId(),
                        ThemeColor = offlineColors.Get(suffixfixer)
                    }.Save();

                    subjects.Add(subject);
                }

                Regex time =
                    new Regex(
                        @"(?<begin_hour>\d\d):(?<begin_minutes>\d\d)\s*-\s*(?<end_hour>\d\d):(?<end_minutes>\d\d)");


                for (int index = 0; index < table.Count; index++)
                {
                    List<string> rows = table[index];
                    if (subjects.Count <= index)
                    {
                        Console.WriteLine("SOE.iOS - Inconsistencia en el horario");
                        break;
                    }
                    Subject subject = subjects[index];
                    Group group = groups[index];
                    rows = rows.Skip(3).Take(5).ToList();
                    DayOfWeek dayOfWeek = DayOfWeek.Monday;
                    foreach (string row in rows)
                    {
                        Match? match = time.Match(row);
                        if (match.Success)
                        {
                            int begin_hour = Convert.ToInt32(match.Groups["begin_hour"].Value);
                            int begin_minutes = Convert.ToInt32(match.Groups["begin_minutes"].Value);
                            int end_hour = Convert.ToInt32(match.Groups["end_hour"].Value);
                            int end_minutes = Convert.ToInt32(match.Groups["end_minutes"].Value);
                            TimeSpan begin = TimeSpan.FromHours(begin_hour).Add(TimeSpan.FromMinutes(begin_minutes));
                            TimeSpan end = TimeSpan.FromHours(end_hour).Add(TimeSpan.FromMinutes(end_minutes));
                            await new ClassTime(group.GetDocumentId(), subject.GetDocumentId(), dayOfWeek, begin, end).Save();
                        }
                        dayOfWeek++;
                    }
                }

                return subjects;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "At classtimedigester");
                Console.WriteLine("SOE.iOS" + ex.ToString());
                Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString());
            }
            return new List<Subject>();
        }
    }
}
