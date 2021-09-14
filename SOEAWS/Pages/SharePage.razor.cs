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
        public Guid ShareGuid { get; set; }
        [Parameter]
        public string ShareType { get; set; }
        public string UserName { get;  set; }
        public string SubjectName { get; private set; }
        public string SubjectColor { get; private set; }
        public string SubjectGroup { get; private set; }
        public string CardTitle { get; private set; }
        public string CardDateTime { get; set; }

        private readonly ILogger<AppController> _logger;
        public SharePage(ILogger<AppController> logger)
        {
            this._logger = logger;
        }

        private void GotoDownloadPage()
        {
            NavigationManagerManager.NavigateTo("DownloadPage");

        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (ShareGuid == Guid.Empty)
            {
                this.GotoDownloadPage();
                return;
            }

            switch (ShareType)
            {
                case "Todo":
                    TodoBase todo = TodoService.Find(ShareGuid, this._logger,out string nick);
                    if (todo is null)
                    {
                        this.GotoDownloadPage();
                        return;
                    }

                    UserName = nick;
                    this.SubjectColor = todo.Subject.ColorDark;
                    this.SubjectName = todo.Subject.Name;
                    this.CardTitle = todo.Title;
                    this.CardDateTime = $"{todo.Date:dd-MM-yyyy} {todo.Time:HH-mm}";
                    break;
                case "Reminder":
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
        }

        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }
    }
}
