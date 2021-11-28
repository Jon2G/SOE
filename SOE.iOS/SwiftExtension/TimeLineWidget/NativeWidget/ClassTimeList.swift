//
//  ClassTimeList.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 16/11/21.
//
import SwiftUI
import WidgetKit

struct ClassTimeList: View {
    var day:DayModel;
    var timelineData:TimeLineData;
    var body: some View {
        VStack(spacing:0) {
            TimeLineHeader(day: day,data: timelineData);
            if(day.classes.isEmpty){
                Spacer()
                Text("EL SEMESTRE HA TERMINADO 😎🍻").frame(width: .infinity,alignment: .center)
                Text("Actualiza tu horario desde ajustes en la App SOE")
                Spacer()
            }else{
                ForEach(day.classes) { classSquare in
                    ClassTimeRow(classSquare: classSquare)
                    Divider()
                }
            }
        }
        
    }
}

struct ClassTimeList_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ClassTimeRow(classSquare: days[0].classes[0])
            ClassTimeRow(classSquare: days[1].classes[1])
            ClassTimeRow(classSquare: days[2].classes[2])
            ClassTimeRow(classSquare: days[3].classes[3])
        }
        .previewLayout(.fixed(width: 300, height: 70))
        //.previewContext(WidgetPreviewContext(family: .systemMedium))
    }
}
