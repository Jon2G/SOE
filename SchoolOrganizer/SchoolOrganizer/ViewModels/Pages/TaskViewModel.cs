using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FFImageLoading.Forms;
using Xamarin.Forms;
using System.Windows.Input;
using Kit.Sql.Base;
using Kit.Sql.Helpers;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using System.Text.RegularExpressions;
using System.Linq;
using Kit.Model;
using SchoolOrganizer.Models;
using Xamarin.Essentials;

namespace SchoolOrganizer.ViewModels.Pages
{

    public class TaskViewModel : ModelBase
    {

        public Command TaskCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OnDateChangedCommand { get; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand CameraImageCommand { get; set; }
        public ICommand GaleryImageCommand { get; set; }

        private Subject _selectedSubject;
        public Regex regex => new(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ObservableCollection<FileImageSource> Photos { get; }

        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();
            }
        }

        private ToDo _Tarea;

        public ToDo Tarea
        {
            get => _Tarea;
            set
            {
                _Tarea = value;
                Raise(() => Tarea);
            }
        }
        private FileImageSource _TaskImage;
        public FileImageSource TaskImage
        {
            get => _TaskImage;
            set
            {
                _TaskImage = value;
                Raise(() => TaskImage);
            }
        }
        public TaskViewModel()
        {
            Tarea = new ToDo();
            TaskCommand = new Command(TaskClicked);
            SaveCommand = new Command(Save);
            CameraImageCommand = new Command(UsarCamara);
            GaleryImageCommand = new Command(Galeria);
            OnDateChangedCommand = new Command(OnDateChanged);
            DeleteImageCommand = new Command<FileImageSource>(DeleteImage);
            this.Photos = new ObservableCollection<FileImageSource>();
        }

        private void OnDateChanged()
        {
            if (this.SelectedSubject != null && this.Tarea?.Date != null)
            {
                this.Tarea.Time =
                    TimeSpan.FromTicks(AppData.Instance.LiteConnection.Single<long>
                        ($"SELECT BEGIN FROM ClassTime WHERE IdSubject={this.SelectedSubject.Id} AND DAY={(int)this.Tarea.Date.DayOfWeek}"));
            }
        }

        private void DeleteImage(FileImageSource img)
        {
            this.Photos.Remove(img);
        }

        private async void Save(object obj)
        {
            this.Tarea.Subject = this.SelectedSubject;
            Document.Delete(this.Tarea.IdDocument);
            var files = new List<string>();
            List<DocumentPart> Contenido = new List<DocumentPart>();
            //insertar o actualizar
            //Links
            MatchCollection matches = regex.Matches(Tarea.Description);
            foreach (Match match in matches)
            {
                files.Add(match.ToString());
            }
            //
            //obtiene la primera pocision
            int last_index = 0;
            if (files.Any())
            {
                int start = Tarea.Description.IndexOf(files[0]);
                if (start > 0)
                {
                    string con = Tarea.Description.Substring(0, start);
                    Contenido.Add(new DocumentPart() { Content = con, DocType = Enums.DocType.Text });
                }
                last_index = start + files[0].Length;
                Contenido.Add(new DocumentPart() { Content = files[0], DocType = Enums.DocType.Link });
            }
            //Separa texto
            for (int i = 1; i < files.Count; i++)
            {
                string file = files[i];
                string contexto = Tarea.Description;
                int s_index = Tarea.Description.IndexOf(file);
                if (last_index > 0)
                {
                    contexto = Tarea.Description.Substring(last_index, s_index - last_index);
                    Contenido.Add(new DocumentPart() { Content = contexto, DocType = Enums.DocType.Text });
                    last_index = s_index + file.Length;
                }
                Contenido.Add(new DocumentPart() { Content = file, DocType = Enums.DocType.Link });
            }
            //
            Document doc = new Document();
            doc.Save();
            Contenido.ForEach(c => c.Save(doc));
            this.Tarea.IdDocument = doc.Id;
            AppData.Instance.LiteConnection.InsertOrReplace(this.Tarea);
            if (Shell.Current is AppShell app)
            {
                await app.MasterPage.TaskFirstPage.Model.Refresh();
            }

            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }

        private async void TaskClicked(object obj)
        {
            var pr = new SubjectPopUp();
            await pr.ShowDialog();
            this.SelectedSubject = pr.Modelo.SelectedSubject;
        }

        private async void Galeria()
        {
            var result = await Xamarin.Essentials.MediaPicker.PickPhotoAsync(new MediaPickerOptions());
            if (result != null)
            {
                if (this.TaskImage is null)
                {
                    this.TaskImage = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.TaskImage.File = result.FullPath;
                }

                Photos.Add(await TaskPageModel.SaveImage(result));
            }
            
        }

        private async void UsarCamara()
        {
            var result = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync(new MediaPickerOptions());
            if (result != null)
            {
                if (this.TaskImage is null)
                {
                    this.TaskImage = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.TaskImage.File = result.FullPath;
                }
                Photos.Add(await TaskPageModel.SaveImage(result));
            }

        }

    }
}
