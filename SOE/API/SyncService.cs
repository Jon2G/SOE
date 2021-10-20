using Kit.Services.Web;
using SOE.Data;
using SOE.Services;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using SOEWeb.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.API
{
    public class SyncService : ISyncService
    {
        public async Task<Response<UserBase>> Sync(UserBase user)
        {
            Response<int> response = await APIService.SignUp(UserType.REGULAR_USER,
                new SOEWeb.Shared.Device()
                {
                    DeviceKey = Kit.Daemon.Devices.Device.Current.DeviceId,
                    Brand = Kit.Daemon.Devices.Device.Current.GetDeviceBrand(),
                    Platform = Kit.Daemon.Devices.Device.Current.GetDevicePlatform(),
                    Model = Kit.Daemon.Devices.Device.Current.GetDeviceModel(),
                    Name = Kit.Daemon.Devices.Device.Current.GetDeviceName()
                });
            if (response.ResponseResult == APIResponseResult.OK)
            {
                AppData.Instance.LiteConnection.EXEC($"UPDATE USER SET ID='{response.Extra}',IsOffline='0' WHERE ID='{user.Id}'");
                AppData.Instance.User.Id = response.Extra;
                AppData.Instance.User.IsOffline =false;

                return new Response<UserBase>(APIResponseResult.OK, "Ok", AppData.Instance.User);
            }
            return new Response<UserBase>(APIResponseResult.INTERNAL_ERROR, "ERROR");
        }

        public async Task<Response<ClassTime>> Sync(ClassTime classTime)
        {
            await Task.Yield();
            Subject subject = SubjectService.Get(classTime.IdSubject);
            if (subject.IsOffline)
            {
                if (!await subject.Sync(AppData.Instance, this))
                {
                    return Response<ClassTime>.Error;
                }
            }
            Response<ClassTime> response = await APIService.PostClassTime(subject.IdTeacher, subject.Id, classTime.Day, classTime.Begin, classTime.End);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                ClassTime NewClassTime = response.Extra;
                AppData.Instance.LiteConnection.EXEC($"UPDATE ClassTime SET ID='{NewClassTime.Id}',IsOffline='0' WHERE ID='{classTime.Id}'");
                return new Response<ClassTime>(APIResponseResult.OK, "Ok", NewClassTime);
            }
            return new Response<ClassTime>(APIResponseResult.INTERNAL_ERROR, "ERROR");
        }

        public async Task<Response<Subject>> Sync(Subject subject)
        {
            await Task.Yield();
            var response = await APIService.PostSubject(subject);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Subject NewSubject = response.Extra;
                AppData.Instance.LiteConnection.EXEC($"UPDATE Subject SET ID='{NewSubject.Id}',IsOffline=0 WHERE ID='{subject.Id}'");
                AppData.Instance.LiteConnection.Update(NewSubject);
                return new Response<Subject>(APIResponseResult.OK, "Ok", NewSubject);
            }
            return new Response<Subject>(APIResponseResult.INTERNAL_ERROR, "ERROR");

        }

        public async Task<Response<Teacher>> Sync(Teacher teacher)
        {
            await Task.Yield();
            var response = await APIService.PostTeacher(teacher);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Teacher NewTeacher = response.Extra;
                AppData.Instance.LiteConnection.EXEC($"UPDATE Teacher SET ID='{NewTeacher.Id}',IsOffline=0 WHERE ID='{teacher.Id}'");
                return new Response<Teacher>(APIResponseResult.OK, "Ok", response.Extra);
            }
            return new Response<Teacher>(APIResponseResult.INTERNAL_ERROR, "ERROR");
        }

        public async Task<Response<Grade>> Sync(Grade grade)
        {
            await Task.Yield();
            Subject subject = SubjectService.Get(grade.SubjectId);
            if (subject.IsOffline)
            {
                if (!await subject.Sync(AppData.Instance, this))
                {
                    return Response<Grade>.Error;
                }
            }
            var response = await APIService.PostGrade(grade, subject);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Grade NewGrade = response.Extra;
                AppData.Instance.LiteConnection.EXEC($"UPDATE Teacher SET ID='{NewGrade.Id}',IsOffline=0,SyncGuid='{NewGrade.Guid}' WHERE ID='{grade.Id}'");
                return new Response<Grade>(APIResponseResult.OK, "Ok", NewGrade);
            }
            return new Response<Grade>(APIResponseResult.INTERNAL_ERROR, "ERROR");
        }
    }
}
