using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class SubjectViewModel : BaseViewModel
    {
        public List<Subject> subjects { get; }

        public SubjectViewModel()
        {
            subjects=AppData.Instance.LiteConnection.Table<Subject>()
                .GroupBy(x=>x.Group)
                .Select(g=>g.First())
                .ToList();
        }
    }
}
