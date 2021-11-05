using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SoeWidget
{
    public partial class MainPage : ContentPage
    {
        private readonly ITestDataGenerator testDataGenerator;
        public MainPage()
        {
            InitializeComponent();
            testDataGenerator = DependencyService.Get<ITestDataGenerator>();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            testDataGenerator.GenerateAndLog();
        }

    }
}

