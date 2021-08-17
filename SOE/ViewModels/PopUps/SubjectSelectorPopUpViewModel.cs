using Kit.Model;
using SOE.Data;
using SOE.Services;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class SubjectSelectorPopUpViewModel : ModelBase
    {
        public List<Subject> Subjects { get; set; }

        public ICommand ClosePopUpCommand { get; }

        SubjectPopUp SubjectsPopUp;
        public Subject SelectedSubject { get; private set; }
        public SubjectSelectorPopUpViewModel(SubjectPopUp sub)
        {
            this.ClosePopUpCommand = new Command<Subject>(this.ClosePopUp);
            this.Refresh();
            this.SubjectsPopUp = sub;
        }

        internal void Refresh()
        {
            this.Subjects = SubjectService.ToList();
            this.Raise(() => this.Subjects);
        }

        public async void ClosePopUp(Subject subject)
        {
            this.SelectedSubject = subject;
            await this.SubjectsPopUp.Close();
            //?
        }
    }
}
