using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.Views.PopUps;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class SubjectSelectorPopUpViewModel : ModelBase
    {
        public List<Subject> Subjects { get; set; }

        private ICommand _ClosePopUpCommand;
        public ICommand ClosePopUpCommand => this._ClosePopUpCommand ??= new Command<Subject>(ClosePopUp);

        private readonly SubjectPopUp SubjectsPopUp;
        public Subject SelectedSubject { get; private set; }
        public SubjectSelectorPopUpViewModel(SubjectPopUp sub)
        {
            this.Refresh().SafeFireAndForget();
            this.SubjectsPopUp = sub;
        }

        internal async Task Refresh()
        {
            await Task.Yield();
            this.Subjects = await Subject.GetAll();
            this.Raise(() => this.Subjects);
        }

        public void ClosePopUp(Subject subject)
        {
            this.SelectedSubject = subject;
            this.SubjectsPopUp.Close().SafeFireAndForget();
            //?
        }
    }
}
