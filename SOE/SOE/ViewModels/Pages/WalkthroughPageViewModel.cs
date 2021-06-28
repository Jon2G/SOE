using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Kit.Extensions;
using Kit.Model;
using SchoolOrganizer.Saes;
using SchoolOrganizer.Views.Pages;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class WalkthroughPageViewModel : ModelBase
    {
        private ICommand _ContinueCommand;
        public ICommand ContinueCommand => _ContinueCommand ??= new Command(Continue);

        private void Continue() => App.Current.MainPage = new SchoolSelector(SchoolLevel.University);
    }
}
