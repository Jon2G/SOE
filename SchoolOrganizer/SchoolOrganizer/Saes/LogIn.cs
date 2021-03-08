using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models;
using SchoolOrganizer.Models.Data;
using Xamarin.Forms;

namespace SchoolOrganizer.Saes
{
    public class LogIn
    {
        public bool IsLogedIn { get; internal set; }
        private readonly WebView Browser;
        private int AttemptCount;

        public LogIn(WebView Browser)
        {
            this.Browser = Browser;
            this.AttemptCount = 0;
            this.IsLogedIn = false;
        }
        public async Task<ImageSource> GetCaptcha()
        {
            ImageSource ImageSource = null;
            try
            {
                string base_64 = await this.Browser.EvaluateJavaScriptAsync(
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
        public async void RequestLogIn(User user, string captcha)
        {
            if (AttemptCount++ < 3)
            {
                Regex regex = new Regex("[\"\\\\]");
                string Password = regex.Replace(user.Password, "\\");
                //binding.captchaDisplayer.visibility = View.GONE
                await this.Browser.EvaluateJavaScriptAsync(
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_UserName\").value = \"{user.Boleta}\";" +
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_Password\").value = \"{Password}\";" +
                    $"document.getElementById(\"ctl00_leftColumn_LoginUser_CaptchaCodeTextBox\").value = \"{captcha}\";" +
                    "document.getElementById(\"ctl00_leftColumn_LoginUser_LoginButton\").click();");
                AppData.Instance.User = user;

            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Tienes varios intentos fallidos. Es posible que la aplicación no pueda comunicarse correctamente con el SAES o tus datos sean incorrectos." +
                                                           " Si continuas es posible que tu cuenta sea suspendida.", "Cuidado!", "Entiendo");
            }

        }
    }
}
