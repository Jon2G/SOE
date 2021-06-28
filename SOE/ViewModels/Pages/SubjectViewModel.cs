using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using APIModels;
using Kit.Model;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class SubjectViewModel : ModelBase
    {
        public List<Subject> subjects { get; }

        public ICommand ClosePopUpCommand { get; }

        SubjectPopUp subjectPop;
       public Subject SelectedSubject { get; private set; }
        public SubjectViewModel(SubjectPopUp sub)
        {
            ClosePopUpCommand = new Command<Subject>(ClosePopUp);
            subjects =AppData.Instance.LiteConnection.Table<Subject>()
                .GroupBy(x=>x.Group)
                .Select(g=>g.First())
                .ToList();
            subjectPop = sub;
        }

        public async void ClosePopUp(Subject subject)
        {
            SelectedSubject = subject;
           await subjectPop.Close();
            //?
        }
    }
}
