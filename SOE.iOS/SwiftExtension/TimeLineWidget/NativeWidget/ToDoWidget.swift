//
//  ToDoWidget.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation
import SwiftUI
import WidgetKit

struct TodoProvider: IntentTimelineProvider {
    public func placeholder(in context: Context) -> ToDoData {
        return trialToDoData();
    }
    
    public func getSnapshot(for configuration: ConfigurationIntent, in context: Context, completion: @escaping (ToDoData) -> Void) {
        let entry = ToDoData(todos: todosTrial(), date: Date(), configuration: configuration)
        completion(entry)
    }
    
    public func getTimeline(for configuration: ConfigurationIntent, in context: Context, completion: @escaping (Timeline<ToDoData>) -> Void) {
        completion(readData(configuration: configuration));
    }
}


struct ToDoContentView_Previews: PreviewProvider {
    static var previews: some View {
        ToDoList(todoData: trialToDoData())
            .previewContext(WidgetPreviewContext(family: .systemMedium))
            
    }
}



//@main
struct ToDoWidget: Widget {
    private let kind: String = "ToDoWidget"

    public var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: ConfigurationIntent.self, provider: TodoProvider()) {
            entry in
            ToDoList(todoData: entry)
            }
            .configurationDisplayName("Tareas pendientes")
            .description("Muestra tus tareas pendientes desde la pantalla principal.")
            .supportedFamilies([.systemLarge,.systemMedium])
    }
}
