using SOEWeb.Shared;
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
