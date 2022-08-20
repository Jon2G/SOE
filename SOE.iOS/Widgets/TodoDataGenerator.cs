using SOE.Data;
using SOE.Enums;
using SOE.iOS.Widgets.Models;

using SOE.Models.TodoModels;
using SOE.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using System.Threading.Tasks;

namespace SOE.iOS.Widgets
{
    [Preserve]
    public class TodoDataGenerator:DataGenerator
    {
        protected override string FileName => "todoAppState.json";

        protected override async Task<IEnumerable> GenerateData()
        {
            List<ToDoModel> todos = new List<ToDoModel>();
            List<ToDo> _todos = await ToDosWidget.GetTasks();    
            for (int i = 0; i < _todos.Count; i++)
            {
                ToDo toDo = _todos[i];
                if(toDo is null)
                {
                    continue;
                }
                await toDo.GetSubject();
                if(toDo.Subject is null) { continue; }
                await toDo.Subject.GetGroup();
                if (toDo.Subject.Group is null)
                    continue;
                todos.Add(new ToDoModel(toDo,i));
            }
            return todos;
        }
    }
}
