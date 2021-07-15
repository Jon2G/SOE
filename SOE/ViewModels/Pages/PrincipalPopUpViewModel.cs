using AsyncAwaitBestPractices;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
   public class PrincipalPopUpViewModel
    {
        private readonly PrincipalPopUp PopUp;
        public PrincipalPopUpViewModel(PrincipalPopUp PopUp)
        {
            this.PopUp = PopUp;
        }
        private ICommand _AddTaskCommand;
        public ICommand AddTaskCommand => _AddTaskCommand ??= new Command(AddTask);

        public ICommand _AddReminderCommand;
        public ICommand AddReminderCommand => _AddReminderCommand ??= new Command(AddReminder);
        private void AddTask()
        {
            App.Current.MainPage.Navigation.PushAsync(new TaskPage(), true).SafeFireAndForget();
            PopUp.Close().SafeFireAndForget();
        }
        private async void AddReminder()
        {
            var pr = new ReminderPage();
            PopUp.Close().SafeFireAndForget();
            await pr.ShowDialog();
           
        }
    }

}
