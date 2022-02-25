using HtmlAgilityPack;
using Kit;
using SOE.Enums;
using SOE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SOE.Processors
{
    public static class GradesDigester
    {
        public static async Task Digest(string HTML)
        {
            await Task.Yield();
            Regex justNumbersRegex = new Regex(@"(?<number>\d+)");
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
                foreach (List<string> row in table)
                {
                    for (int j = 2; j <= 6; j++)
                    {
                        int index = j - 2;
                        string score = row[j];
                        if (score == "&nbsp;")
                        {
                            score = "-";
                        }

                        GradePartial partial = (GradePartial)j - 1;
                        string textScore = score;
                        var justNumbersMatch = justNumbersRegex.Match(textScore);
                        if (justNumbersMatch.Success)
                        {
                            textScore = justNumbersMatch.Groups["number"]?.Value ?? "-";
                        }

                        if (string.IsNullOrEmpty(textScore))
                        {
                            textScore = "-";
                        }
                        int.TryParse(textScore, out int numericScore);
                        string? subjectName = row[1].ToUpper()?.Trim();
                        if (string.IsNullOrEmpty(subjectName)) { continue; }
                        Regex subjectNameRegex = new Regex(@"(?<Prefix>.+-+)(?<SubjectName>(\w| )+)");
                        Match match = subjectNameRegex.Match(subjectName);
                        if (match.Success)
                        {
                            subjectName = match.Groups["SubjectName"].Value.ToUpper().Trim();
                        }

                        await new Grade()
                        {
                            Partial = partial,
                            TextScore = textScore,
                            NumericScore = numericScore,
                            Subject = await Subject.FindByName(subjectName)
                        }.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "At gradedigester");
            }
        }
    }
}
