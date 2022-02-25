using Kit.Forms.Controls.WebView;
using SOE.API;
using SOE.Models;
using SOE.Models.Data;
using SOE.Secrets;
using SOEWeb.Shared;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SOE.Views.ViewItems
{
    public class SOEWebView : KitWebView
    {


        public override string HomePage => DotNetEnviroment.BaseUrl;
        //public Task GoToSubjectNotesPage(Subject subject, User user) =>
        //    GoTo(string.Concat(DotNetEnviroment.BaseUrl, $"/materias/{user.Guid}/{subject.IdTeacher}/{subject.GroupId}/{subject.Guid}"));

        public SOEWebView()
        {
            this.Visual = VisualMarker.Material;
        }

    }
}
