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
using SOEWeb.Shared.Secrets;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace SOE.ViewModels.PopUps
{
    public class ReportBugsPageViewModel : ModelBase
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
            try
            {

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("", DotNetEnviroment.CORREOSOE));
                message.To.Add(new MailboxAddress("", DotNetEnviroment.CORREOSOE));
                message.Subject = $"BUG REPORT - [{AppData.Instance.User.Boleta}] - {DateTime.Now}";


                Kit.Daemon.Devices.Device device = Kit.Daemon.Devices.Device.Current;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DEVICE DETAILS")
                  .Append("RuntimePlatform: ").AppendLine(Device.RuntimePlatform)
                  .Append("DeviceId: ").AppendLine(device.DeviceId)
                  .Append("Brand: ").AppendLine(device.GetDeviceBrand())
                  .Append("Model: ").AppendLine(device.GetDeviceModel())
                  .Append("DevicePlatform: ").AppendLine(device.GetDeviceName())
                  .Append("DeviceId: ").AppendLine(device.GetDevicePlatform())
                  .Append("AppBuild: ").AppendLine(device.IDeviceInfo.AppBuild)
                  .Append("AppVersion: ").AppendLine(device.IDeviceInfo.AppVersion)
                  .Append("Manufacturer: ").AppendLine(device.IDeviceInfo.Manufacturer)
                  .Append("VersionNumber: ").AppendLine(device.IDeviceInfo.VersionNumber.ToString())
                  .Append("User name: ").AppendLine(AppData.Instance.User.Name)
                  .Append("User id: ").AppendLine(AppData.Instance.User.Boleta)
                  .Append("User message: ").AppendLine(this.Body);
               
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
                PopUp.Close().SafeFireAndForget();
                await Application.Current.MainPage.DisplayAlert("Confirmación", "El reporte se a mandado exitosamente", "Ok");

            }

            catch (Exception ex)
            {
                Log.Logger.Error(ex, nameof(SendReport));
            }
        }
    }
}
