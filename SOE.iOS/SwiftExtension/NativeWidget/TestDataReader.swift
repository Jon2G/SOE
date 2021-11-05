
import Foundation
import FileProvider

struct Values: Codable {
    let data: [String: Datum]
}

struct Datum: Codable {
    let value, delta: String
}

func readTestData() -> Values {
    var jsonData :Data ;
    
    
    if let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: "group.com.xamarin.sample.TestApplication") {
    
        let path = url.appendingPathComponent("timeline.json", isDirectory: false);
        //let data = try? String(contentsOf: path);
        
        
        //if let data=data{
        //    jsonData = data.data(using: .utf8)!
        //}else{
            jsonData = "{\"data\":{\"2021-08-12\":{\"value\":\"50.34\",\"delta\":\"\(path)\"}}}".data(using: .utf8)!
        //}
    }else{
        jsonData = "{\"data\":{\"2021-08-12\":{\"value\":\"50.34\",\"delta\":\"NO URLA\"}}}".data(using: .utf8)!
    }

    
    let value = try? JSONDecoder().decode(Values.self, from: jsonData);
    if let value = value {
        return value;
    }
    
        
    return Values (data: [String: Datum]())
}
