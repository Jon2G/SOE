using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kit;
using Kit.Model;
using PanCardView.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class GalleryViewModel : ModelBase
    {
        public ICommand PanPositionChangedCommand { get; }

        public ICommand ShareCommand { get; set; }
        public ICommand RemoveCurrentItemCommand { get; }

        public ICommand GoToLastCommand { get; }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                Raise(() => CurrentIndex);
            }
        }

        public bool IsAutoAnimationRunning { get; set; }

        public bool IsUserInteractionRunning { get; set; }

        public ObservableCollection<FileImageSource> Items { get; }

        private int _currentIndex;
        private int _imageCount = 1078;

        public GalleryViewModel()
        {
            Items = new ObservableCollection<FileImageSource>();
            PanPositionChangedCommand = new Command(v =>
            {
                if (IsAutoAnimationRunning || IsUserInteractionRunning)
                {
                    return;
                }

                var index = CurrentIndex + (bool.Parse(v.ToString()) ? 1 : -1);
                if (index < 0 || index >= Items.Count)
                {
                    return;
                }
                CurrentIndex = index;
            });

            RemoveCurrentItemCommand = new Command(() =>
            {
                if (!Items.Any())
                {
                    return;
                }
                Items.RemoveAt(CurrentIndex.ToCyclicalIndex(Items.Count));
            });

            GoToLastCommand = new Command(() =>
            {
                CurrentIndex = Items.Count - 1;
            });
            ShareCommand = new Command(Share);
        }

        private async void Share()
        {
            FileImageSource seleccionada = Items[CurrentIndex];
            ShareFile toShare = new ShareFile(seleccionada.File);
            ShareFileRequest request = new ShareFileRequest("Compartir", toShare);
            await Xamarin.Essentials.Share.RequestAsync(request);
        }
        public void SendImages(IEnumerable<FileImageSource> photos, FileImageSource seleccionada)
        {
            Items.Clear();
            Items.AddRange(photos);
            CurrentIndex = Items.IndexOf(seleccionada);
        }
    }
}
