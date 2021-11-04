using AsyncAwaitBestPractices;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComingSoon 
    {
        public ComingSoon()
        {
            InitializeComponent();
        }

        public static void Alert()
        {
            ComingSoon coming = new ComingSoon();
            coming.Show().SafeFireAndForget();
        }
    }
}