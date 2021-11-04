using Forms9Patch;
using Plugin.XamarinFormsSaveOpenPDFPackage;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage(string Html)
        {
            InitializeComponent();
            this.WebView.Source = new HtmlWebViewSource()
            {
                Html = Html
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (Forms9Patch.ToPdfService.IsAvailable)
            {
                if (await this.WebView.ToPdfAsync("output.pdf",
                    Forms9Patch.PageSize.IsoA4,
                    PageMargin.CreateInMillimeters(0)) is ToFileResult pdfResult)
                {
                    if (pdfResult.IsError)
                        using (Toast.Create("PDF Failure", pdfResult.Result)) { }
                    else
                    {
                        using (FileStream fileStream = new FileStream(pdfResult.Result, FileMode.Open))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                fileStream.Position = 0;
                                await fileStream.CopyToAsync(memoryStream); 
                                await CrossXamarinFormsSaveOpenPDFPackage.Current.SaveAndView(
                                    $"Horario_Escolar.pdf", "application/pdf", memoryStream,
                                    PDFOpenContext.InApp);

                            }
                        }

                        //await Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfResult.Result))
                        //{
                        //    Title = "Tu horario"
                        //});
                    }
                }
            }
            else
                using (Toast.Create(null, "PDF Export is not available on this device")) { }

            await Navigation.PopToRootAsync(true);
        }

        //string path = Path.Combine(Kit.Tools.Instance.LibraryPath, $"{Guid.NewGuid():N}.pdf");
        //var doc = PdfSharp.Xamarin.Forms.PDFManager.GeneratePDFFromView(this.WebView, PageOrientation.Landscape, PageSize.A4,
        //    true);
        //doc.Save(path);

    }

}
