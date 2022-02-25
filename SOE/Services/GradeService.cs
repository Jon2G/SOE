using SOEWeb.Shared;
using SOE.Data;
using SOE.Models;

namespace SOE.Services
{
    internal static class GradeService
    {
        internal static void Save(Grade grade)
        {
            AppData.Instance.LiteConnection.Insert(grade);
        }

        public static void ClearAll()
        {
            AppData.Instance.LiteConnection.DeleteAll<Grade>(false);
        }
    }
}
