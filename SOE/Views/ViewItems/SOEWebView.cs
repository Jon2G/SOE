using Kit.Forms.Controls.WebView;
using SOE.API;
using SOE.Models.Data;
using SOEWeb.Shared;
using System;
using System.Threading.Tasks;

namespace SOE.Views.ViewItems
{
    public class SOEWebView : KitWebView
    {

        private const string SubjectNotesPage = "/materias/{0}/{1}";
        public override string HomePage => API.APIService.BaseUrl;
        public Task GoToSubjectNotesPage(Subject subject,User user) =>
            GoTo(string.Concat(APIService.BaseUrl, string.Format(SubjectNotesPage,user.Id,subject.Id)));

        

    }
}
