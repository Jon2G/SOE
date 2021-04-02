using Kit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.Saes
{
    public class SAES : WebView
    {

        public const string HomePage = "https://www.saes.esimecu.ipn.mx";
        private const string AlumnosPage = "/alumnos/default.aspx";
        private const string HorariosPage = "/alumnos/informacion_semestral/horario_alumno.aspx";
        private const string KardexPage = "/alumnos/boleta/kardex.aspx";
        private const string CitaReinscripcionPage = "/alumnos/reinscripciones/fichas_reinscripcion.aspx";
        private const string CalificacionesPage = "/alumnos/informacion_semestral/calificaciones_sem.aspx";

        private AutoResetEvent _messageReceived;
        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(5000);
        public bool IsNavigating { get; private set; }

        public SAES()
        {
            //this.LoginViewModel = new LoginViewModel();
            //this.Browser = Browser;
            //this.LogIn = new LogIn(this.Browser);
            this.Navigated += Browser_Navigated;
            this.Navigating += Browser_Navigating;
            //this.IsCheckingSession = false;
            //GoTo(HomePage);
            this._messageReceived = new AutoResetEvent(false);
        }

        private async void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            this.IsNavigating = false;
            await this.EvaluateJavaScriptAsync(@"window.onerror = function myErrorHandler(errorMsg, url, lineNumber) { alert(""Error occured: "" + errorMsg); return false;");
            //OnNavigated?.Invoke(e);
            //OnNavigated = null;
            if (e.Url is null)
            {
                await GoTo(HomePage);
                return;
            }
            Uri Uri = new Uri(e.Url);
            string path = Uri.AbsolutePath.ToLower();
            switch (path)
            {
                //if (!IsLogedIn)
                //{
                //    //AppData.Instance.User = null;
                //    LoginViewModel.CaptchaImg = await this.LogIn.GetCaptcha();
                //}
                //else
                //{
                //    AppData.Instance.User.IsLogedIn = true;
                //    GoTo(AlumnosPage);
                //}
                //break;

                //GetName();
                //break;

                //GetHorario();
                //break;

                //GetKardexInfo();
                //break;

                //GetCitasReinscripcionInfo();
                //break;

                //GetSchoolGrades();
                //break;
                case "/default.aspx":
                case "/": //HomePage
                case AlumnosPage:
                case HorariosPage:
                case KardexPage:
                case CitaReinscripcionPage:
                case CalificacionesPage:
                    this._messageReceived.Set();
                    break;
                default:
                    //await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"I dont know what to do on =>[{e.Url}]");
                    Log.Logger.Debug($"I dont know what to do on =>[{e.Url}]");
                    GoTo(HomePage);
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
            if (url != HomePage)
            {
                navigateUrl = HomePage + url;
            }
            Source = new UrlWebViewSource()
            {
                Url = navigateUrl
            };
            await Task.Run(() => this._messageReceived.WaitOne());

        }


        public async Task<ImageSource> GetCaptcha()
        {
            ImageSource ImageSource = null;
            try
            {
                await GoTo(HomePage);
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
            return
               !string.IsNullOrEmpty(await EvaluateJavaScriptAsync(
                       "document.getElementById(\"ctl00_leftColumn_LoginNameSession\").innerHTML"));
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
            await GoTo(HomePage);
            await GoTo(HomePage);

        }
        public async Task<bool> LogIn(LoginViewModel login)
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
                await Task.Run(() => this._messageReceived.WaitOne());
                if (await IsLoggedIn())
                {
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
    }
}
