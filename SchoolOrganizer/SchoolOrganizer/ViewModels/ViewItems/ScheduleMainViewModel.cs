using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.ViewItems.ScheduleViewModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Threading;
using Kit;
using Kit.Forms.Extensions;
using Kit.Sql.Reflection;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.ViewItems.ScheduleView;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using Task = System.Threading.Tasks.Task;
namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class ScheduleMainViewModel : ModelBase
    {
        public ObservableCollection<SheduleDay> WeekDays { get; set; }
        public ObservableCollection<Hour> WeekHours { get; set; }
        public double TimeLineOffset { get; set; }
        private TimeSpan StartTime { get; set; }
        private TimeSpan EndTime { get; set; }
        private readonly Timer UpateOffsetTimer;
        public ICommand ExportToPdfCommand { get; set; }
        public ScheduleMainViewModel()
        {
            this.ExportToPdfCommand = new Command(ExportToPdf);
            this.UpateOffsetTimer = new Timer(CalculateTimeLineOffset);
            WeekDays = new ObservableCollection<SheduleDay>();
            WeekHours = new ObservableCollection<Hour>();
            GetWeek();
        }

        private async void ExportToPdf()
        {
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Generando horrario..."))
            {
                await _ExportToPdf();
            }
        }
        private async Task _ExportToPdf()
        {
            await Shell.Current.Navigation.PushAsync(new WebViewPage(ToHTML()));
        }

        private string ToHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html>")
                .Append("<html>")
                .Append("<head>")
                .Append("<style>");
            using (ReflectionCaller caller = ReflectionCaller.FromAssembly<App>())
            {
                string css = ReflectionCaller.ToText(caller.GetResource("PdfTimetableStyle.css"));
                sb.Append(css);
            }
            sb.Append("</style>")
                .Append("</head>")
                .Append(GetHTMLTable())
                .Append("</html>");
            return sb.ToString();
        }

        private class Td
        {
            public readonly ClassSquare Subject;
            public readonly int Rowspan;

            public Td(ClassSquare Subject, int Rowspan = 1)
            {
                this.Subject = Subject;
                this.Rowspan = Rowspan;
            }

            public string ToHTML()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<td ");
                sb.Append("colspan=\"1\" rowspan=\"").Append(Rowspan).Append("\" ")
                    .Append("style=\"")
                    .Append("background-color:")
                    .Append(Subject.Subject.Color)
                    .Append(";")
                    .Append("\">")
                    .Append(Subject.Subject.Name)
                    .AppendLine("</td>");
                /*
<td colspan="1" rowspan="1" class="stage-saturn">Welcome</td>
<td rowspan="2" colspan="1" class="stage-mercury">Speaker Five<span>Mercury Stage</span></td>
 */
                return sb.ToString();
            }
        }
        private class Tr : Collection<Td>
        {
            public TimeSpan Begin => Subject.Begin;
            public TimeSpan End => Subject.End;
            public readonly ClassSquare Subject;

            public Tr(ClassSquare subject) : base(new List<Td>())
            {
                this.Subject = subject;
                this.Add(new Td(subject));
            }

            public string ToHTML()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<tr>")
                    .Append("<th>")
                    .Append(Subject.FormattedTime)
                    .Append("</th>");
                this.ForEach(td => sb.Append(td.ToHTML()));
                sb.Append("</tr>");
                return sb.ToString();
            }
        }
        private string GetHTMLTable()
        {
            List<Tr> rows = new List<Tr>();
            foreach (var day in WeekDays)
            {
                foreach (var subject in day.Class)
                {
                    if (rows.FirstOrDefault(x => x.Begin == subject.Begin) is Tr actualRow)
                    {
                        if (actualRow.End > subject.End) //procuramos tener los intervalos de horario mas pequeñas
                        {
                            rows.Remove(actualRow);
                            rows.Add(new Tr(subject));
                        }
                    }
                    else
                    {
                        rows.Add(new Tr(subject));
                    }
                }
            }
            foreach (var day in WeekDays)
            {
                foreach (var subject in day.Class)
                {
                    foreach (var row in rows)
                    {
                        if (row.Subject == subject)
                        {
                            continue;
                        }
                        if (row.Begin == subject.Begin && row.End == subject.End)
                        {
                            row.Add(new Td(subject));
                        }
                        else if (row.Begin == subject.Begin)
                        {

                            double CurrentHours = (subject.End - subject.Begin).TotalHours;
                            double Hours = (row.End - row.Begin).TotalHours;
                            int rowspan = (int)Math.Round(CurrentHours / Hours);

                            row.Add(new Td(subject, rowspan));
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<table>")
                .Append("<body>")
                .Append(GetHTMLHeader());
            rows.ForEach(r => sb.Append(r.ToHTML()));
            sb.Append("</body>")
            .Append("</table>");
            return sb.ToString();
        }

        private string GetHTMLHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<tr>")
                .Append("<td>Horario</td>");
            foreach (var day in WeekDays)
            {
                sb.Append("<td>")
                    .Append(day.Day.Name)
                    .Append("</td>");
            }
            sb.Append("</tr>");
            return sb.ToString();
        }

        private bool GetRow(SheduleDay day)
        {
            throw new NotImplementedException();
        }

        private void GetWeek()
        {
            TimeSpan min_hourtime = TimeSpan.Zero;
            TimeSpan max_hourtime = TimeSpan.Zero;
            int min_hour = 25;
            int max_hour = -1;
            for (DayOfWeek dayOfWeek = DayOfWeek.Monday; dayOfWeek <= DayOfWeek.Friday; dayOfWeek++)
            {
                var schedule = new SheduleDay(Day.GetNearest(dayOfWeek));
                if (!schedule.Class.Any()) { continue; }
                WeekDays.Add(schedule);

                TimeSpan max_newhourtime = schedule.Class.Max(x => x.End);
                TimeSpan min_newhourtime = schedule.Class.Min(x => x.Begin);
                int max_newhour = max_newhourtime.Hours;
                int min_newhour = min_newhourtime.Hours;
                if (max_newhour > max_hour)
                {
                    max_hour = max_newhour;
                    max_hourtime = max_newhourtime;
                }
                if (min_newhour < min_hour)
                {
                    min_hour = min_newhour;
                    min_hourtime = min_newhourtime;
                }
            }

            int j = 0;
            for (int i = min_hour; i <= max_hour; i++, j++)
            {
                this.WeekHours.Add(new Hour(j, i));
            }

            this.StartTime = min_hourtime;
            this.EndTime = max_hourtime;
            CalculateTimeLineOffset(null);
            this.UpateOffsetTimer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private void CalculateTimeLineOffset(object obj)
        {
            if (DateTime.Now.TimeOfDay > this.EndTime)
            {
                this.TimeLineOffset = Math.Abs((this.StartTime - this.EndTime).TotalHours) * (Hour.HourHeigth + 4.5);
                this.TimeLineOffset -= 3; //Al final no debe considerar el ultimo margén
            }
            else
            {
                TimeSpan total = DateTime.Now.TimeOfDay - this.StartTime;
                double TotalHours = total.TotalHours;
                this.TimeLineOffset = (TotalHours) * (Hour.HourHeigth + 4.5); //3*1.5 compensa los margenes de 3px entre cuadros de elementos
            }

            this.TimeLineOffset--; //descontar el alto de la boxview misma
            Raise(() => TimeLineOffset);
        }
    }
}
