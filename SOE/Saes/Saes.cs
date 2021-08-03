using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using HtmlAgilityPack;
using Kit;
using SOE.API;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Data;
using SOE.Services;
using SOE.Widgets;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using WebView = Xamarin.Forms.WebView;
using Kit.Forms.Controls.WebView;
using SOE.Data.Images;
using SOE.Models;
using SOE.Models.TaskFirst;
using Xamarin.CommunityToolkit.Extensions;

namespace SOE.Saes
{
    public class SAES : WebView
    {
        private const string AlumnosPage = "alumnos/default.aspx";
        private const string HorariosPage = "alumnos/informacion_semestral/horario_alumno.aspx";
        private const string KardexPage = "alumnos/boleta/kardex.aspx";
        private const string CitaReinscripcionPage = "alumnos/reinscripciones/fichas_reinscripcion.aspx";
        private const string CalificacionesPage = "alumnos/informacion_semestral/calificaciones_sem.aspx";

        private NavigationRequest CurrentRequest;
        private readonly Queue<NavigationRequest> NavigationQueue;

        private bool _IsNavigating;
        public bool IsNavigating
        {
            get => _IsNavigating;
            private set
            {
                _IsNavigating = value;
            }
        }
        public bool ShowLoading { get; set; }

        public SAES()
        {
            this.IsPlatformEnabled = true;
            this.On<Windows>().SetIsJavaScriptAlertEnabled(true);
            this.On<Windows>().SetExecutionMode(WebViewExecutionMode.SeparateProcess);
            this.ShowLoading = true;
            this.Navigated += Browser_Navigated;
            this.NavigationQueue = new Queue<NavigationRequest>();

        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
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
            await this.EvaluateJavaScriptAsync(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { console.log('Error occured: ' + errorMsg); return false; }");
            if (e.Url is null)
            {
                await GoTo(AppData.Instance.User.School.HomePage);
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

        public Task GoHome() => GoTo(AppData.Instance.User.School.HomePage);
        public async Task GoTo(string url)
        {
            string navigateUrl = url;
            if (url != AppData.Instance.User.School.HomePage)
            {
                navigateUrl = AppData.Instance.User.School.HomePage;
                if (!navigateUrl.EndsWith("/"))
                {
                    navigateUrl +='/';
                }
                navigateUrl += url;
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
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Espere un momento...", show: ShowLoading))
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
                    OnPropertyChanged(nameof(Source));
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
            await GoHome();
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
                if (string.IsNullOrEmpty(base_64))
                {
                    return null;
                }
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
            if ((path == "/default.aspx" || path == "/") && UriCompare(request.Url, new Uri(AppData.Instance.User.School.HomePage)))
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
                        "var label= document.getElementById('ctl00_leftColumn_LoginNameSession'); if(label) label.innerHTML; else ''"));
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
            await GoTo(AppData.Instance.User.School.HomePage);

        }
        public async Task<bool> LogIn(string captcha, int AttemptCount, bool ShouldGetUserData = true)
        {
            if (AttemptCount < 3)
            {
                Regex regex = new Regex("[\"\\\\]");
                string Password = regex.Replace(AppData.Instance.User.Password, "\\");
                //binding.captchaDisplayer.visibility = View.GONE
                await this.EvaluateJavaScriptAsync(
                    $"document.getElementById('ctl00_leftColumn_LoginUser_UserName').value = '{AppData.Instance.User.Boleta}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_Password').value = '{AppData.Instance.User.Password}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_CaptchaCodeTextBox').value ='{captcha}';"
                    );
                await Task.Delay(100);
                await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginUser_LoginButton').click();");
                await Task.Delay(TimeSpan.FromSeconds(2));
                await GoTo(AppData.Instance.User.School.HomePage);
                if (await IsLoggedIn())
                {
                    if (ShouldGetUserData)
                        await GetUserData(AppData.Instance.User);
                    return true;
                }
            }
            else
            {
                await GoTo(AppData.Instance.User.School.HomePage);
                Acr.UserDialogs.UserDialogs.Instance.Alert("Tienes varios intentos fallidos. Es posible que la aplicación no pueda comunicarse correctamente con el SAES o tus datos sean incorrectos." +
                                                           " Si continuas es posible que tu cuenta sea suspendida.", "Cuidado!", "Entiendo");
            }
            return false;
        }
        public async Task GetUserData(User user)
        {
            AppData.Instance.ClearData(
                typeof(Subject), typeof(ClassTime), typeof(Document),
                typeof(DocumentPart), typeof(Archive), typeof(Grade),
                typeof(Keeper), typeof(Teacher), typeof(ToDo),typeof(Reminder));
            Keeper.ClearAllFiles();

            AppData.Instance.User = user;
            await GetName();
            bool HasSubjects = await GetSubjects();
            await GetKardexInfo();
            await GetCitasReinscripcionInfo();
            if (HasSubjects)
            {
                await GetGrades();
            }
            else
            {
                Shell.Current.CurrentPage.DisplayAlert(
                        title: "Sin inscripción",
                        message: "Actualmente no estas inscrito en ninguna materia.", "Ok")
                    .SafeFireAndForget();
            }
            AppData.Instance.User.HasSubjects = HasSubjects;
            AppData.Instance.LiteConnection.InsertOrReplace(AppData.Instance.User);
        }
        public async Task GetName()
        {
            await GoTo(AlumnosPage);
            AppData.Instance.User.Name = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_FormView1_nombrelabel').innerHTML;");
            AppData.Instance.User.Boleta = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML;");
        }
        private async Task<bool> GetSubjects()
        {
            await GoTo(HorariosPage);
            //if (DataInfo.HasTimeTable())
            //{
            //    return;
            //}
            string horario_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_GV_Horario').outerHTML");
            Unescape(ref horario_html);
            if (string.IsNullOrEmpty(horario_html))
            {
                return false;
            }
            Response response = await APIService.PostClassTime(System.Text.Encoding.UTF8.GetBytes(horario_html), AppData.Instance.User.Boleta);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Log.Logger.Debug(response.Extra);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response.Message);
                XmlNodeList nodes = xmlDoc.SelectNodes("//Teachers/Teacher");
                foreach (XmlNode t_node in nodes)
                {
                    TeacherService.Save(t_node.ConvertNode<Teacher>());
                }
                nodes = xmlDoc.SelectNodes("//Subjects/Subject");
                foreach (XmlNode t_node in nodes)
                {
                    SubjectService.Save(t_node.ConvertNode<Subject>());
                }
                if (nodes.Count <= 0)
                {
                    return false;
                }
                nodes = xmlDoc.SelectNodes("//ClassTimes/ClassTime");
                foreach (XmlNode t_node in nodes)
                {
                    var class_time = t_node.ConvertNode<ClassTime>();
                    ClassTimeService.Save(class_time);
                }
                TimeLineWidget.UpdateWidget();
                return true;
            }
            return false;
        }
        private async Task GetKardexInfo()
        {
            await GoTo(KardexPage);
            string carrera = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_Lbl_Carrera').innerHTML");
            AppData.Instance.User.Career = carrera;

            Response response = await APIService.PostCareer(carrera, AppData.Instance.User.Boleta);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                AppData.Instance.LiteConnection.DeleteAll<Career>();
                AppData.Instance.LiteConnection.Insert(new Career()
                {
                    Name = carrera,
                    Id = Convert.ToInt32(response.Message)
                });
            }
        }
        private async Task GetCitasReinscripcionInfo()
        {
            await GoTo(CitaReinscripcionPage);
            double creditos_totales = 0;
            double creditos_alumno = 0;

            string creditos_carrera_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_CREDITOSCARRERA').outerHTML");
            string alumno_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_alumno').outerHTML");
            string cita_reinscripcion_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_grvEstatus_alumno').outerHTML");

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

            //Save fecha de reinscripción
            Unescape(ref cita_reinscripcion_html);
            if (!string.IsNullOrEmpty(cita_reinscripcion_html))
            {
                List<List<string>> table = HtmlToTable(cita_reinscripcion_html);
                string date = table[0][3];
                InscriptionDate IDate = new InscriptionDate(date);
                IDate.Save();
            }
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
        public async Task GetGrades()
        {
            await GoTo(CalificacionesPage);
            string grades_html = await this.EvaluateJavaScriptAsync("document.getElementById('ctl00_mainCopy_GV_Calif').outerHTML");
            Unescape(ref grades_html);
            if (string.IsNullOrEmpty(grades_html))
            {
                return;
            }
            Response response = await APIService.PostGrades(System.Text.Encoding.UTF8.GetBytes(grades_html));
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Log.Logger.Debug(response.Extra);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response.Message);
                XmlNodeList nodes = xmlDoc.SelectNodes("//Grades/Grade");
                foreach (XmlNode t_node in nodes)
                {
                    GradeService.Save(t_node.ConvertNode<Grade>());
                }
            }
        }
        private static void Unescape(ref string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }
            html = System.Text.RegularExpressions.Regex.Unescape(html);
        }
        private async Task<string> _EvaluateJavaScriptAsync(string script)
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); //dale tiempo para cargar
            return
                await this.EvaluateJavaScriptAsync(script);
        }

    }
}
