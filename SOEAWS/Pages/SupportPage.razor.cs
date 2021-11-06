using AsyncAwaitBestPractices;
using Kit;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SOEAWS.Controllers;
using SOEAWS.Services;
using SOEWeb.Shared;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using MimeKit;
using SOEWeb.Shared.Secrets;
using System.IO;
using System.Text;

namespace SOEAWS.Pages
{
    public partial class SupportPage : IStateHasChanged
    {
        public string Report { get; set; }
        public bool Thankyou { get; set; }
        public SupportPage()
        {
            Thankyou = false;
            Report = @"**Describe el problema**
Una clara y concisa descripción del problema.

Pasos para reproducir el problema:
1. Vaya a '...'
2. Haga clic en '....'
3. Desplácese hacia abajo hasta '....'

**Comportamiento esperado**
Una descripción clara y concisa de lo que esperaba que sucediera.

** Capturas de pantalla **
Si corresponde, agregue capturas de pantalla para ayudar a explicar su problema.

** Smartphone (complete la siguiente información): **
  - Dispositivo: [p. Ej. iphone 6]
  - SO: [p. Ej. iOS8.1]
  - Versión [p. Ej. 22]

** Contexto adicional **
Agregue aquí cualquier otro contexto sobre el problema.";
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private async void Send()
        {
            try
            {

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("", DotNetEnviroment.CORREOSOE));
                message.To.Add(new MailboxAddress("", DotNetEnviroment.CORREOSOE));
                message.Subject = $"BUG REPORT - [WEB PAGE] - {DateTime.Now}";


                Kit.Daemon.Devices.Device device = Kit.Daemon.Devices.Device.Current;
                StringBuilder sb = new StringBuilder(Report);
                Multipart multipart = new Multipart("mixed");
                multipart.Add(new TextPart("plain") { Text = sb.ToString() });
                FileInfo log = new FileInfo(Log.Current.LoggerPath);
                if (log.Exists)
                {
                    multipart.Add(new MimePart("text", "plain")
                    {
                        Content = new MimeContent(File.OpenRead(log.FullName)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(log.FullName)
                    });
                }
                FileInfo critical_log = new FileInfo(Log.Current.CriticalLoggerPath);
                if (log.Exists)
                {
                    multipart.Add(new MimePart("text", "plain")
                    {
                        Content = new MimeContent(File.OpenRead(critical_log.FullName)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(critical_log.FullName)
                    });
                }

                message.Body = multipart;



                MailKit.Net.Smtp.SmtpClient SmtpServer = new();
                SmtpServer.CheckCertificateRevocation = false;
                SmtpServer.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                SmtpServer.Authenticate(DotNetEnviroment.CORREOSOE, DotNetEnviroment.PASSWORDSOE);
                SmtpServer.Send(message);
                SmtpServer.Disconnect(true);

            }

            catch (Exception ex)
            {
                Log.Logger.Error(ex, nameof(Report));
            }


            Thankyou = true;
        }



        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }
    }
}
