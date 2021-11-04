﻿using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Enums;
using SOE.Models;
using SOE.Models.TodoModels;
using SOE.Services;
using SOE.ViewModels.Pages;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class PendingRemindersViewModel : ModelBase
    {

        public static PendingRemindersViewModel Instance { get; private set; }
        public bool IsCompleted { get; set; }
        private ICommand _OpenMenuCommand;
        public ICommand OpenMenuCommand => this._OpenMenuCommand ??= new Command<Reminder>(ReminderPopUp.ShowPopUp);

        private ICommand _CompleteCommand;
        public ICommand CompleteCommand => this._CompleteCommand ??= new Command<CheckBox>(Completada);
        private ObservableCollection<Reminder> _Reminders;
        public ObservableCollection<Reminder> Reminders
        {
            get => _Reminders;
            set
            {
                _Reminders = value;
                Raise(() => Reminders);
            }
        }

        public PendingRemindersViewModel()
        {
            Instance = this;
            Reminders = new ObservableCollection<Reminder>();

        }

        public void Load(PendingStatus Status = PendingStatus.Pending)
        {
            Reminders.Clear();
            Reminders.AddRange(
                AppData.Instance.LiteConnection.Table<Reminder>()
                .Where(x => x.Status == Status).ToList());
            foreach (Reminder reminder in Reminders)
            {
                if (reminder.SubjectId > 0)
                {
                    reminder.Subject = SubjectService.Get(reminder.SubjectId);
                }
            }
        }

        private async void Completada(CheckBox checkBox)
        {
            Reminder r = (Reminder)checkBox.BindingContext;
            r.Status = !r.IsComplete ? PendingStatus.Done : PendingStatus.Pending;
            AppData.Instance.LiteConnection.Update(r);
            checkBox.IsChecked = r.IsComplete;

            var frame = checkBox.FindParent<Frame>();
            if (frame != null)
            {
                await frame.TranslateTo(300, 0, 500);
                await frame.FadeTo(0, 500);
            }
            Reminders.Remove(r);
            await Task.Delay(300);
            frame.TranslateTo(0, 0, 0).SafeFireAndForget();
            frame.FadeTo(1, 0).SafeFireAndForget();


        }


    }
}
