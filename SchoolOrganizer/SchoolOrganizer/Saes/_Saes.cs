using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using HtmlAgilityPack;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using SchoolOrganizer.Views.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Log = Kit.Log;

namespace SchoolOrganizer.Saes
{
    public class _Saes
    {
        //public WebView Browser { get; private set; }
        //private async void GetCitasReinscripcionInfo()
        //{
        //    double creditos_totales = 0;
        //    double creditos_alumno = 0;

        //    string creditos_carrera_html = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_CREDITOSCARRERA\").outerHTML");
        //    string alumno_html = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_alumno\").outerHTML");

        //    alumno_html = System.Text.RegularExpressions.Regex.Unescape(alumno_html);
        //    if (!string.IsNullOrEmpty(alumno_html))
        //    {
        //        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //        doc.LoadHtml(alumno_html);
        //        HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
        //        List<List<string>> table = htable
        //            .Descendants("tr")
        //            .Skip(1)
        //            .Where(tr => tr.Elements("td").Count() > 1)
        //            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        //            .ToList();
        //        creditos_alumno = Convert.ToDouble(table[0][1]);
        //    }

        //    creditos_carrera_html = System.Text.RegularExpressions.Regex.Unescape(creditos_carrera_html);
        //    if (!string.IsNullOrEmpty(creditos_carrera_html))
        //    {
        //        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //        doc.LoadHtml(creditos_carrera_html);
        //        HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
        //        List<List<string>> table = htable
        //            .Descendants("tr")
        //            .Skip(1)
        //            .Where(tr => tr.Elements("td").Count() > 1)
        //            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        //            .ToList();
        //        creditos_totales = Convert.ToDouble(table[0][1]);
        //    }

        //    double result = (creditos_alumno / creditos_totales) * 100;
        //    result = Math.Round(result, 2);
        //    GoTo(CalificacionesPage);
        //}
        //private async void GetSchoolGrades()
        //{
        //    string grades_html = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_GV_Calif\").outerHTML");
        //    grades_html = System.Text.RegularExpressions.Regex.Unescape(grades_html);
        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(grades_html);
        //    HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
        //    List<List<string>> table = htable
        //        .Descendants("tr")
        //        .Skip(1)
        //        .Where(tr => tr.Elements("td").Count() > 1)
        //        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        //        .ToList();
        //    List<Grade> grades = new List<Grade>();
        //    foreach (var row in table)
        //    {
        //        string grupo = row[0];
        //        Grade[] row_grades = new Grade[5];
        //        for (int j = 2; j <= 6; j++)
        //        {
        //            int index = j - 2;
        //            row_grades[index] = new Grade((Partial)index, row[j], Subject.GetId(grupo));
        //        }
        //        grades.AddRange(row_grades);
        //    }

        //    foreach (var grade in grades)
        //    {
        //        AppData.Instance.LiteConnection.Table<Grade>()
        //            .Delete(x => x.Partial == grade.Partial && x.SubjectId == grade.SubjectId);
        //        AppData.Instance.LiteConnection.Insert(grade);
        //    }

        //    if (AppShell.Current is AppShell shell)
        //    {
        //        if (shell.MasterPage.TabView.TabItems[0].Content is SchoolGrades view)
        //        {
        //            view.Model.HasBeenRefreshedCommand?.Execute(this);
        //        }
        //    }
        //    //if (OnFinished is null)
        //    //{
        //    //    //Reboot app
        //    //    App.Current.MainPage = new SplashScreen();
        //    //}
        //    //else
        //    //{

        //    //}
        //}
        //private async void GetKardexInfo()
        //{
        //    string carrera = await this.Browser.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_Lbl_Carrera').innerHTML");
        //    GoTo(CitaReinscripcionPage);
        //}
        //private async void GetHorario()
        //{
        //    if (DataInfo.HasTimeTable())
        //    {
        //        return;
        //    }
        //    string horario_html = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_GV_Horario\").outerHTML");
        //    horario_html = System.Text.RegularExpressions.Regex.Unescape(horario_html);

        //    if (!string.IsNullOrEmpty(horario_html))
        //    {

        //        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //        doc.LoadHtml(horario_html);

        //        var htable = doc.DocumentNode.SelectSingleNode("//table");

        //        List<List<string>> table = htable
        //            .Descendants("tr")
        //            .Skip(1)
        //            .Where(tr => tr.Elements("td").Count() > 1)
        //            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        //            .ToList();


        //        Regex time = new Regex(@"(?<begin_hour>\d\d):(?<begin_minutes>\d\d)\s*-\s*(?<end_hour>\d\d):(?<end_minutes>\d\d)");
        //        Regex teacher_name = new Regex(@"//");
        //        int aux_suffix = 1;
        //        foreach (var row in table)
        //        {
        //            string TeacherName = row[3];
        //            if (teacher_name.IsMatch(TeacherName))
        //            {
        //                TeacherName = teacher_name.Split(TeacherName).First();
        //            }
        //            Teacher teacher = new Teacher()
        //            {
        //                Name = TeacherName
        //            };
        //            AppData.Instance.LiteConnection.Insert(teacher);

        //            for (int i = 6; i < 12; i++)
        //            {
        //                var match = time.Match(row[i]);
        //                if (match.Success)
        //                {
        //                    string group = row[0];
        //                    Subject subject;

        //                    subject = (AppData.Instance.LiteConnection.Table<Subject>().FirstOrDefault(x =>
        //                        x.Group == group));
        //                    if (subject is null)
        //                    {

        //                        int suffix = Convert.ToInt32(group.Last().ToString());
        //                        subject = new Subject()
        //                        {
        //                            Name = row[2],
        //                            Color = ((Color)Application.Current.Resources[$"SubjectColor_{suffix}"]).ToHex(),
        //                            Group = group,
        //                            IdTeacher = teacher.Id
        //                        };
        //                        if (AppData.Instance.LiteConnection.Table<Subject>().Where(x =>
        //                            x.Color == subject.Color && x.Group != subject.Group).Any())
        //                        {
        //                            subject.Color =
        //                                ((Color)Application.Current.Resources[$"SubjectColor_Aux{aux_suffix}"])
        //                                .ToHex();
        //                            aux_suffix++;
        //                        }

        //                        AppData.Instance.LiteConnection.Insert(subject);
        //                    }

        //                    int begin_hour = Convert.ToInt32(match.Groups["begin_hour"].Value);
        //                    int begin_minutes = Convert.ToInt32(match.Groups["begin_minutes"].Value);
        //                    int end_hour = Convert.ToInt32(match.Groups["end_hour"].Value);
        //                    int end_minutes = Convert.ToInt32(match.Groups["end_minutes"].Value);

        //                    TimeSpan begin = TimeSpan.FromHours(begin_hour).Add(TimeSpan.FromMinutes(begin_minutes));
        //                    TimeSpan end = TimeSpan.FromHours(end_hour).Add(TimeSpan.FromMinutes(end_minutes));
        //                    ClassTime classTime = new ClassTime()
        //                    {
        //                        Begin = begin,
        //                        End = end,
        //                        Day = (DayOfWeek)i - 5,
        //                        IdSubject = subject.Id
        //                    };
        //                    AppData.Instance.LiteConnection.Insert(classTime);
        //                }
        //            }


        //        }
        //    }
        //    GoTo(KardexPage);
        //}
        //private async void GetName()
        //{
        //    AppData.Instance.User.Name = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_FormView1_nombrelabel\").innerHTML;");
        //    AppData.Instance.User.Boleta = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML;");
        //    //AppData.Instance.User.RemeberMe = false;

        //    this.LoginViewModel.OnLoginSuccess.Execute(this);
        //    GoTo(HorariosPage);
        //}
        //public void RefreshSchoolGrades()
        //{
        //    GoTo(CalificacionesPage);
        //}
        //private async void CaptchaConfirmed(object obj)
        //{
        //    await Task.Delay(1);
        //    if (string.IsNullOrEmpty(LoginViewModel.Captcha))
        //    {
        //        return;
        //    }
        //    OnFinished = null;
        //    AskForCaptcha askForCaptcha = new AskForCaptcha(this.LoginViewModel);
        //    askForCaptcha.ClosedCommad = new Command(() => { LoginRequested(this.LoginViewModel); });
        //    await askForCaptcha.Mostrar();
        //}
    }
}
