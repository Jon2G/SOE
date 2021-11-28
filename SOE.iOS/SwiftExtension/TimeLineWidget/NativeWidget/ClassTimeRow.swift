//
//  ClassTimeRow.swift
//  TestNativeApp
//
//  Created by Jonathan Eduardo Garcia Garcia on 15/11/21.
//

import Foundation
import SwiftUI
import WidgetKit

struct ClassTimeRow:View{
    var classSquare:ClassSquare
    var body: some View{
        HStack{
            Rectangle().frame(width: 20).foregroundColor(Color(UIColor(hex:classSquare.color)))
            VStack(spacing:0){
                Text(classSquare.subjectName).frame(maxWidth: .infinity, alignment: .leading).textCase(.uppercase).font(Font.headline.weight(.bold))
                Text(classSquare.formattedTime).frame(maxWidth: .infinity, alignment: .leading)
                Text(classSquare.group).frame(maxWidth: .infinity, alignment: .leading)
            }.padding(0)//.background(Color.yellow)
        }.padding(0)//.background(Color.red)
    }
}

struct ClassTimeRow_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ClassTimeRow(classSquare: days[0].classes[0])
            //ClassTimeRow(classSquare: days[1].Classes[1])
            //ClassTimeRow(classSquare: days[2].Classes[2])
            //ClassTimeRow(classSquare: days[3].Classes[3])
            //ClassTimeRow(classSquare: days[4].Classes[4])
        }
        .previewLayout(.fixed(width: 300, height: 70))
        //.previewContext(WidgetPreviewContext(family: .systemMedium))
    }
}
