using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Widgets.Horario;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Log = Kit.Log;

namespace SchoolOrganizer.Saes
{
    public class Saes : IBrowser
    {
        private const string HomePage = "https://www.saes.esimecu.ipn.mx";
        private const string AlumnosPage = "/alumnos/default.aspx";
        private const string HorariosPage = "/alumnos/informacion_semestral/horario_alumno.aspx";
        private LogIn LogIn { get; set; }
        private bool IsCheckingSession { get; set; }
        public bool IsLogedIn { get => LogIn?.IsLogedIn ?? false; }
        private Action<WebNavigatedEventArgs> OnNavigated { get; set; }
        public WebView Browser { get; set; }
        private LoginViewModel LoginViewModel { get; set; }
        public Saes()
        {

            this.IsCheckingSession = false;
        }

        private void Browser_Navigating(object sender, WebNavigatingEventArgs e)
        {
            this.IsCheckingSession = true;
        }

        private async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.IsCheckingSession = false;
            await this.Browser.EvaluateJavaScriptAsync(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { alert(""Error occured: "" + errorMsg); return false;");
            //OnNavigated?.Invoke(e);
            //OnNavigated = null;
            this.LogIn.IsLogedIn =
                !string.IsNullOrEmpty(await Browser.EvaluateJavaScriptAsync(
                    "document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML"));
            if (e.Url is null)
            {
                GoTo(HomePage);
                return;
            }
            Uri Uri = new Uri(e.Url);
            switch (Uri.AbsolutePath.ToLower())
            {
                case "/default.aspx":
                case "/": //HomePage
                    if (!IsLogedIn)
                    {
                        //AppData.Instance.User = null;
                        LoginViewModel.CaptchaImg = await this.LogIn.GetCaptcha();
                    }
                    else
                    {
                        AppData.Instance.User.IsLogedIn = true;
                        GoTo(AlumnosPage);
                    }
                    break;
                case AlumnosPage:
                    GetName();
                    break;
                case HorariosPage:
                    GetHorario();
                    break;
                default:
                    //await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"I dont know what to do on =>[{e.Url}]");
                    Log.Logger.Debug($"I dont know what to do on =>[{e.Url}]");
                    GoTo(HomePage);
                    break;
            }
        }

        private async void GetHorario()
        {
            if (DataInfo.HasTimeTable())
            {
                return;
            }
            string horario_html = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_GV_Horario\").outerHTML");
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
                List<Materia> subjects = new List<Materia>();
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
            //Reboot app
            App.Current.MainPage = new SplashScreen();
        }

        private void GoTo(string url)
        {
            string navigateUrl = url;
            if (url != HomePage)
            {
                navigateUrl = HomePage + url;
            }
            Browser.Source = new UrlWebViewSource()
            {
                Url = navigateUrl
            };
        }

        private async void GetName()
        {
            AppData.Instance.User.Name = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_mainCopy_FormView1_nombrelabel\").innerHTML;");
            AppData.Instance.User.Boleta = await this.Browser.EvaluateJavaScriptAsync("document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML;");
            //AppData.Instance.User.RemeberMe = false;

            this.LoginViewModel.OnLoginSuccess.Execute(this);
            GoTo(HorariosPage);
        }
        public void OnLogIn()
        {
            this.LoginViewModel = new LoginViewModel(LoginRequested);
            var loginpage = new LoginPage(LoginViewModel);
            App.Current.MainPage = loginpage;

            Browser = loginpage.Browser;
            //Browser.IsNativeStateConsistent = true;
            //Browser.IsInNativeLayout = true;
            //Browser.IsPlatformEnabled = true;

            this.Browser.Navigated += Browser_Navigated;
            this.Browser.Navigating += Browser_Navigating;
            this.LogIn = new LogIn(this.Browser);

            GoTo(HomePage);
            //OnNavigated = (async (e) =>
            //{
            //    if (e.Url == HomePage)
            //        loginModel.CaptchaImg = await this.LogIn.GetCaptcha();
            //    if(e.Url=="")
            //});
        }


        private void LoginRequested(LoginViewModel obj)
        {
            this.LogIn.RequestLogIn(obj.User, obj.Captcha);
            //OnNavigated = ((e) =>
            //{

            //});
        }
    }
}
