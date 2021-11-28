//
//  TodoData.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation
import WidgetKit

struct ToDoData: TimelineEntry {
    public let todos:[ToDoModel]
    public let date: Date
    public let configuration: ConfigurationIntent
}

func trialToDoData() -> ToDoData{
   return ToDoData(todos: todosTrial(),date: Date(), configuration: TodoProvider.Intent.init());
}

func readData(configuration: ConfigurationIntent) -> Timeline<ToDoData> {
    var entries = [ToDoData]()
    let currentDate = Date();
    let data:ToDoData = ToDoData(todos: load("todoAppState.json",[ToDoModel]()), date: currentDate, configuration: configuration);
    entries.append(data);
    let midnight = Calendar.current.startOfDay(for: currentDate)
    let nextMidnight = Calendar.current.date(byAdding: .day, value: 1, to: midnight)!
    return Timeline(entries: entries, policy: .after(nextMidnight));
}
