using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using Kit;
using SOE.Models;

namespace SOE.ViewModels.ViewItems
{
    public class SubjectClassmatesViewModel
    {
        public ObservableCollection<Classmate> Classmates { get; set; }
        public Subject Subject { get; set; }

        public SubjectClassmatesViewModel(Subject subject)
        {
            this.Subject = subject;
            Classmates = new ObservableCollection<Classmate>();
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            var class_mates = await API.APIService.GetClassmates(this.Subject.Group);
            this.Classmates.AddRange(class_mates);
        }
    }
}
