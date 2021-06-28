﻿using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using P42.Utils;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TaskFirstViewModel : BaseViewModel
    {
        public static TaskFirstViewModel Instance { get; private set; }
        public ObservableCollection<ByDayGroup> DayGroups { get; set; }
        public const string Done = "AND DONE=1 AND ARCHIVED=0";
        public const string Pending = "AND DONE=0 AND ARCHIVED=0";
        public const string Archived = "AND ARCHIVED=1";

        public TaskFirstViewModel()
        {
            Instance = this;
            DayGroups = new ObservableCollection<ByDayGroup>();
            Refresh(Pending).SafeFireAndForget();
        }

      

        public async Task Refresh(string condition="")
        {
            await Task.Yield();
            DayGroups.Clear();
            DayGroups.AddRange(
            AppData.Instance.LiteConnection.
                Lista<long>($"SELECT Distinct {nameof(ToDo.Date)} from {nameof(ToDo)} where {nameof(ToDo.Date)}>={DateTime.Today.Ticks} {condition} order by date")
                .Select((x) => new ByDayGroup()
                {
                    FDateTime =new DateTime(x)
                }).ToList());
            foreach (var day in DayGroups)
            {
                day.Refresh(condition);
            }
        }

    }
}