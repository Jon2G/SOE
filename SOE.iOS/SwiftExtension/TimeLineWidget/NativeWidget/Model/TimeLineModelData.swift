//
//  ModelData.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 16/11/21.
//

import Foundation
struct ModelData:Codable{
    var days:[DayModel]
}

var days:[DayModel] = load() // load("classSquare.json")

func load() -> [DayModel] {
    var Dummydays = [DayModel]()
    Dummydays.append(DayModel(id: 1, name:"Lunes", classes: loadClasses(1)))
    Dummydays.append(DayModel(id: 2, name:"Martes", classes: loadClasses(2)))
    Dummydays.append(DayModel(id: 3, name:"Miercoles", classes: loadClasses(3)))
    Dummydays.append(DayModel(id: 4, name:"Jueves", classes: loadClasses(4)))
    Dummydays.append(DayModel(id: 5, name:"Viernes", classes: loadClasses(5)))
    return Dummydays;
}

func loadClasses(_ dayInt: Int) -> [ClassSquare]{
    var DummyClasses=[ClassSquare]()
    DummyClasses.append(ClassSquare(id: 1, index: 0, subjectName:"Redes de computadoras", formattedTime: "14:30-16:00",group: "8CV1",color:"#a1d7c9"))
    DummyClasses.append(ClassSquare(id: 2, index: 1, subjectName:"Bases de datos", formattedTime: "16:00-17:30",group: "8CV2",color: "#f7cac9"))
    DummyClasses.append(ClassSquare(id: 3, index: 2, subjectName:"Proyecto de ingeniería", formattedTime: "17:30-19:00",group: "8CV3",color: "#FEC1FF"))
    DummyClasses.append(ClassSquare(id: 4, index: 3, subjectName:"Estructura de datos", formattedTime: "19:00-20:30",group: "8CV4",color: "#A783F9"))
    DummyClasses.append(ClassSquare(id: 5, index: 4, subjectName:"Metodología de la investigación", formattedTime: "20:30-22:00",group: "8CV5",color: "#f2dea4"))
    return DummyClasses;
    
}
