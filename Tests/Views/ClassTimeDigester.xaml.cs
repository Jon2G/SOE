using SOE.API;
using SOEWeb.Shared;
using System.Windows;

namespace Tests.Views
{
    /// <summary>
    /// Interaction logic for ClassTimeDigester.xaml
    /// </summary>
    public partial class ClassTimeDigester : Window
    {
        public ClassTimeDigester()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Response response = await APIService.PostClassTime(System.Text.Encoding.UTF8.GetBytes(this.TxtHtml.Text), "2015130425");
            
        }
    }
}
