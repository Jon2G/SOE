using System;
using System.Collections.Generic;
using System.Linq;
using CoreFoundation;
using Foundation;
using SOE.Fonts;
using UIKit;
using Xamarin.Forms;
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
                Console.WriteLine("SOE.iOS " + ex.ToString());
               CoreFoundation.OSLog.Default.Log(OSLogLevel.Error,ex.ToString());
                UIAlertController.Create("ERROR", "SOE.iOS "+ ex.ToString(), UIAlertControllerStyle.Alert);
            }
            
        }
    }
}
