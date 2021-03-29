using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class SubjectViewModel : BaseViewModel
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
