using System;
using System.Collections.Generic;
using System.Linq;
using CoreFoundation;
using Foundation;
using UIKit;

namespace SOE.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
                UIApplication.Main(args, null, typeof(AppDelegate));
            }catch(Exception ex)
            {
               CoreFoundation.OSLog.Default.Log(OSLogLevel.Error,ex.ToString());
                UIAlertController.Create("ERROR", ex.ToString(), UIAlertControllerStyle.Alert);
            }
            
        }
    }
}
