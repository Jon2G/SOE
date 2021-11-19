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
   return TimeLineData(days: dayTrial(), date: Date(), configuration: Provider.Intent.init(),selectedDay: 1);
}

func readData(configuration: ConfigurationIntent) -> Timeline<TimeLineData> {
    var entries = [TimeLineData]()
    let today:Date = Date();
    let currentDate = Date();
    let data:TimeLineData = TimeLineData(days: load("timelineAppState.json"), date: currentDate, configuration: configuration,selectedDay:today.dayNumberOfWeek());
    entries.append(data);
    let midnight = Calendar.current.startOfDay(for: currentDate)
    let nextMidnight = Calendar.current.date(byAdding: .day, value: 1, to: midnight)!
    return Timeline(entries: entries, policy: .after(nextMidnight));
}

func load<T:Decodable>(_ filename: String) -> T{
    let data:Data
    guard let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: "group.com.soe.soe-app")
    else{
        fatalError("Couldn't get app group folder")
    }
    let file = url.appendingPathComponent(filename);
    do{
        data = try Data(contentsOf: file)
        //data = try "[{\"id\":1,\"name\":\"Lunes\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":9,\"index\":3,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV21\",\"color\":\"#A783F9\"}]},{\"id\":2,\"name\":\"Martes\",\"classes\":[{\"id\":12,\"index\":0,\"subjectName\":\"PROYECTO DE INGENIERIA\",\"formattedTime\":\"16:00 - 19:00\",\"group\":\"8CV24\",\"color\":\"#f2dea4\"},{\"id\":9,\"index\":1,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV21\",\"color\":\"#A783F9\"},{\"id\":11,\"index\":2,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":3,\"name\":\"Miércoles\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":11,\"index\":3,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":4,\"name\":\"Jueves\",\"classes\":[{\"id\":12,\"index\":0,\"subjectName\":\"PROYECTO DE INGENIERIA\",\"formattedTime\":\"16:00 - 19:00\",\"group\":\"8CV24\",\"color\":\"#f2dea4\"},{\"id\":9,\"index\":1,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV21\",\"color\":\"#A783F9\"},{\"id\":11,\"index\":2,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":5,\"name\":\"Viernes\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":11,\"index\":3,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]}]".data(using: String.Encoding.utf8) ?? Data(contentsOf: file);
        
    }catch{
            fatalError("Couldn't find \(filename).")
        }

        do{
            let decoder=JSONDecoder()
            return try decoder.decode(T.self, from:data)
        }catch{
            fatalError("Couldn't parse \(filename) as \(T.self):\n\(error)")
        }
    
}
