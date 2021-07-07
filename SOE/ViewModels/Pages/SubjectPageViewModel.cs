using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.Services;
using SOE.Views.ViewItems;

namespace SOE.ViewModels.Pages
{
    public class SubjectPageViewModel : ModelBase
    {
        public Subject Subject { get; set; }

        public ObservableCollection<IconView> Views {get; private set; }

        public SubjectPageViewModel(Subject Subject)
        {
            this.Subject = Subject;
            this.Views = new ObservableCollection<IconView>();
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            Views.Add(new SubjectNotesView(this.Subject));
            Views.Add(new SubjectClassmatesView());

        }
    }
}
