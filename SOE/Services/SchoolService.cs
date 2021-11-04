using Kit.Services.Web;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOEWeb.Shared;
using System.Threading.Tasks;

namespace SOE.Services
{
    public static class SchoolService
    {
        public static void Save(School school)
        {
            AppData.Instance.LiteConnection.DeleteAll<School>(false);
            AppData.Instance.LiteConnection.Insert(school, false);

        }

        public static School Get() => AppData.Instance.LiteConnection.Table<School>().FirstOrDefault();

        public static async Task<int> GetId(User user)
        {
            var response = await APIService.GetSchoolId(user);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                return response.Extra;
            }

            return -1;
        }
    }
}
