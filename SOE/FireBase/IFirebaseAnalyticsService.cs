﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.FireBase
{
    public interface IFirebaseAnalyticsService
    {
        void LogEvent(string eventId);
        void LogEvent(string eventId, string paramName, string value);
        void LogEvent(string eventId, IDictionary<string, string> parameters);
        void SetUserId(string userId);
    }
}
