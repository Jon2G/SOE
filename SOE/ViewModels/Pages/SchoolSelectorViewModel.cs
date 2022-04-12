using AsyncAwaitBestPractices;
using FirestoreLINQ;

using Kit;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SOEWeb.Shared;
using Kit.Model;
using SOE.API;
using SOE.Data;
using SOE.Models;
using SOE.Views.Pages.Login;
using System;
using System.Text;
using System.Threading.Tasks;
using static Xamarin.Forms.Application;

namespace SOE.ViewModels.Pages
{
    public class SchoolSelectorViewModel : ModelBase
    {
        private School[] Schools;
        private List<School> _SchoolSearch;

        public List<School> SchoolSearch
        {
            get => _SchoolSearch;
            set
            {
                _SchoolSearch = value;
                Raise(() => SchoolSearch);
            }
        }

        public ICommand SelectSchoolCommand { get; }

        public ICommand TextChangedCommand { get; }
        public bool PrivacyAlertDisplayed { get; set; }
        public SchoolSelectorViewModel()
        {
            this.SelectSchoolCommand = new Xamarin.Forms.Command<School>(SelectSchool);
            this.TextChangedCommand = new Xamarin.Forms.Command<string>(TextChanged);
            GetSchools();
        }

        private void TextChanged(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                this.SchoolSearch = this.Schools.ToList();
                return;
            }
            Search(search);
        }
        private void Search(string search) => SchoolSearch = Schools.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();

        private void GetSchools()
        {
            this.Schools =
#pragma warning disable CS8601 // Possible null reference assignment.
                Newtonsoft.Json.JsonConvert.DeserializeObject<School[]>(
                    ReflectionCaller.ToText(ReflectionCaller.FromThis(this).GetResource("Schools.json"),Encoding.UTF7));
#pragma warning restore CS8601 // Possible null reference assignment.
            SchoolSearch = this.Schools.ToList();
        }


        private void SelectSchool(School School)
        {
            AppData.Instance.User.School = School;
            Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage(this.PrivacyAlertDisplayed), true);
        }
    }
}
