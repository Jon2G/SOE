using APIModels;
using System;
using System.Collections.Generic;
using System.Text;
using SOE.Data;

namespace SOE.Services
{
    internal static class ClassTimeService
    {
        internal static void Save(ClassTime classTime)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(classTime);
        }
    }
}
