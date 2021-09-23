using Foundation;
using SOE.FireBase;
using SOE.iOS.FireBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAnalyticsService))]
namespace SOE.iOS.FireBase
{
    public class FirebaseAnalyticsService : IFirebaseAnalyticsService
    {
        public void LogEvent(string eventId)
        {
            LogEvent(eventId, (IDictionary<string, string>)null);
        }

        public void LogEvent(string eventId, string paramName, string value)
        {
            LogEvent(eventId, new Dictionary<string, string>
            {
                { paramName, value }
            });
        }

        public void SetUserId(string userId)
        {
#if !DEBUG
    Analytics.SetUserId(userId);
#endif
        }

        public void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
#if !DEBUG
    if (parameters == null)
    {
      Analytics.LogEvent(eventId, parameters: null);
      return;
    }

    var keys = new List<NSString>();
    var values = new List<NSString>();
    foreach (var item in parameters)
    {
      keys.Add(new NSString(item.Key));
      values.Add(new NSString(item.Value));
    }

    var parametersDictionary = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count);
    Analytics.LogEvent(eventId, parametersDictionary);
#endif
        }
    }
}