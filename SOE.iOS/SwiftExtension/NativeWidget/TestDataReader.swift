
import Foundation

struct Values: Codable {
    let data: [String: Datum]
}

struct Datum: Codable {
    let value, delta: String
}

func readTestData() -> Values {
    if let url = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: "group.com.soe.soe-app") {
        let path = url.appendingPathComponent("testAppState.json");
        let data = try? String(contentsOf: path);
        if let data = data {
            let jsonData = data.data(using: .utf8)!
            let value = try? JSONDecoder().decode(Values.self, from: jsonData);
            
            if let value = value {
                return value;
            }
        }
    }
    return Values (data: [String: Datum]())
}
