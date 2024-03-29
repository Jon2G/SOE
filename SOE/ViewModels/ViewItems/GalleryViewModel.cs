﻿using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using PanCardView.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
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

        public ObservableCollection<ImageSource> Items { get; }

        private int _currentIndex;

        public GalleryViewModel()
        {
            Items = new ObservableCollection<ImageSource>();
            PanPositionChangedCommand = new Command(v =>
            {
                if (IsAutoAnimationRunning || IsUserInteractionRunning)
                {
                    return;
                }

                int index = CurrentIndex + (bool.Parse(v.ToString()) ? 1 : -1);
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

        private void Share()
        {
            ImageSource seleccionada = Items[CurrentIndex];
            if (seleccionada is FileImageSource file)
            {
                ShareFile toShare = new ShareFile(file.File);
                ShareFileRequest request = new ShareFileRequest("Compartir", toShare);
                Xamarin.Essentials.Share.RequestAsync(request).SafeFireAndForget();
            }
        }
        public void SendImages(IEnumerable<ImageSource> photos, ImageSource seleccionada)
        {
            Items.Clear();
            Items.AddRange(photos);
            CurrentIndex = Items.IndexOf(seleccionada);
        }
    }
}
