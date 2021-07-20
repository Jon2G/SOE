using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SOEWeb.Shared;
using Kit.Model;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class SubjectViewModel : ModelBase
    {
        public List<Subject> Subjects { get; set; }

        public ICommand ClosePopUpCommand { get; }

        SubjectPopUp SubjectsPopUp;
        public Subject SelectedSubject { get; private set; }
        public SubjectViewModel(SubjectPopUp sub)
        {
            ClosePopUpCommand = new Command<Subject>(ClosePopUp);
            Refresh();
            SubjectsPopUp = sub;
        }

        internal void Refresh()
        {
            this.Subjects = AppData.Instance.LiteConnection.Table<Subject>()
                .GroupBy(x => x.Group)
                .Select(g => g.First())
                .ToList();
            Raise(() => Subjects);
        }

        public async void ClosePopUp(Subject subject)
        {
            SelectedSubject = subject;
            await SubjectsPopUp.Close();
            //?
        }
    }
}
