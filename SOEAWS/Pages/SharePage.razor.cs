using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SOEAWS.Controllers;
using SOEAWS.Services;
using SOEWeb.Shared;

namespace SOEAWS.Pages
{
    public partial class SharePage : IStateHasChanged
    {
        [Inject]
        public NavigationManager NavigationManagerManager { get; set; }
        [Parameter]
        public string ShareId { get; set; }
        public Guid ShareGuid { get; set; }
        [Parameter]
        public string ShareType { get; set; }
        public string UserName { get;  set; }
        public string SubjectName { get; private set; }
        public string SubjectColor { get; private set; }

        public string CardTitle { get; private set; }
        public string CardDateTime { get; set; }
        [Inject]
        public ILogger<AppController> _logger { get; set; }

        private const string FireBaseUrl = "https://soeapp.page.link/H3Ed";
        public SharePage()
        {

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        private void GotoDownloadPage()
        {
            try
            {
                NavigationManagerManager.NavigateTo($"{FireBaseUrl}?Verb=share?Type={ShareType}?Id={ShareGuid}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GotoDownloadPage");
            }
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ShareGuid=Guid.Parse(ShareId);
            if (ShareGuid == Guid.Empty)
            {
                this.GotoDownloadPage();
                return;
            }

            switch (ShareType)
            {
                case "todo":
                    TodoBase todo = TodoService.Find(ShareGuid, this._logger,out string nick);
                    if (todo is null)
                    {
                        this.GotoDownloadPage();
                        return;
                    }
                    UserName = nick;
                    todo.Subject = SubjectService.GetById(todo.Subject.Id);
                    this.SubjectColor = todo.Subject.ColorDark;
                    this.SubjectName = todo.Subject.Name;
                    this.CardTitle = todo.Title;
                    this.CardDateTime = $"{todo.Date:dd-MM-yyyy} {todo.Time.Hours:D2}:{todo.Time.Minutes:D2}";
                    break;
                case "reminder":
                    ReminderBase reminder = ReminderService.Find(ShareGuid, this._logger, out string nickr);
                    if (reminder is null)
                    {
                        this.GotoDownloadPage();
                        return;
                    }
                    if (reminder.Subject is not null)
                    {
                        this.SubjectName = reminder.Subject.Name;
                        this.SubjectColor = reminder.Subject.ColorDark;
                    }
                    else
                    {
                        this.SubjectName = "Recordatorio";
                        this.SubjectColor = "#0277bd";
                    }
                    UserName = nickr;
                    this.CardTitle = reminder.Title;
                    this.CardDateTime = $"{reminder.Date:dd-MM-yyyy} {reminder.Time:HH-mm}";
                    break;
                default:
                    this.GotoDownloadPage();
                    return;
            }
            this.InvokeStateHasChanged();
        }

        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }
    }
}
