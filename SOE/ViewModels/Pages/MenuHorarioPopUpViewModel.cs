using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.Models.Scheduler;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class MenuHorarioPopUpViewModel : ModelBase
    {
        private readonly MenuHorarioPopUp PopUp;
        public ClassSquare ClassSquare { get; }
        public Teacher Teacher { get; set; }
        public MenuHorarioPopUpViewModel(MenuHorarioPopUp PopUp, ClassSquare square)
        {
            this.PopUp = PopUp;
            this.ClassSquare = square;
            LoadTeacher().SafeFireAndForget();
        }
        private async Task LoadTeacher()
        {
            await Task.Yield();
            this.Teacher = ClassSquare.Subject.Teacher;
            Raise(() => Teacher);
        }
        private ICommand _TapedCommand;
        public ICommand TapedCommand => _TapedCommand ??= new Command<string>(Tapped);
        public string Action { get; private set; }
        private void Tapped(string Action)
        {
            this.Action = Action;
            PopUp.Close().SafeFireAndForget();
        }
    }
}
