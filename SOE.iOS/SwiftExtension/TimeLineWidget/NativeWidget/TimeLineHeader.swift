//
//  TimeLineHeader.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 18/11/21.
//

import Foundation
import SwiftUI
import WidgetKit



struct TimeLineHeader: View {
    var day:DayModel;
    var data:TimeLineData;
    var body: some View {
        VStack(spacing:0){
        HStack() {
            Text(day.name).frame(maxWidth: .infinity, alignment: .center).textCase(.uppercase).font(Font.headline.weight(.bold));
        }.padding(10)//.background(Color(UIColor.init(hex: "#f5f5f5ff")))
            Divider();
        }
    }
}

struct TimeLineHeader_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            TimeLineHeader(day: days[0],data:trialTimeLineData())
        }
        .previewLayout(.fixed(width: 300, height: 70))
        .previewContext(WidgetPreviewContext(family: .systemLarge ))
    }
}
