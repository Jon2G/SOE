//
//  ToDoHeader.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 24/11/21.
//

import Foundation
import SwiftUI
import WidgetKit


struct ToDoHeader: View {
    var body: some View {
        VStack(spacing:0){
        HStack() {
            Text("Tareas pendientes").frame(maxWidth: .infinity, alignment: .center).textCase(.uppercase).font(Font.headline.weight(.bold));
        }.padding(10)//.background(Color(UIColor.init(hex: "#f5f5f5ff")))
            Divider();
        }
    }
}

struct ToDoHeader_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ToDoHeader()
        }
        .previewLayout(.fixed(width: 300, height: 70))
        //.previewContext(WidgetPreviewContext(family: .systemLarge ))
    }
}
