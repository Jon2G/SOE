using AsyncAwaitBestPractices;
using System.Windows;

namespace Tests.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SubjectColorsView : Window
    {
        public SubjectColorsView()
        {
            this.InitializeComponent();
            this.Model.Init().SafeFireAndForget();
        }
        
    }
}
