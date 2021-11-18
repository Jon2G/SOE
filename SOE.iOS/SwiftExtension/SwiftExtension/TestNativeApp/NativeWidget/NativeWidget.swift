//
//  NativeWidget.swift
//  NativeWidget
//
//  Created by Chris Hamons on 6/30/20.
//

import WidgetKit
import SwiftUI
import Intents

struct Provider: IntentTimelineProvider {
    public func placeholder(in context: Context) -> TimeLineData {
        return trialTimeLineData();
    }
    
    public func getSnapshot(for configuration: ConfigurationIntent, in context: Context, completion: @escaping (TimeLineData) -> Void) {
        let entry = TimeLineData(days: dayTrial(), date: Date(), configuration: configuration,selectedDay: 1)
        completion(entry)
    }
    
    public func getTimeline(for configuration: ConfigurationIntent, in context: Context, completion: @escaping (Timeline<TimeLineData>) -> Void) {
        completion(readData(configuration: configuration));
    }
}



struct NativeWidgetEntryView : View {
    var entry: Provider.Entry

    var body: some View {
        VStack {
            ClassTimeList(day: entry.days[entry.selectedDay],timelineData: entry);
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ClassTimeList(day: days[0],timelineData: trialTimeLineData())
        //NativeWidgetEntryView(entry: <#T##Provider.Entry#>)
            .previewContext(WidgetPreviewContext(family: .systemLarge))
    }
}



@main
struct NativeWidget: Widget {
    private let kind: String = "NativeWidget"

    public var body: some WidgetConfiguration {
        IntentConfiguration(kind: kind, intent: ConfigurationIntent.self, provider: Provider()) {
            entry in
            //NativeWidgetEntryView(entry: entry)
            ClassTimeList(day: entry.days[entry.selectedDay],timelineData:entry)
            }
            .configurationDisplayName("Horario del día")
            .description("Muestra tu horario del día desde la pantalla principal.")
            .supportedFamilies([.systemLarge])
    }
}
