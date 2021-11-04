using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Services;

namespace SOE.ViewModels.ViewItems
{
   public class SubjectNotesViewModel:ModelBase
    {
        public Subject Subject { get; set; }
        private Teacher _Teacher;
        public Teacher Teacher
        {
            get => _Teacher;
            set
            {
                _Teacher = value;
                Raise(() => Teacher);
            }
        }
        public SubjectNotesViewModel(Subject Subject)
        {
            this.Subject = Subject;
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            this.Teacher = TeacherService.Get(this.Subject.IdTeacher);

        }
    }
}
