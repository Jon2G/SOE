using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using SOE.Views.Pages;
using SOE.Views.ViewItems.ScheduleView;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SOE.ViewModels.ViewItems
{
    public class NoSubjectsViewModel : ModelBase
    {
        public ICommand AddSubjectCommand { get; set; }
        public NoSubjectsViewModel()
        {
            AddSubjectCommand = new AsyncCommand(AddSubject);
        }
        private async Task AddSubject()
        {
            AddSubjectPage? addPage = new AddSubjectPage();
            await App.Current.MainPage.Navigation.PushAsync(addPage);
            await addPage.WaitUntilClose();
            Tools.Container.Get<ScheduleViewMain>()?.Init();
        }
    }
}
