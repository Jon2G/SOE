using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOE.ViewModels.Pages
{
    public class SubjectsPageViewModel : ModelBase
    {
        private List<Subject> _subjects;

        public List<Subject> Subjects
        {
            get => this._subjects;
            set
            {
                this._subjects = value;
                Raise(() => Subjects);
            }
        }

        public SubjectsPageViewModel()
        {
            this.Subjects = new List<Subject>();
            this.Init().SafeFireAndForget();
        }

        private async Task Init()
        {
            await Task.Yield();
            this.Subjects = await Subject.GetAll();
        }
    }
}
