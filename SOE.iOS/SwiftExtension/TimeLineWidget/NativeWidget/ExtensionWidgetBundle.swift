//
//  ExtensionWidgetBundle.swift
//  NativeWidgetExtension
//
//  Created by Jonathan Eduardo Garcia Garcia on 27/11/21.
//
import Foundation
import WidgetKit
import SwiftUI
import Intents

@main
struct ExtensionWidgetBundle: WidgetBundle {
    private let kind: String = "NativeWidget"
    public var body: some Widget {
       TimeLineWidget();
       ToDoWidget();
   }
}
