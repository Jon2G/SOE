using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using SOE.Models;

namespace SOE.ViewModels.ViewItems
{
    public class SubjectClassmatesViewModel
    {
        public ObservableCollection<Classmate> Classmates { get; set; }

        public SubjectClassmatesViewModel()
        {
            Classmates = new ObservableCollection<Classmate>();
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            for (int i = 0; i <35; i++)
            {
                Classmates.Add(new Classmate("Jonathan ", "jgarciaj1404@alumno.ipn.mx"));
            }
        }
    }
}
