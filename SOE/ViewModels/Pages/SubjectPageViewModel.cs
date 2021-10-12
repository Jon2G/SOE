using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.API;
using SOE.Data;
using SOE.Models;
using SOE.Views.ViewItems;
using System.Linq;
using System.Windows.Input;
using System;

namespace SOE.ViewModels.Pages
{
    public class SubjectPageViewModel : ModelBase
    {
        public Subject Subject { get; set; }
        private int _SelectedIndex;

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (value >= 0 && this._SelectedIndex != value && (Views?.Any() ?? false))
                {
                    _SelectedIndex = value;
                    SelectedView = Views[value];
                    Raise(() => SelectedIndex);
                    SelectionChanged(value);
                }
            }
        }

        private IconView _SelectedView;

        public IconView SelectedView
        {
            get => _SelectedView;
            set
            {
                if (_SelectedView != value)
                {
                    _SelectedView = value;
                    SelectedIndex = Views.IndexOf(value);
                    Raise(() => SelectedView);
                }
            }
        }
        public ObservableCollection<IconView> Views { get; private set; }
        private ICommand _NotesViewCommand;
        public ICommand NotesViewCommand
            => _NotesViewCommand ??= new Xamarin.Forms.Command(NotesView);

        private ICommand _ClassmatesViewCommand;
        public ICommand ClassmatesViewCommand
            => _ClassmatesViewCommand ??= new Xamarin.Forms.Command(ClassmatesView);


        private void ClassmatesView()
        {
            SelectedIndex = 1;
        }
        private void NotesView()
        {
            SelectedIndex = 0;
        }

        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
        }
        public SubjectPageViewModel(Subject Subject)
        {
            this.Subject = Subject;
            this.Views = new ObservableCollection<IconView>();
            if (Subject.IsOffline)
            {
                SyncSubject().SafeFireAndForget(); ;
                return;
            }
            Load().SafeFireAndForget();
        }

        private async Task SyncSubject()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Actualizando información..."))
            {
                if (!await Subject.Sync(AppData.Instance, new SyncService()))
                {
                    await this.Load(false);
                }
            }
            this.Load().SafeFireAndForget();
        }
        private async Task Load(bool Online = true)
        {
            await Task.Yield();
            var notesview = new SubjectNotesView(this.Subject, Online);
            Views.Add(notesview);
            Views.Add(new SubjectClassmatesView(this.Subject, Online));
            await notesview.Init();
        }
    }
}
