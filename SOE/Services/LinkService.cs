using Kit.Services.Web;
using SOE.API;
using SOE.Data;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.Services
{
    public static class LinkService
    {
        public static async Task<bool> Upload(this Link link, Subject subject)
        {
            Response response = await APIService.PostLink(subject, link, AppData.Instance.User);
            return response.ResponseResult == APIResponseResult.OK;
        }
    }
}
