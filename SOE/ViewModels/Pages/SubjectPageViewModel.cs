using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.Views.ViewItems;

namespace SOE.ViewModels.Pages
{
    public class SubjectPageViewModel : ModelBase
    {
        public Subject Subject { get; set; }

        public ObservableCollection<IconView> Views { get; private set; }

        public SubjectPageViewModel(Subject Subject)
        {
            this.Subject = Subject;
            this.Views = new ObservableCollection<IconView>();
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            var notesview = new SubjectNotesView(this.Subject);
            Views.Add(notesview);
            Views.Add(new SubjectClassmatesView(this.Subject));
            await notesview.Init();
        }
    }
}
