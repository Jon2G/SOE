//
//  ToDoList.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation
import SwiftUI
import WidgetKit

struct ToDoList: View {
    var todoData:ToDoData;
    var body: some View {
        VStack(alignment:.leading, spacing:0) {
            ToDoHeader();
            if(todoData.todos.isEmpty){
                Spacer()
                VStack(alignment: .leading, spacing: 6) {
                    Text("SIN TAREAS PENDIENTES 😎🍻").frame(width: .infinity,alignment: .center)
                }
            }else{
                ForEach(todoData.todos) { todo in
                    ToDoRow(todoModel: todo)
                    Divider()
                }
            }
            Spacer()
        }.frame(maxWidth: .infinity, maxHeight: .infinity)
        
    }
}

struct ToDoList_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ToDoList(todoData: trialToDoData())
                
        }
        .previewLayout(.fixed(width: 300, height: 70))
        .previewContext(WidgetPreviewContext(family: .systemMedium))
    }
}
