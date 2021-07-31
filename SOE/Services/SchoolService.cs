using SOE.Data;
using SOEWeb.Shared;

namespace SOE.Services
{
    public static class SchoolService
    {
        public static void Save(School school)
        {
            AppData.Instance.LiteConnection.DeleteAll<School>();
            AppData.Instance.LiteConnection.Insert(school);

        }

        public static School Get() => AppData.Instance.LiteConnection.Table<School>().FirstOrDefault();
    }
}
