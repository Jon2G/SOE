using AsyncAwaitBestPractices;
using System.Windows;

namespace ColorTest.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SubjectColorsView : Window
    {
        public SubjectColorsView()
        {
            InitializeComponent();
            this.Model.Init().SafeFireAndForget();

        }
        
    }
}
