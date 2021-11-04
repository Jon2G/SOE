using Kit.Services.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOEWeb.Shared.Interfaces
{
    public interface ISyncService
    {
        public Task<Response<UserBase>> Sync(UserBase user);
        public Task<Response<ClassTime>> Sync(ClassTime classTime);
        public Task<Response<Subject>> Sync(Subject subject);
        public Task<Response<Teacher>> Sync(Teacher teacher);
        public Task<Response<Grade>> Sync(Grade grade);
    }
}
