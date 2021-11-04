using Kit.Daemon.Sync;
using Kit.Model;
using Kit.Services.Web;
using Kit.Sql.Sqlite;
using SOEWeb.Shared.Interfaces;
using System.Threading.Tasks;

namespace SOEWeb.Shared
{
    public abstract class OfflineSync : ModelBase, IOffline
    {
        public bool IsOffline { get; set; }

        public abstract Task<bool> Sync(IApplicationData app, ISyncService syncService);

        protected async Task<bool> CheckUser(IApplicationData app, ISyncService apiService)
        {
            await Task.Yield();
            if (app.UserBase.IsOffline)
            {
                if (!await app.UserBase.Sync(app, apiService))
                {
                    return false;
                }
            }
            return true;
        }
        public OfflineSync()
        {

        }

    }
}
