using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using System.Threading.Tasks;

namespace SOE.ViewModels.Pages
{
    public class BlogPageViewModel : ModelBase
    {
        public Subject Subject { get; set; }
        private Teacher _Teacher;
        public Teacher Teacher
        {
            get => this._Teacher;
            set
            {
                this._Teacher = value;
                this.Raise(() => this.Teacher);
            }
        }
        public BlogPageViewModel(Subject Subject)
        {
            this.Subject = Subject;
            this.Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            this.Teacher = await this.Subject.GetTeacher();
        }
    }
}
