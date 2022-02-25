using Kit.Services.Web;
using SOE.API;
using SOE.Data;
using SOE.Models;
using SOEWeb.Shared;
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
            //Response response = await APIService.Current.PostContact(school, AppData.Instance.User);
            //return response.ResponseResult == APIResponseResult.OK;
            throw new NotImplementedException();
        }
        //public static  Task<bool> Delete(this SchoolContact school)=> 
        //    APIService.Current.DeleteContact(AppData.Instance.User.DocumentId, school);
    }
}
