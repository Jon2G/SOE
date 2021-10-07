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
using Kit.Services.Web;
using SOE.Data.Images;
using SOE.Models;
using SOE.Models.TaskFirst;
using SOEWeb.Shared.Interfaces;
using SOEWeb.Shared.Processors;
using Xamarin.CommunityToolkit.Extensions;

namespace SOE.Saes
{
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
            this.Navigated += Browser_Navigated;
            this._navigationQueue = new Queue<NavigationRequest>();

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
            var request = new NavigationRequest(navigateUrl);
            this._navigationQueue.Enqueue(request);
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

            await Task.Delay(500); //Esperar el html
        }
        private async void NavigateAsync()
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

        public async Task<ImageSource> GetCaptcha()
        {
            ImageSource imageSource = null;
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
                string base64 = await EvaluateJavaScript(sb.ToString());
                if (string.IsNullOrEmpty(base64))
                {
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
        private async Task<string> EvaluateJavaScript(string script)
        {
            await Task.Delay(50);
            return await EvaluateJavaScriptAsync(script);
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
            catch (Exception ex) { Log.Logger.Error(ex, "IsLoggedIn"); return false; }
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
                string password = regex.Replace(AppData.Instance.User.Password, "\\");
                //binding.captchaDisplayer.visibility = View.GONE
                await this.EvaluateJavaScript(
                    $"document.getElementById('ctl00_leftColumn_LoginUser_UserName').value = '{AppData.Instance.User.Boleta}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_Password').value = '{AppData.Instance.User.Password}';" +
                    $"document.getElementById('ctl00_leftColumn_LoginUser_CaptchaCodeTextBox').value ='{captcha}';"
                    );
                await Task.Delay(100);
                await this.EvaluateJavaScript("document.getElementById('ctl00_leftColumn_LoginUser_LoginButton').click();");
                await Task.Delay(TimeSpan.FromSeconds(2));
                await GoTo(AppData.Instance.User.School.HomePage);
                if (await IsLoggedIn())
                {
                    if (shouldGetUserData)
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

        public async Task GetUserData(Models.Data.User user, bool? isWebServiceOnline = null)
        {
            await Task.Yield();
            bool isOnline = isWebServiceOnline is bool online ? online : await APIService.IsOnline();
            AppData.Instance.User.IsOffline = !isOnline;
            AppData.Instance.ClearData(
                typeof(Subject), typeof(ClassTime), typeof(Document),
                typeof(DocumentPart), typeof(Archive), typeof(Grade),
                typeof(Keeper), typeof(Teacher), typeof(ToDo), typeof(Reminder));
            Keeper.ClearAllFiles();

            AppData.Instance.User = user;
            await GetName();
            bool hasSubjects = await GetSubjects(isOnline);
            await GetKardexInfo(isOnline);
            await GetCitasReinscripcionInfo();
            if (hasSubjects)
            {
                await GetGrades(isOnline);
                AppData.Instance.User.Semester = CalculateSemester();

            }
            else
            {
                AppData.Instance.User.Semester = "Ninguno";
                Acr.UserDialogs.UserDialogs.Instance.AlertAsync(
                        "Actualmente no estas inscrito en ninguna materia.",
                        "Sin inscripción", "Ok").SafeFireAndForget();
            }
            AppData.Instance.User.HasSubjects = hasSubjects;
            AppData.Instance.LiteConnection.InsertOrReplace(AppData.Instance.User);
        }

        private string CalculateSemester()
        {
            string semester = "?";
            try
            {
                semester =
                    string.Join(",", AppData.Instance.LiteConnection.Lista<string>(@"SELECT ""Group"" FROM ""Subject""")
                        .Select(x => x.FirstOrDefault()).Distinct());
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
        private async Task<bool> GetSubjects(bool isWebServiceOnline)
        {
            await Task.Yield();
            await GoTo(HorariosPage);
            //if (DataInfo.HasTimeTable())
            //{
            //    return;
            //}
            string horarioHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_GV_Horario').outerHTML");
            Unescape(ref horarioHtml);
            if (string.IsNullOrEmpty(horarioHtml))
            {
                return false;
            }
            return await DigestSubjects(horarioHtml, isWebServiceOnline);
        }

        private async Task<bool> DigestSubjects(string horarioHtml, bool isWebServiceOnline)
        {
            await Task.Yield();
            Response response = Response.NotExecuted;
            if (isWebServiceOnline)
            {
                response = await APIService.PostClassTime(System.Text.Encoding.UTF8.GetBytes(horarioHtml), AppData.Instance.User.Boleta);
                if (response.ResponseResult != APIResponseResult.OK) return await DigestSubjects(horarioHtml, false);
            }
            else
            {
                response = ClassTimeDigester.Digest(horarioHtml, AppData.Instance.User.Id, null, false).ToResponse();
            }
            if (response.ResponseResult == APIResponseResult.OK)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response.Message);
                XmlNodeList nodes = xmlDoc.SelectNodes("//Teachers/Teacher");
                foreach (XmlNode tNode in nodes)
                {
                    TeacherService.Save(tNode.ConvertNode<Teacher>());
                }
                nodes = xmlDoc.SelectNodes("//Subjects/Subject");
                foreach (XmlNode tNode in nodes)
                {
                    SubjectService.Save(tNode.ConvertNode<Subject>());
                }
                if (nodes.Count <= 0)
                {
                    return false;
                }
                nodes = xmlDoc.SelectNodes("//ClassTimes/ClassTime");
                foreach (XmlNode tNode in nodes)
                {
                    var classTime = tNode.ConvertNode<ClassTime>();
                    ClassTimeService.Save(classTime);
                }
                TimeLineWidget.UpdateWidget();
                return true;
            }
            return false;
        }
        private async Task GetKardexInfo(bool isWebServiceOnline)
        {
            await GoTo(KardexPage);
            string carrera = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_Lbl_Carrera').innerHTML");
            AppData.Instance.User.Career = carrera;
            if (string.IsNullOrEmpty(carrera))
            {
                return;
            }
            if (isWebServiceOnline)
            {
                Response response = await APIService.PostCareer(carrera, AppData.Instance.User.Boleta);
                if (response.ResponseResult == APIResponseResult.OK)
                {
                    AppData.Instance.LiteConnection.DeleteAll<Career>(false);
                    AppData.Instance.LiteConnection.Insert(
                        new Career() { Name = carrera, Id = Convert.ToInt32(response.Message) }, false);
                }
            }
            else
            {
                if (AppData.Instance.LiteConnection.Table<Career>().Any())
                {
                    AppData.Instance.LiteConnection.EXEC($"UPDATE Career SET Name='{carrera}'");
                }
                else
                {
                    AppData.Instance.LiteConnection.Insert(
                        new Career() { Name = carrera, Id = OfflineConstants.IdBase, IsOffline = true }, false);
                }
            }
        }
        private async Task GetCitasReinscripcionInfo()
        {
            await GoTo(CitaReinscripcionPage);
            double creditosTotales = 0;
            double creditosAlumno = 0;

            string creditosCarreraHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_CREDITOSCARRERA').outerHTML");
            string alumnoHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_alumno').outerHTML");
            string citaReinscripcionHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_grvEstatus_alumno').outerHTML");

            Unescape(ref alumnoHtml);
            if (!string.IsNullOrEmpty(alumnoHtml))
            {
                var table = HtmlToTable(alumnoHtml);
                creditosAlumno = Convert.ToDouble(table[0][1]);
            }

            Unescape(ref creditosCarreraHtml);
            if (!string.IsNullOrEmpty(creditosCarreraHtml))
            {
                List<List<string>> table = HtmlToTable(creditosCarreraHtml);
                creditosTotales = Convert.ToDouble(table[0][1]);
            }

            double result = (creditosAlumno / creditosTotales) * 100;
            result = Math.Round(result, 2);

            Credits credits = new Credits
            {
                Id = 1,
                CurrentCredits = creditosAlumno,
                TotalCredits = creditosTotales,
                Percentage = result,
                UserId = AppData.Instance.User.Boleta
            };
            AppData.Instance.LiteConnection.InsertOrReplace(credits);

            //Save fecha de reinscripción
            Unescape(ref citaReinscripcionHtml);
            if (!string.IsNullOrEmpty(citaReinscripcionHtml))
            {
                List<List<string>> table = HtmlToTable(citaReinscripcionHtml);
                string _date = table[0][3];
                InscriptionDate date = new InscriptionDate(_date);
                date.Save();
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
        public async Task<bool> GetGrades(bool isWebServiceOnline)
        {
            await Task.Yield();
            await GoTo(CalificacionesPage);
            string gradesHtml = await this.EvaluateJavaScript("document.getElementById('ctl00_mainCopy_GV_Calif').outerHTML");
            Unescape(ref gradesHtml);
            if (string.IsNullOrEmpty(gradesHtml))
            {
                return false;
            }
            return await DigestGrades(gradesHtml, isWebServiceOnline);


        }
        private async Task<bool> DigestGrades(string gradesHtml, bool isWebServiceOnline)
        {
            await Task.Yield();
            Response response = Response.NotExecuted;
            if (isWebServiceOnline)
            {
                response = await APIService.PostGrades(System.Text.Encoding.UTF8.GetBytes(gradesHtml));
                if (response.ResponseResult != APIResponseResult.OK) return await DigestGrades(gradesHtml, false);
            }
            else
            {
                response = GradesDigester.Digest(gradesHtml, AppData.Instance.User.Boleta, null, false).ToResponse();
            }

            if (response.ResponseResult == APIResponseResult.OK)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response.Message);
                XmlNodeList nodes = xmlDoc.SelectNodes("//Grades/Grade");
                foreach (XmlNode tNode in nodes)
                {
                    GradeService.Save(tNode.ConvertNode<Grade>());
                }
                return true;
            }
            return false;
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
