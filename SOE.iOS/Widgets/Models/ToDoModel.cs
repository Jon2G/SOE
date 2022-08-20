using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using SOE.Models;
using SOE.Models.TodoModels;
using SOE.Widgets;
using System;
using SOE.Models.Scheduler;
using Kit;
namespace SOE.iOS.Widgets.Models
{
    [Serializable, Preserve(AllMembers =true)]
    public class ToDoModel 
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subject")]
        public ClassSquare Subject { get; set; }
        [JsonProperty("emoji")]
        public string Emoji { get; set; }
        [JsonProperty("day")]
        public string DayName { get; set; }
        [JsonProperty("formattedDateTime")]
        public string FormattedDatetime { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }

        public ToDoModel(ToDo toDo,int index)
        {
            Subject = new Models.ClassSquare(toDo.Subject, index, "");
            DayName = toDo.Date.DayOfWeek.GetDayName();
            FormattedDatetime = toDo.FormattedDate;
            Id = toDo.DocumentId;
            Title = toDo.Title;
            Color = ToDosWidget.GetColor(toDo);
            Emoji = ToDosWidget.GetEmoji(toDo);
        }
    }
}
