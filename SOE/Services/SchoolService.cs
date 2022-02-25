using Google.Cloud.Firestore;
using SOE.API;
using SOE.Data;
using SOE.Models;
using System.Threading.Tasks;

namespace SOE.Services
{
    public static class SchoolService
    {
        public static async Task Save(School school)
        {
            await Task.Yield();
            WriteResult result = await FireBaseConnection.GetDocument<School>().SetAsync(school);
        }

        public static School Get() => AppData.Instance.LiteConnection.Table<School>().FirstOrDefault();

        //public static async Task<int> GetId(User user)
        //{
        //    var response = await APIService.Current.GetSchoolId(user);
        //    if (response.ResponseResult == APIResponseResult.OK)
        //    {
        //        return response.Extra;
        //    }

        //    return -1;
        //}
    }
}
