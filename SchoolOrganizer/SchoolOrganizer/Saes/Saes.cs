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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Application = Xamarin.Forms.Application;
using WebView = Xamarin.Forms.WebView;

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
        private NavigationRequest CurrentRequest;
        private readonly Queue<NavigationRequest> NavigationQueue;
        public School School { get; set; }
        private bool _IsNavigating;
        public bool IsNavigating
        {
            get => _IsNavigating;
            private set
            {
                _IsNavigating = value;
            }
        }

        public SAES() : this(new School())
        {

        }
        public SAES(School School)
        {
            this.IsPlatformEnabled = true;
            this.On<Windows>().SetIsJavaScriptAlertEnabled(true);
            this.On<Windows>().SetExecutionMode(WebViewExecutionMode.SeparateProcess);

            this.School = School;
            this.Navigated += Browser_Navigated;
            this.NavigationQueue = new Queue<NavigationRequest>();
            Init();
        }

        private async void Init()
        {
            NavigateAsync();
            if (this.School.IsSchoolSelected)
                await GoTo(this.School.HomePage);

        }
        private async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Timeout || e.Result == WebNavigationResult.Cancel ||
                e.Result == WebNavigationResult.Failure)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert(
                    "No fue posible conectarse al SAES, verifique si el sitio esta activo", "Sin conexión");
                return;
            }
            await this.EvaluateJavaScriptAsync(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { alert('Error occured: ' + errorMsg); return false; }");
            if (e.Url is null)
            {
                await GoTo(School.HomePage);
                return;
            }
            if (IsOn(e.Source as UrlWebViewSource, this.CurrentRequest))
            {
                this.CurrentRequest.Done();
            }
            else
            {
                //await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"I dont know what to do on =>[{e.Url}]");
                //Log.Logger.Debug($"I dont know what to do on =>[{e.Url}]");
                //await GoTo(School.HomePage);
                Log.Logger.Debug($"Landed unexpectly at =>[{e.Url}]");
            }
        }
        public async Task GoTo(string url)
        {
            string navigateUrl = url;
            if (url != School.HomePage)
            {
                navigateUrl = School.HomePage + url;
            }
            await Task.Run(() =>
            {
                while (this.IsNavigating)
                {
                }
            });
            var request = new NavigationRequest(navigateUrl);
            NavigationQueue.Enqueue(request);
            NavigateAsync();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Espere un momento..."))
            {
                await request.Wait();
                await Task.Run(() =>
                {
                    while (!request.IsComplete)
                    {
                    }
                });
            }
        }
        private async void NavigateAsync()
        {
            this.IsNavigating = true;
            await Task.Yield();
            try
            {
                while (NavigationQueue.Any())
                {
                    this.CurrentRequest = NavigationQueue.Dequeue();
                    Source = new UrlWebViewSource()
                    {
                        Url = this.CurrentRequest.Url.AbsoluteUri
                    };
                    Log.Logger.Debug("Requested:{0}", CurrentRequest);
                    await this.CurrentRequest.Wait();
                    this.CurrentRequest.IsComplete = true;
                    Log.Logger.Debug("Complete:{0}", CurrentRequest);
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "NavigateAsync");
            }
            finally
            {
                this.IsNavigating = false;
            }
        }

        public async Task<ImageSource> GetCaptcha()
        {
            ImageSource ImageSource = null;
            await GoTo(School.HomePage);
            await Task.Delay(TimeSpan.FromSeconds(2)); //dale tiempo al captcha para cargar
            StringBuilder sb = new StringBuilder();
            sb.Append("var getDataUrl = function (img) {")
                .Append("var canvas = document.createElement('canvas');")
                .Append("var ctx = canvas.getContext('2d');")
                // If the image is not png, the format
                // must be specified here
                .Append("canvas.width = img.width;")
                .Append("canvas.height = img.height;")
                .Append("ctx.drawImage(img, 0, 0);")
                .Append("return canvas.toDataURL();")
                .Append("};")
                .Append("var img=document.getElementById('c_default_ctl00_leftcolumn_loginuser_logincaptcha_CaptchaImage');")
                .Append("getDataUrl(img);");

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                string base_64 = await EvaluateJavaScriptAsync(sb.ToString());
                base_64 = base_64.Replace("data:image/png;base64,", string.Empty);
                ImageSource =
                    Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base_64)));
            }
            return ImageSource;
        }

        private bool UriCompare(Uri uri1, Uri uri2)
        {
            var result = Uri.Compare(uri1, uri2,
                UriComponents.Host | UriComponents.PathAndQuery,
                UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
            return result == 0;
        }
        private bool IsOn(UrlWebViewSource source, NavigationRequest request)
        {
            if (source is null)
            {
                return false;
            }
            if (UriCompare(new Uri(source.Url), request.Url))
            {
                return true;
            }
            string path = new Uri(source.Url).AbsolutePath.ToLower();
            if ((path == "/default.aspx" || path == "/") && UriCompare(request.Url, new Uri(School.HomePage)))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsLoggedIn()
        {
            try
            {
                return
                    !string.IsNullOrEmpty(await EvaluateJavaScriptAsync(
                        "document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML"));
            }
            catch (Exception) { return false; }
        }
        public async Task<string> GetCurrentUser()
        {
            return await EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML");
        }
        public async Task LogOut()
        {
            await EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginStatusSession').click();");
            await Task.Delay(TimeSpan.FromSeconds(2)); //dale tiempo para redireccionar
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
                    $"document.getElementById('ctl00_leftColumn_LoginUser_UserName').value = '{login.User.Boleta}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_Password').value = '{Password}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_CaptchaCodeTextBox').value ='{login.Captcha}';");
                await Task.Delay(100);
                await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginUser_LoginButton').click();");
                await Task.Delay(TimeSpan.FromSeconds(2));
                await GoTo(School.HomePage);
                if (await IsLoggedIn())
                {
                    if (ShouldGetUserData)
                        await GetUserData(login.User);
                    return true;
                }
            }
            else
            {
                await GoTo(School.HomePage);
                Acr.UserDialogs.UserDialogs.Instance.Alert("Tienes varios intentos fallidos. Es posible que la aplicación no pueda comunicarse correctamente con el SAES o tus datos sean incorrectos." +
                                                           " Si continuas es posible que tu cuenta sea suspendida.", "Cuidado!", "Entiendo");
            }
            return false;
        }
        private async Task GetUserData(User user)
        {
            AppData.Instance.User = user;
            user.IsLogedIn = true;
            user.School = this.School;
            await GetName();
            await GetHorario();
            await GetKardexInfo();
            await GetCitasReinscripcionInfo();
            await GetSchoolGrades();
            AppData.Instance.LiteConnection.InsertOrReplace(AppData.Instance.User);
        }
        private async Task GetName()
        {
            await GoTo(AlumnosPage);
            AppData.Instance.User.Name = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_FormView1_nombrelabel').innerHTML;");
            AppData.Instance.User.Boleta = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML;");
        }
        private async Task GetHorario()
        {
            await GoTo(HorariosPage);
            if (DataInfo.HasTimeTable())
            {
                return;
            }
            string horario_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_GV_Horario').outerHTML");
            Unescape(ref horario_html);

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
            AppData.Instance.User.Career = carrera;
        }
        private async Task GetCitasReinscripcionInfo()
        {
            await GoTo(CitaReinscripcionPage);
            double creditos_totales = 0;
            double creditos_alumno = 0;

            string creditos_carrera_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_CREDITOSCARRERA').outerHTML");
            string alumno_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_alumno').outerHTML");

            Unescape(ref alumno_html);
            if (!string.IsNullOrEmpty(alumno_html))
            {
                var table = HtmlToTable(alumno_html);
                creditos_alumno = Convert.ToDouble(table[0][1]);
            }

            Unescape(ref creditos_carrera_html);
            if (!string.IsNullOrEmpty(creditos_carrera_html))
            {
                List<List<string>> table = HtmlToTable(creditos_carrera_html);
                creditos_totales = Convert.ToDouble(table[0][1]);
            }

            double result = (creditos_alumno / creditos_totales) * 100;
            result = Math.Round(result, 2);

            Credits credits = new Credits
            {
                CurrentCredits = creditos_alumno,
                TotalCredits = creditos_totales,
                Percentage = result,
                UserId = AppData.Instance.User.Boleta
            };
            AppData.Instance.LiteConnection.InsertOrReplace(credits);
        }

        private List<List<string>> HtmlToTable(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode htable = doc.DocumentNode.SelectSingleNode("//table");
            return htable
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();
        }
        public async Task GetSchoolGrades()
        {

            await GoTo(CalificacionesPage);
            string grades_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_GV_Calif').outerHTML");
            if (string.IsNullOrEmpty(grades_html))
            {
                return;
            }
            Unescape(ref grades_html);
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

        private static void Unescape(ref string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }
            html = System.Text.RegularExpressions.Regex.Unescape(html);
        }
        private HtmlDocument GetHtmlDoc(string html)
        {
            Unescape(ref html);
            var doc = new HtmlAgilityPack.HtmlDocument();
            if (string.IsNullOrEmpty(html))
            {
                return doc;
            }
            doc.LoadHtml(html);
            return doc;
        }

        private async Task<string> _EvaluateJavaScriptAsync(string script)
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); //dale tiempo para cargar
            return
                await this.EvaluateJavaScriptAsync(script);
        }
        public async Task<IEnumerable<School>> GetSchools(SchoolLevel Level)
        {
            List<School> schools = new List<School>();
            await GoTo(Level == SchoolLevel.University ? UniversitiesPage : HighSchoolsPage);
            string html = await _EvaluateJavaScriptAsync(
                "document.getElementById(\"botones_esc\").outerHTML;");
            Unescape(ref html);
            if (!string.IsNullOrEmpty(html))
            {
                HtmlAgilityPack.HtmlDocument doc = GetHtmlDoc(html);
                var ul = doc.DocumentNode.SelectSingleNode("//ul");
                var lis = ul.Descendants("li");
                foreach (var li in lis)
                {
                    var a = li.Descendants("a").First();
                    var img = a.Descendants("img").First();
                    schools.Add(new School(a.Attributes["href"].Value, img.Attributes["alt"].Value.Trim(),
                        SaesHomePage + "/" + img.Attributes["src"].Value.Trim()));
                }
            }
            return schools;
        }



    }
}
