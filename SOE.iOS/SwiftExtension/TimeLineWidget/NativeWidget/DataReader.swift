//
//  DataReader.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 27/11/21.
//

import Foundation

func load<T:Decodable>(_ filename: String,_ defaultValue:T) -> T{
    let data:Data
    guard let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: "group.com.soe.soe-app")
    else{
        fatalError("Couldn't get app group folder")
    }
    let file = url.appendingPathComponent(filename);
    do{
        data = try Data(contentsOf: file)
        //TodoTest
        //data = try "[{\"id\":0,\"title\":\"Trabajo de investigación sobre economía \",\"subject\":{\"id\":9004,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"\",\"group\":\"8CV23\",\"color\":\"#74bbfb\"},\"emoji\":\"⏰\",\"day\":\"Lunes\",\"formattedDateTime\":\"Lunes - 13/12\",\"color\":\"#FFADD8E6\"}]".data(using: String.Encoding.utf8) ?? Data(contentsOf: file);
        
        //data = try "[{\"id\":1,\"name\":\"Lunes\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":9,\"index\":3,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV21\",\"color\":\"#A783F9\"}]},{\"id\":2,\"name\":\"Martes\",\"classes\":[{\"id\":12,\"index\":0,\"subjectName\":\"PROYECTO DE INGENIERIA\",\"formattedTime\":\"16:00 - 19:00\",\"group\":\"8CV24\",\"color\":\"#f2dea4\"},{\"id\":9,\"index\":1,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV21\",\"color\":\"#A783F9\"},{\"id\":11,\"index\":2,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":3,\"name\":\"Miércoles\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":11,\"index\":3,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":4,\"name\":\"Jueves\",\"classes\":[{\"id\":12,\"index\":0,\"subjectName\":\"PROYECTO DE INGENIERIA\",\"formattedTime\":\"16:00 - 19:00\",\"group\":\"8CV24\",\"color\":\"#f2dea4\"},{\"id\":9,\"index\":1,\"subjectName\":\"REDES DE COMPUTADORAS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV21\",\"color\":\"#A783F9\"},{\"id\":11,\"index\":2,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]},{\"id\":5,\"name\":\"Viernes\",\"classes\":[{\"id\":10,\"index\":0,\"subjectName\":\"FORMULACION Y EVALUACION DE PROYECTOS\",\"formattedTime\":\"16:00 - 17:30\",\"group\":\"8CV23\",\"color\":\"#a1d7c9\"},{\"id\":7,\"index\":1,\"subjectName\":\"SISTEMAS DE INFORMACION II\",\"formattedTime\":\"17:30 - 19:00\",\"group\":\"8CV32\",\"color\":\"#f7cac9\"},{\"id\":8,\"index\":2,\"subjectName\":\"SISTEMAS EXPERTOS\",\"formattedTime\":\"19:00 - 20:30\",\"group\":\"8CV46\",\"color\":\"#FEC1FF\"},{\"id\":11,\"index\":3,\"subjectName\":\"SISTEMAS DISTRIBUIDOS\",\"formattedTime\":\"20:30 - 22:00\",\"group\":\"8CV22\",\"color\":\"#74bbfb\"}]}]".data(using: String.Encoding.utf8) ?? Data(contentsOf: file);
        
    }catch{
            //fatalError("Couldn't find \(filename).")
        return defaultValue;
    }

        do{
            let decoder=JSONDecoder()
            let decoed = try decoder.decode(T.self, from:data)
            return decoed
        }catch{
            fatalError("Couldn't parse \(filename) as \(T.self):\n\(error)")
        }
    
}
