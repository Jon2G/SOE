//
//  ToDoModel.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation

struct ToDoModel:Hashable,Codable,Identifiable{
    var id:String
    var title:String
    var subject:ClassSquare
    var emoji:String
    var day:String
    var formattedDateTime:String
    var color:String
}

var todos:[ToDoModel] = todosTrial()

func todosTrial() -> [ToDoModel] {
    var trialTodos:[ToDoModel] = [ToDoModel]();
    trialTodos.append(ToDoModel(id:"1ASDASD",title:"Estudiar programación",subject: ClassSquare(id: "xxxxx", index: 0, subjectName:"Redes de computadoras", formattedTime: "14:30-16:00",group: "8CV1",color:"#a1d7c9"),emoji: "🧐",day: "19/11",formattedDateTime: "16:00",color:"#a1d7c9"));
    trialTodos.append(ToDoModel(id:"2ASDAXA",title:"Reporte de 5 cuartillas",subject:ClassSquare(id:  "zzzzzzz", index: 0, subjectName:"Redes de computadoras", formattedTime: "14:30-16:00",group: "8CV1",color:"#a1d7c9"),emoji: "🧐",day: "19/11",formattedDateTime: "16:00",color:"#a1d7c9"));
    trialTodos.append(ToDoModel(id:"3XADEA",title:"Leerx documento",subject:ClassSquare(id:  "wwww", index: 0, subjectName:"Redes de computadoras", formattedTime: "14:30-16:00",group: "8CV1",color:"#a1d7c9"),emoji: "🧐",day: "19/11",formattedDateTime: "16:00",color:"#a1d7c9"));
    return trialTodos;
}
