using SOE.Data;
using SOE.Enums;
using SOE.iOS.Widgets.Models;
using SOE.Models.Scheduler;
using SOE.Models.TodoModels;
using SOE.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kit;
using Foundation;

namespace SOE.iOS.Widgets
{
    [Preserve]
    public class TodoDataGenerator:DataGenerator
    {
        protected override string FileName => "todoAppState.json";

        protected override IEnumerable GenerateData()
        {
            List<ToDoModel> todos = new List<ToDoModel>();
            List<ToDo> _todos =  ToDosWidget.GetTasks();    
            for (int i = 0; i < _todos.Count; i++)
            {
                ToDo toDo = _todos[i];
                todos.Add(new ToDoModel() {
                    Subject=new Models.ClassSquare(toDo.Subject,i,""),
                    DayName=toDo.Date.DayOfWeek.GetDayName(),
                    FormattedDatetime=toDo.FormattedDate,
                    Id=i,
                    Title=toDo.Title,
                    Color=ToDosWidget.GetColor(toDo),
                    Emoji=ToDosWidget.GetEmoji(toDo)
                });
            }
            return todos;
        }
    }
}
