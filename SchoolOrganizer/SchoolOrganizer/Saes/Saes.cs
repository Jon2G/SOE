using Kit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.PopUps;
using Xamarin.Forms;

namespace SchoolOrganizer.Saes
{
    public class SAES : WebView
    {
        private const string SaesHomePage = "https://www.saes.ipn.mx";
        //public const string HomePage = "https://www.saes.esimecu.ipn.mx";
        private const string AlumnosPage = "/alumnos/default.aspx";
        private const string HorariosPage = "/alumnos/informacion_semestral/horario_alumno.aspx";
        private const string KardexPage = "/alumnos/boleta/kardex.aspx";
        private const string CitaReinscripcionPage = "/alumnos/reinscripciones/fichas_reinscripcion.aspx";
        private const string CalificacionesPage = "/alumnos/informacion_semestral/calificaciones_sem.aspx";
        private const string UniversitiesPage = "https://www.saes.ipn.mx/ns.html";
        private const string HighSchoolsPage = "https://www.saes.ipn.mx/nms.html";
        private readonly AutoResetEvent NavigatedCallback;
        public School School { get; set; }
        public bool IsNavigating { get; private set; }

        public SAES() : this(new School())
        {

        }
        public SAES(School School)
        {
            this.School = School;
            this.Navigated += Browser_Navigated;
            this.Navigating += Browser_Navigating;
            this.NavigatedCallback = new AutoResetEvent(false);
            if (this.School.IsSchoolSelected)
                GoTo(this.School.HomePage);
        }
        private async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.IsNavigating = false;
            await this.EvaluateJavaScriptAsync(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { alert(""Error occured: "" + errorMsg); return false;");
            //OnNavigated?.Invoke(e);
            //OnNavigated = null;
            if (e.Url is null)
            {
                await GoTo(School.HomePage);
                return;
            }

            if (e.Url == School.HomePage)
            {
                this.NavigatedCallback.Set();
                return;
            }
            switch (e.Url)
            {
                case UniversitiesPage:
                case HighSchoolsPage:
                    this.NavigatedCallback.Set();
                    return;
            }
            Uri Uri = new Uri(e.Url);
            string path = Uri.AbsolutePath.ToLower();
            switch (path)
            {
                case "/default.aspx":
                case "/": //HomePage
                case AlumnosPage:
                case HorariosPage:
                case KardexPage:
                case CitaReinscripcionPage:
                case CalificacionesPage:
                    this.NavigatedCallback.Set();
                    break;
                default:
                    //await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"I dont know what to do on =>[{e.Url}]");
                    Log.Logger.Debug($"I dont know what to do on =>[{e.Url}]");
                    await GoTo(School.HomePage);
                    break;
            }
        }
        private void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {
            this.IsNavigating = true;
        }
        public async Task GoTo(string url)
        {
            string navigateUrl = url;
            if (url != School.HomePage)
            {
                navigateUrl = School.HomePage + url;
            }
            Source = new UrlWebViewSource()
            {
                Url = navigateUrl
            };
            await Task.Run(() => this.NavigatedCallback.WaitOne(TimeSpan.FromMinutes(1)));

        }
        public async Task<ImageSource> GetCaptcha()
        {
            ImageSource ImageSource = null;
            try
            {
                await GoTo(School.HomePage);
                string base_64 = await this.EvaluateJavaScriptAsync(
                    @"var getDataUrl = function (img) {
var canvas = document.createElement('canvas')
var ctx = canvas.getContext('2d')
canvas.width = img.width
canvas.height = img.height
ctx.drawImage(img, 0, 0)
// If the image is not png, the format
 // must be specified here
return canvas.toDataURL()
}
var img=document.getElementById(""c_default_ctl00_leftcolumn_loginuser_logincaptcha_CaptchaImage"");
            getDataUrl(img);");
                if (!string.IsNullOrEmpty(base_64))
                {
                    base_64 = base_64.Replace("data:image/png;base64,", string.Empty);
                    ImageSource =
                        Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base_64)));
                }
            }
            catch (Exception ex)
            {

            }

            return ImageSource;
        }
        public async Task<bool> IsLoggedIn()
        {
            try
            {
                return
                    !string.IsNullOrEmpty(await EvaluateJavaScriptAsync(
                        "document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML"));
            }
            catch (Exception) { return false; }
        }
        public async Task<string> GetCurrentUser()
        {
            return await EvaluateJavaScriptAsync(
                "document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML");
        }
        public async Task LogOut()
        {
            await EvaluateJavaScriptAsync(
                "document.getElementById('ctl00_leftColumn_LoginStatusSession').click()");
            await GoTo(School.HomePage);
            await GoTo(School.HomePage);

        }
        public async Task<bool> LogIn(LoginViewModel login, bool ShouldGetUserData = true)
        {
            if (login.AttemptCount++ < 3)
            {
                Regex regex = new Regex("[\"\\\\]");
                string Password = regex.Replace(login.User.Password, "\\");
                //binding.captchaDisplayer.visibility = View.GONE
                await this.EvaluateJavaScriptAsync(
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_UserName\").value = \"{login.User.Boleta}\";" +
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_Password\").value = \"{Password}\";" +
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_CaptchaCodeTextBox\").value = \"{login.Captcha}\";" +
                    "document.getElementById(\"ctl00_leftColumn_LoginUser_LoginButton\").click();");
                await Task.Run(() => this.NavigatedCallback.WaitOne());
                if (await IsLoggedIn())
                {
                    if (ShouldGetUserData)
                        await GetUserData();
                    AppData.Instance.User = login.User;
                    return true;
                }
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Tienes varios intentos fallidos. Es posible que la aplicación no pueda comunicarse correctamente con el SAES o tus datos sean incorrectos." +
                                                           " Si continuas es posible que tu cuenta sea suspendida.", "Cuidado!", "Entiendo");
            }
            return false;
        }
        private async Task GetUserData()
        {
            await GetName();
            await GetHorario();
            await GetKardexInfo();
            await GetCitasReinscripcionInfo();
            await GetSchoolGrades();
        }
        private async Task GetName()
        {
            await GoTo(AlumnosPage);
            AppData.Instance.User.Name = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_FormView1_nombrelabel\").innerHTML;");
            AppData.Instance.User.Boleta = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML;");
        }
        private async Task GetHorario()
        {
            await GoTo(HorariosPage);
            if (DataInfo.HasTimeTable())
            {
                return;
            }
            string horario_html = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_GV_Horario\").outerHTML");
            horario_html = System.Text.RegularExpressions.Regex.Unescape(horario_html);

            if (!string.IsNullOrEmpty(horario_html))
            {

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(horario_html);

                var htable = doc.DocumentNode.SelectSingleNode("//table");

                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                    .ToList();


                Regex time = new Regex(@"(?<begin_hour>\d\d):(?<begin_minutes>\d\d)\s*-\s*(?<end_hour>\d\d):(?<end_minutes>\d\d)");
                Regex teacher_name = new Regex(@"//");
                int aux_suffix = 1;
                foreach (var row in table)
                {
                    string TeacherName = row[3];
                    if (teacher_name.IsMatch(TeacherName))
                    {
                        TeacherName = teacher_name.Split(TeacherName).First();
                    }
                    Teacher teacher = new Teacher()
                    {
                        Name = TeacherName
                    };
                    AppData.Instance.LiteConnection.Insert(teacher);

                    for (int i = 6; i < 12; i++)
                    {
                        var match = time.Match(row[i]);
                        if (match.Success)
                        {
                            string group = row[0];
                            Subject subject;

                            subject = (AppData.Instance.LiteConnection.Table<Subject>().FirstOrDefault(x =>
                                x.Group == group));
                            if (subject is null)
                            {

                                int suffix = Convert.ToInt32(group.Last().ToString());
                                subject = new Subject()
                                {
                                    Name = row[2],
                                    Color = ((Color)Application.Current.Resources[$"SubjectColor_{suffix}"]).ToHex(),
                                    Group = group,
                                    IdTeacher = teacher.Id
                                };
                                if (AppData.Instance.LiteConnection.Table<Subject>().Where(x =>
                                    x.Color == subject.Color && x.Group != subject.Group).Any())
                                {
                                    subject.Color =
                                        ((Color)Application.Current.Resources[$"SubjectColor_Aux{aux_suffix}"])
                                        .ToHex();
                                    aux_suffix++;
                                }

                                AppData.Instance.LiteConnection.Insert(subject);
                            }

                            int begin_hour = Convert.ToInt32(match.Groups["begin_hour"].Value);
                            int begin_minutes = Convert.ToInt32(match.Groups["begin_minutes"].Value);
                            int end_hour = Convert.ToInt32(match.Groups["end_hour"].Value);
                            int end_minutes = Convert.ToInt32(match.Groups["end_minutes"].Value);

                            TimeSpan begin = TimeSpan.FromHours(begin_hour).Add(TimeSpan.FromMinutes(begin_minutes));
                            TimeSpan end = TimeSpan.FromHours(end_hour).Add(TimeSpan.FromMinutes(end_minutes));
                            ClassTime classTime = new ClassTime()
                            {
                                Begin = begin,
                                End = end,
                                Day = (DayOfWeek)i - 5,
                                IdSubject = subject.Id
                            };
                            AppData.Instance.LiteConnection.Insert(classTime);
                        }
                    }


                }
            }
        }
        private async Task GetKardexInfo()
        {
            await GoTo(KardexPage);
            string carrera = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_Lbl_Carrera').innerHTML");

        }
        private async Task GetCitasReinscripcionInfo()
        {
            await GoTo(CitaReinscripcionPage);
            double creditos_totales = 0;
            double creditos_alumno = 0;

            string creditos_carrera_html = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_CREDITOSCARRERA\").outerHTML");
            string alumno_html = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_alumno\").outerHTML");

            alumno_html = System.Text.RegularExpressions.Regex.Unescape(alumno_html);
            if (!string.IsNullOrEmpty(alumno_html))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(alumno_html);
                HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                    .ToList();
                creditos_alumno = Convert.ToDouble(table[0][1]);
            }

            creditos_carrera_html = System.Text.RegularExpressions.Regex.Unescape(creditos_carrera_html);
            if (!string.IsNullOrEmpty(creditos_carrera_html))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(creditos_carrera_html);
                HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
                List<List<string>> table = htable
                    .Descendants("tr")
                    .Skip(1)
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                    .ToList();
                creditos_totales = Convert.ToDouble(table[0][1]);
            }

            double result = (creditos_alumno / creditos_totales) * 100;
            result = Math.Round(result, 2);
        }
        public async Task GetSchoolGrades()
        {

            await GoTo(CalificacionesPage);
            string grades_html = await this.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_GV_Calif\").outerHTML");
            if (string.IsNullOrEmpty(grades_html))
            {
                return;
            }
            grades_html = System.Text.RegularExpressions.Regex.Unescape(grades_html);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(grades_html);
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
                string grupo = row[0];
                Grade[] row_grades = new Grade[5];
                for (int j = 2; j <= 6; j++)
                {
                    int index = j - 2;
                    string score = row[j];
                    if (score == "&nbsp;")
                    {
                        score = "-";
                    }
                    row_grades[index] = new Grade((Partial)index, score, Subject.GetId(grupo));
                }
                grades.AddRange(row_grades);
            }

            foreach (var grade in grades)
            {
                AppData.Instance.LiteConnection.Table<Grade>()
                    .Delete(x => x.Partial == grade.Partial && x.SubjectId == grade.SubjectId);
                AppData.Instance.LiteConnection.Insert(grade);
            }

            //if (AppShell.Current is AppShell shell)
            //{
            //    if (shell.MasterPage.TabView.TabItems[0].Content is SchoolGrades view)
            //    {
            //        view.Model.HasBeenRefreshedCommand?.Execute(this);
            //    }
            //}
            //if (OnFinished is null)
            //{
            //    //Reboot app
            //    App.Current.MainPage = new SplashScreen();
            //}
            //else
            //{

            //}
        }

        public async Task<IEnumerable<School>> GetSchools(SchoolLevel Level)
        {
            await GoTo(Level == SchoolLevel.University ? UniversitiesPage : HighSchoolsPage);
            string html = await this.EvaluateJavaScriptAsync("document.getElementById('botones_esc').outerHTML");
            html = System.Text.RegularExpressions.Regex.Unescape(html);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var ul = doc.DocumentNode.SelectSingleNode("//ul");

            var lis = ul.Descendants("li");
            List<School> schools = new List<School>();
            foreach (var li in lis)
            {
                var a = li.Descendants("a").First();
                var img = a.Descendants("img").First();
                schools.Add(new School(a.Attributes["href"].Value, img.Attributes["alt"].Value.Trim(), SaesHomePage + "/" + img.Attributes["src"].Value.Trim()));
            }

            return schools;
        }
    }
}
