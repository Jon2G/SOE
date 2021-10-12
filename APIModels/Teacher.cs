using Kit.Services.Web;
using Kit.Sql.Attributes;
using Kit.Sql.Sqlite;
using SOEWeb.Shared.Interfaces;
using System.Threading.Tasks;

namespace SOEWeb.Shared
{
    public class Teacher : OfflineSync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        public override async Task<bool> Sync(IApplicationData app, ISyncService apiService)
        {
            await Task.Yield();
            if (!await CheckUser(app, apiService))
            {
                return false;
            }
            Response<Teacher> response = await apiService.Sync(this);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Teacher teacher = response.Extra;
                app.LiteConnection.Update(teacher, x => x.Id == this.Id);
                app.LiteConnection.Update($"update teacher set IdTeacher='{teacher.Id}',IsOffline=0 where IdTeacher='{this.Id}'");
                app.LiteConnection.Update($"update Subject set IdTeacher='{teacher.Id}' where IdTeacher='{this.Id}'");
                return true;
            }
            return false;
        }
    }
}
