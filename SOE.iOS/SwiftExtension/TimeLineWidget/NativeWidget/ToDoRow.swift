//
//  ToDoRow.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation
import SwiftUI
import WidgetKit

struct ToDoRow:View{
    var todoModel:ToDoModel
    var body: some View{
        VStack(alignment: .leading, spacing: 0){
        HStack{
            Rectangle().frame(width: 20).foregroundColor(Color(UIColor(hex:todoModel.color)))
            VStack(spacing:0){
                
                HStack(spacing:0){
                    Text(todoModel.emoji)
                    Text(todoModel.title).frame(maxWidth: .infinity, alignment: .leading).textCase(.uppercase).font(Font.headline.weight(.bold))
                }
                Text(todoModel.subject.subjectName).frame(maxWidth: .infinity, alignment: .leading).textCase(.uppercase)
                    .background(Color(UIColor(hex: todoModel.subject.color)))
                HStack(spacing:0){
                    Text(todoModel.day)
                    Text(" ⏰ ")
                    Text(todoModel.formattedDateTime)
                    Text(" ")
                    Text(todoModel.subject.group).frame(maxWidth: .infinity, alignment: .leading)
                }
            }.padding(0)//.background(Color.yellow)
        }.padding(0).frame(maxWidth: .infinity, alignment: .topLeading)//.background(Color.red)
        }
    }
}

struct ToDoRow_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ToDoRow(todoModel: todos[0])
            ToDoRow(todoModel: todos[1])
            ToDoRow(todoModel: todos[2])
        }
        .previewLayout(.fixed(width: 300, height: 70))
        //.previewContext(WidgetPreviewContext(family: .systemMedium))
    }
}
