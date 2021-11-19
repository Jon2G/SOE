//
//  DayModel.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 16/11/21.
//

import Foundation

struct DayModel:Hashable,Codable,Identifiable{
    var id:Int //Day of the week
    var name:String
    var classes:[ClassSquare]
}

func dayTrial() -> [DayModel] {
    var trialDays:[DayModel] = [DayModel]();
    trialDays.append(DayModel(id:1,name:"Lunes",classes:loadClasses(1)));
    trialDays.append(DayModel(id:1,name:"Martes",classes:loadClasses(1)));
    trialDays.append(DayModel(id:1,name:"Miércoles",classes:loadClasses(1)));
    trialDays.append(DayModel(id:1,name:"Jueves",classes:loadClasses(1)));
    trialDays.append(DayModel(id:1,name:"Viernes",classes:loadClasses(1)));
    return trialDays;
}
