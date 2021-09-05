using SOE.API;
using SOE.Data;
using SOE.Models;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SOE.Services
{
    public static class SchoolContactsService
    {
        public static async Task<bool> Upload(this SchoolContact school)
        {
            Response response = await APIService.PostContact(school, AppData.Instance.User);
            return response.ResponseResult == APIResponseResult.OK;
        }
        public static  Task<bool> Delete(this SchoolContact school)=> APIService.DeleteContact(AppData.Instance.User.Id, school);
        
        public static async Task<ObservableCollection<ContactsByDeparment>> Get()
        {
            await Task.Yield();
            var contacts = await API.APIService.GetContacts(AppData.Instance.User.School.Id);
            return new ObservableCollection<ContactsByDeparment>(contacts);
        }
    }
}
