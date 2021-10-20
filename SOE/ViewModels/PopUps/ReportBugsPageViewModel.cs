using Forms9Patch;
using Kit;
using Kit.Model;
using MimeKit;
using SOE.Data;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using SOE.Views.Pages;
using AsyncAwaitBestPractices;

namespace SOE.ViewModels.PopUps
{
    public class ReportBugsPageViewModel: ModelBase
    {
        private string body;
        private readonly ReportBugsPage PopUp;
        private Command _SendReportCommand;
        public ICommand SendReportCommand => _SendReportCommand ??= new Command(SendReport);
        public string Body
        {
            get => this.body;
            set
            {
                body = value;
                Raise(() => Body);
            }
        }
        public ReportBugsPageViewModel(ReportBugsPage PopUp)
        {
            this.PopUp = PopUp;
        }
        public async void SendReport()
        {
            string Email = Environment.GetEnvironmentVariable("CORREOSOE");
            string Pass = Environment.GetEnvironmentVariable("PASSWORDSOE");
            try
            {
                MimeMessage mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("", "soeapp.soporte@gmail.com"));
                mail.To.Add(new MailboxAddress("", "soeapp.soporte@gmail.com"));
                mail.Subject = "soeapp.soporte@gmail.com";
                BodyBuilder BodyMessage = new();
                BodyMessage.TextBody = $"{AppData.Instance.User.Name}\n" + this.Body;
                mail.Body = BodyMessage.ToMessageBody();

                MailKit.Net.Smtp.SmtpClient SmtpServer = new();
                SmtpServer.CheckCertificateRevocation = false;
                SmtpServer.Connect("smtp.gmail.com", 587,MailKit.Security.SecureSocketOptions.StartTls);
                SmtpServer.Authenticate(Email, Pass);
                SmtpServer.Send(mail);
                SmtpServer.Disconnect(true);
                PopUp.Close().SafeFireAndForget();
                await Application.Current.MainPage.DisplayAlert("Confirmación", "El reporte se a mandado exitosamente", "Ok");

            }

            catch (Exception ex)
            {
               //Application.Current.MainPage.DisplayAlert("ERROR", "No se pudo enviar su reporte", "Ok");
                Log.Logger.Error(ex, nameof(SendReport));
            }
        }
    }
}
