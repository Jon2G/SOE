using SOEWeb.Shared;
using SOE.Data;
using SOE.Models;

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
