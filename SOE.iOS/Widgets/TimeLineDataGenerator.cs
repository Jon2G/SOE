using Foundation;
using SOE.iOS.Widgets.Models;
using SOE.Models.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SOE.iOS.Widgets
{
    [Preserve]
    public class TimeLineDataGenerator:DataGenerator
    {
        protected override string FileName => "timelineAppState.json";

        protected override IEnumerable GenerateData()
        {
           List<Day> week = Day.Week();
            List<DayModel> days = new List<DayModel>();
            foreach(Day day in week)
            {
                List<Models.ClassSquare> classes = new List<Models.ClassSquare>();
                List<SOE.Models.Scheduler.ClassSquare> list = day.GetTimeLine();
                for (int i = 0; i < list.Count; i++)
                {
                    SOE.Models.Scheduler.ClassSquare classSquare = list[i];
                    classes.Add(new Models.ClassSquare()
                    {
                        Color=classSquare.Subject.Color,
                        Group=classSquare.Subject.GroupId,
                        FormattedTime=classSquare.FormattedTime,
                        Id=classSquare.Subject.Id,
                        Index=i,
                        SubjectName=classSquare.Subject.Name
                    });
                }
                days.Add(new DayModel() { Id = (int)day.DayOfWeek, Name = day.Name, Classes = classes.ToArray() });
            }
            return days.ToArray();
        }
    }
}

