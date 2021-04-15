using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SchoolOrganizer.Models.TaskFirst;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class TaskDetailsViewModel
    {
        public ToDo ToDo { get; set; }
        public  ObservableCollection<ImageSource> PhotosCollection{get; set; }

        public TaskDetailsViewModel(ToDo ToDo)
        {
            this.ToDo = ToDo;
            PhotosCollection = new ObservableCollection<ImageSource>();
            //no hay fotos :c
        }

    }
}
