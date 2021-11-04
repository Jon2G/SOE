using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using SOE.Data;
using SOE.Services;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class SubjectSelectorPopUpViewModel : ModelBase
    {
        public List<Subject> Subjects { get; set; }

        private ICommand _ClosePopUpCommand;
        public ICommand ClosePopUpCommand => this._ClosePopUpCommand ??= new AsyncCommand<Subject>(ClosePopUp);

        private readonly SubjectPopUp SubjectsPopUp;
        public Subject SelectedSubject { get; private set; }
        public SubjectSelectorPopUpViewModel(SubjectPopUp sub)
        {
            this.Refresh();
            this.SubjectsPopUp = sub;
        }

        internal void Refresh()
        {
            this.Subjects = SubjectService.ToList();
            this.Raise(() => this.Subjects);
        }

        public async Task ClosePopUp(Subject subject)
        {
            this.SelectedSubject = subject;
            await this.SubjectsPopUp.Close();
            //?
        }
    }
}
