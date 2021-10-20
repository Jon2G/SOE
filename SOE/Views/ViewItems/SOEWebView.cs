using Kit.Forms.Controls.WebView;
using SOE.API;
using SOE.Models.Data;
using SOEWeb.Shared;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Views.ViewItems
{
    public class SOEWebView : KitWebView
    {


        public override string HomePage => API.APIService.BaseUrl;
        public Task GoToSubjectNotesPage(Subject subject, User user) =>
            GoTo(string.Concat(APIService.BaseUrl, $"/materias/{user.Id}/{subject.IdTeacher}/{subject.GroupId}/{subject.Id}"));

        public SOEWebView()
        {
            this.Visual = VisualMarker.Material;
        }

    }
}
