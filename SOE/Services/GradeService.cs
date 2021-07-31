using SOEWeb.Shared;
using SOE.Data;

namespace SOE.Services
{
    internal static class GradeService
    {
        internal static void Save(Grade grade)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(grade);
        }
    }
}
