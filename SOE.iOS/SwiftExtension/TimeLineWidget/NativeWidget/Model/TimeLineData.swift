//
//  TimeLineData.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 16/11/21.
//

import Foundation
import WidgetKit

struct TimeLineData: TimelineEntry {
    public let days:[DayModel]
    public let date: Date
    public let configuration: ConfigurationIntent
    public let selectedDay:Int
}


extension Date {
    func dayNumberOfWeek() -> Int {
        var dayOfWeek = (Calendar.current.dateComponents([.weekday], from: self).weekday ?? 0) - 2;
        if(dayOfWeek<0){
            dayOfWeek=0;
        }
        else if(dayOfWeek>4){
                dayOfWeek=4;
        }
        return dayOfWeek;
    }
}

func trialTimeLineData() -> TimeLineData{
   return TimeLineData(days: dayTrial(), date: Date(), configuration: TimelineProvider.Intent.init(),selectedDay: 1);
}

func readData(configuration: ConfigurationIntent) -> Timeline<TimeLineData> {
    var entries = [TimeLineData]()
    let today:Date = Date();
    let currentDate = Date();
    let data:TimeLineData = TimeLineData(days: load("timelineAppState.json",[DayModel]()), date: currentDate, configuration: configuration,selectedDay:today.dayNumberOfWeek());
    entries.append(data);
    let midnight = Calendar.current.startOfDay(for: currentDate)
    let nextMidnight = Calendar.current.date(byAdding: .day, value: 1, to: midnight)!
    return Timeline(entries: entries, policy: .after(nextMidnight));
}


