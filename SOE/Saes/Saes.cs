using AsyncAwaitBestPractices;
using HtmlAgilityPack;
using Kit;
using Kit.Forms.Controls.WebView;
using Microsoft.AppCenter.Crashes;
using SOE.Data;
using SOE.Models;
using SOE.Models.Academic;
using SOE.Models.Data;
using SOE.Notifications;
using SOE.Processors;
using SOE.Widgets;
using SOEWeb.Shared.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using WebView = Xamarin.Forms.WebView;

namespace SOE.Saes
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class Saes : WebView
    {
        private const string AlumnosPage = "alumnos/default.aspx";
        private const string HorariosPage = "alumnos/informacion_semestral/horario_alumno.aspx";
        private const string KardexPage = "alumnos/boleta/kardex.aspx";
        private const string CitaReinscripcionPage = "alumnos/reinscripciones/fichas_reinscripcion.aspx";
        private const string CalificacionesPage = "alumnos/informacion_semestral/calificaciones_sem.aspx";
        private const string CambioCorreoPersonal = "alumnos/cambiocorreopersonal.aspx";

        private NavigationRequest _currentRequest;
        private readonly Queue<NavigationRequest> _navigationQueue;

        private bool _isNavigating;
        public bool IsNavigating
        {
            get => this._isNavigating;
            private set
            {
                this._isNavigating = value;
            }
        }
        public bool ShowLoading { get; set; }

        public Saes()
        {
            this.IsPlatformEnabled = true;
            this.On<Windows>().SetIsJavaScriptAlertEnabled(true);
            this.On<Windows>().SetExecutionMode(WebViewExecutionMode.SeparateProcess);
            this.ShowLoading = true;
            this.Navigated += (s, e) => Browser_Navigated(s, e).SafeFireAndForget();
            this._navigationQueue = new Queue<NavigationRequest>();
            this.Visual = VisualMarker.Material;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
        }

        private async Task Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Timeout || e.Result == WebNavigationResult.Cancel)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert(
                    $"No fue posible conectarse al SAES, verifique si el sitio esta activo - {e.Result}", "Sin conexión");
                return;
            }
            await this.EvaluateJavaScript(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { console.log('Error occured: ' + errorMsg); return false; }");
            if (e.Url is null)
            {
                await GoTo(AppData.Instance.User.School.HomePage);
                return;
            }
            if (IsOn(e.Source as UrlWebViewSource, this._currentRequest))
            {
                this._currentRequest.Done();
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
            await Task.Yield();
            string navigateUrl = url;
            if (url != AppData.Instance.User.School.HomePage)
            {
                navigateUrl = AppData.Instance.User.School.HomePage;
                if (!navigateUrl.EndsWith("/"))
                {
                    navigateUrl += '/';
                }
                navigateUrl += url;
            }
            await Task.Run(() =>
            {
                while (this.IsNavigating)
                {
                }
            });
            NavigationRequest? request = new NavigationRequest(navigateUrl);
            this._navigationQueue.Enqueue(request);
            await NavigateAsync();
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
            await Task.Delay(500); //Esperar el html
        }
        private async Task NavigateAsync()
        {
            this.IsNavigating = true;
            await Task.Yield();
            try
            {
                while (this._navigationQueue.Any())
                {
                    this._currentRequest = this._navigationQueue.Dequeue();
                    Source = new UrlWebViewSource()
                    {
                        Url = this._currentRequest.Url.AbsoluteUri
                    };
                    OnPropertyChanged(nameof(Source));
                    Log.Logger.Debug("Requested:{0}", this._currentRequest);
                    await this._currentRequest.Wait();
                    this._currentRequest.IsComplete = true;
                    Log.Logger.Debug("Complete:{0}", this._currentRequest);
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

        public async Task<ImageSource> GetCaptcha(bool recursive = false)
        {
            ImageSource imageSource = null;
            await GoHome();
            await Task.Delay(TimeSpan.FromSeconds(2)); //dale tiempo al captcha para cargar
            string sb = new StringBuilder().Append("var getDataUrl = function (img) {")
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
                .Append("getDataUrl(img);").ToString();

            if (!string.IsNullOrEmpty(sb))
            {
                string base64 = await EvaluateJavaScript(sb.ToString());
                if (string.IsNullOrEmpty(base64))
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    if (!recursive)
                        return await GetCaptcha(true);
                    return null;
                }
                base64 = base64.Replace("data:image/png;base64,", string.Empty);
                imageSource =
                    Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));
            }
            return imageSource;
        }

        private bool UriCompare(Uri uri1, Uri uri2)
        {
            int result = Uri.Compare(uri1, uri2,
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
        private async Task<string> EvaluateJavaScript(string script)
        {
            await Task.Delay(50);
            try
            {
                return await EvaluateJavaScriptAsync(script);
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "EvaluateJavaScript - {0}", script);
                return String.Empty;
            }

        }

        public async Task<bool> IsLoggedIn()
        {
            await Task.Yield();
            try
            {
                return
                    !string.IsNullOrEmpty(await EvaluateJavaScript(
                        "var label= document.getElementById('ctl00_leftColumn_LoginNameSession'); if(label) label.innerHTML; else ''"));
            }
            catch (Exception ex) { Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "IsLoggedIn"); return false; }
        }
        public async Task<string> GetCurrentUser()
        {
            return await EvaluateJavaScript("document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML");
        }
        public async Task LogOut()
        {
            await EvaluateJavaScript("document.getElementById('ctl00_leftColumn_LoginStatusSession').click();");
            await Task.Delay(TimeSpan.FromSeconds(2)); //dale tiempo para redireccionar
            await GoTo(AppData.Instance.User.School.HomePage);

        }
        public async Task<bool> LogIn(string captcha, int attemptCount, bool shouldGetUserData = true)
        {
            await Task.Yield();
            if (attemptCount < 3)
            {
                Regex regex = new Regex("[\"\\\\]");
                string password = regex.Replace(UserLocalData.Instance.Password, "\\");
                //binding.captchaDisplayer.visibility = View.GONE
                await this.EvaluateJavaScript(
                    $"document.getElementById('ctl00_leftColumn_LoginUser_UserName').value = '{AppData.Instance.User.Boleta}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_Password').value = '{UserLocalData.Instance.Password}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_CaptchaCodeTextBox').value ='{captcha}';"
                    );
                await Task.Delay(100);
                await this.EvaluateJavaScript("const button_login=document.getElementById('ctl00_leftColumn_LoginUser_LoginButton'); button_login.focus();button_login.click();");
                await Task.Delay(TimeSpan.FromSeconds(3));
                await GoTo(AppData.Instance.User.School.HomePage);
                if (await IsLoggedIn())
                {
                    if (shouldGetUserData)
                        await GetUserData();
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

        public async Task GetUserData()
        {
            await Task.Yield();
            ILocalNotificationService notification = DependencyService.Get<ILocalNotificationService>();
            //AppData.Instance.ClearData(
            //    typeof(Subject), typeof(ClassTime), typeof(Document),
            //    typeof(DocumentPart), typeof(Archive), typeof(Grade),
            //    typeof(Keeper), typeof(Teacher), typeof(ToDo), typeof(Reminder));
            //Keeper.ClearAllFiles();
            notification.UnScheduleAll();
            await GetName();
            bool hasSubjects = (await GetSubjects()).Any();
            await GetKardexInfo();
            await GetCitasReinscripcionInfo();
            if (hasSubjects)
            {
                await GetGrades();
                AppData.Instance.User.Semester = await CalculateSemester();
                notification.ScheduleAll();
            }
            else
            {
                AppData.Instance.User.Semester = "Ninguno";
                Acr.UserDialogs.UserDialogs.Instance.AlertAsync(
                        "Actualmente no estas inscrito en ninguna materia.",
                        "Sin inscripción", "Ok").SafeFireAndForget();
            }
            AppData.Instance.User.HasSubjects = hasSubjects;
            await AppData.Instance.User.Save();
        }

        private async Task<string> CalculateSemester()
        {
            string semester = "?";
            try
            {
                IEnumerable<ClassTime> classTimes = await ClassTime.GetAll();
                Models.Group[] groups = await Task.WhenAll(classTimes.Select(x => x.GetGroup()).ToArray());
                semester = string.Join(",", groups.Select(x => x.Name).Select(x => x.FirstOrDefault()).Distinct());
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Al calcular el semestre en curso");
            }
            return semester;
        }

        public async Task GetName()
        {
            await GoTo(AlumnosPage);
            AppData.Instance.User.Name = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_FormView1_nombrelabel').innerHTML;");
            AppData.Instance.User.Boleta = await this.EvaluateJavaScript("document.getElementById('ctl00_leftColumn_LoginNameSession').innerHTML;");
        }
        public async Task GetEmail()
        {
            await GoTo(CambioCorreoPersonal);
            AppData.Instance.User.Email = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_txtcorreoper').value;");
        }
        private async Task<IEnumerable<Subject>> GetSubjects()
        {
            await Task.Yield();
            IEnumerable<Subject>? subjects = null;
            try
            {
                await GoTo(HorariosPage);
                string horarioHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_GV_Horario').outerHTML");
                Unescape(ref horarioHtml);
                if (string.IsNullOrEmpty(horarioHtml))
                {
                    return new List<Subject>();
                }
                subjects = await DigestSubjects(horarioHtml);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SOE.iOS" + ex.ToString());
                Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString());
            }
            return subjects ?? new List<Subject>();
        }
        private async Task<List<Subject>> DigestSubjects(string horarioHtml)
        {
            await Task.Yield();
            List<Subject>? subjects = await ClassTimeDigester.Digest(horarioHtml);
            TimeLineWidget.UpdateWidget();
            return subjects;
        }
        private async Task GetKardexInfo()
        {
            await Task.Yield();
            await GoTo(KardexPage);
            string carrera = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_Lbl_Carrera').innerHTML");
            AppData.Instance.User.Career = carrera;
            if (string.IsNullOrEmpty(carrera))
            {
                return;
            }
        }
        private async Task GetCitasReinscripcionInfo()
        {
            await Task.Yield();
            try
            {
                await GoTo(CitaReinscripcionPage);
                float creditosTotales = 0f;
                float creditosAlumno = 0f;

                string creditosCarreraHtml =
                    await this.EvaluateJavaScript(
                        "document.getElementById('ctl00_mainCopy_CREDITOSCARRERA').outerHTML");
                string alumnoHtml =
                    await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_alumno').outerHTML");
                string citaReinscripcionHtml =
                    await this.EvaluateJavaScript(
                        "document.getElementById('ctl00_mainCopy_grvEstatus_alumno').outerHTML");

                Unescape(ref alumnoHtml);
                if (!string.IsNullOrEmpty(alumnoHtml))
                {
                    List<List<string>>? table = HtmlToTable(alumnoHtml);
                    float.TryParse(table[0][1], out creditosAlumno);
                }

                Unescape(ref creditosCarreraHtml);
                if (!string.IsNullOrEmpty(creditosCarreraHtml))
                {
                    List<List<string>> table = HtmlToTable(creditosCarreraHtml);
                    float.TryParse(table[0][1], out creditosTotales);
                }

                float result = 0f;
                if (creditosTotales != 0)
                {
                    result = (creditosAlumno / creditosTotales) * 100;
                    result = (float)Math.Round(result, 2);
                }

                AppData.Instance.User.Credits =
                    new Credits
                    {
                        //Id = 1,
                        CurrentCredits = creditosAlumno,
                        TotalCredits = creditosTotales,
                        Percentage = result
                    };
                //Save fecha de reinscripción
                Unescape(ref citaReinscripcionHtml);
                if (!string.IsNullOrEmpty(citaReinscripcionHtml))
                {
                    List<List<string>> table = HtmlToTable(citaReinscripcionHtml);
                    string _date = table[0][3];
                    AppData.Instance.User.InscriptionDate = _date;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "GetCitasReinscripcionInfo");
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
        public async Task<bool> GetGrades()
        {
            await Task.Yield();
            await GoTo(CalificacionesPage);
            string gradesHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_GV_Calif').outerHTML");
            Unescape(ref gradesHtml);
            if (string.IsNullOrEmpty(gradesHtml))
            {
                return false;
            }
            await GradesDigester.Digest(gradesHtml);
            return true;


        }

        private static void Unescape(ref string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }
            html = System.Text.RegularExpressions.Regex.Unescape(html);
        }
        private async Task<string> _EvaluateJavaScript(string script)
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); //dale tiempo para cargar
            return
                await this.EvaluateJavaScript(script);
        }

    }
}
