using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SOEWeb.Shared;
using Kit.Model;
using SOE.Data;
using SOE.Views.Pages.Login;
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
            SchoolSearch = new List<School>();
            int Id = 1;
            for (;Id <= 19; Id++)
            {
                string HomePage = $"https://www.saes.cecyt{Id}.ipn.mx/";
                string SchoolPage = $"https://www.cecyt{Id}.ipn.mx/";
                this.SchoolSearch.Add(new School(Id, HomePage, $"CECyT {Id}",
                    $"{HomePage}Images/logos/{Id:D2}.png",SchoolPage));
            }
            Id--;
            this.SchoolSearch.AddRange(
                new School[]{
                    new (++Id,"https://www.saes.cet1.ipn.mx/", $"CET 1", $"https://www.saes.cet1.ipn.mx/Images/logos/17.png","https://www.cet1.ipn.mx/"),
                    new(++Id,"https://www.saes.esimecu.ipn.mx/","ESIME CULHUACÁN","https://www.saes.esimecu.ipn.mx/Images/logos/35.png","https://www.esimecu.ipn.mx/"),
                    new(++Id,"https://www.saes.esimeazc.ipn.mx/","ESIME AZCAPOTZALCO","https://www.saes.esimeazc.ipn.mx/Images/logos/36.png","'https://www.esimeazc.ipn.mx/"),
                    new(++Id,"https://www.saes.esimetic.ipn.mx/","ESIME TICOMÁN","https://www.saes.esimetic.ipn.mx/Images/logos/37.png","https://www.esimetic.ipn.mx/"),
                    new(++Id,"https://www.saes.esimez.ipn.mx/","ESIME ZACATENCO","https://www.saes.esimez.ipn.mx/Images/logos/30.png","https://www.esimez.ipn.mx/"),
                    new(++Id,"https://www.saes.esiatec.ipn.mx/","ESIA TECAMACHALCO","https://www.saes.esiatec.ipn.mx/Images/logos/38.png","https://www.esiatec.ipn.mx/"),
                    new(++Id,"https://www.saes.esiatic.ipn.mx/","ESIA TICOMÁN","https://www.saes.esiatec.ipn.mx/Images/logos/38.png","https://www.esiatic.ipn.mx/"),
                    new(++Id,"https://www.saes.esiaz.ipn.mx/","ESIA ZACATENCO","https://www.saes.esiaz.ipn.mx/Images/logos/31.png","https://www.esiaz.ipn.mx/"),
                    new(++Id,"https://www.saes.cicsma.ipn.mx/","CICS MILPA ALTA","https://www.saes.cicsma.ipn.mx/Images/logos/61.png","https://www.cicsma.ipn.mx/"),
                    new(++Id,"https://www.saes.cicsst.ipn.mx/","CICS SANTO TOMAS","https://www.saes.cicsst.ipn.mx/Images/logos/65.png","https://www.cics-sto.ipn.mx/"),
                    new(++Id,"https://www.saes.escasto.ipn.mx/","ESCA SANTO TOMAS","https://www.saes.escasto.ipn.mx/Images/logos/40.png","https://www.escasto.ipn.mx/"),
                    new(++Id,"https://www.saes.escatep.ipn.mx/","ESCA TEPEPAN","https://www.saes.escatep.ipn.mx/Images/logos/43.png","https://www.escatep.ipn.mx/"),
                    new(++Id,"https://www.saes.encb.ipn.mx/","ENCB","https://www.saes.encb.ipn.mx/Images/logos/50.png","https://www.encb.ipn.mx/"),
                    new(++Id,"https://www.saes.enmh.ipn.mx/","ENMH","https://www.saes.enmh.ipn.mx/Images/logos/52.png","https://www.enmh.ipn.mx/"),
                    new(++Id,"https://www.saes.eseo.ipn.mx/","ESEO","https://www.saes.eseo.ipn.mx/Images/logos/53.png","https://www.eseo.ipn.mx/"),
                    new(++Id,"https://www.saes.esm.ipn.mx/","ESM","https://www.saes.esm.ipn.mx/Images/logos/51.png","https://www.esm.ipn.mx/"),
                    new(++Id,"https://www.saes.ese.ipn.mx/","ESE","https://www.saes.ese.ipn.mx/Images/logos/41.png","https://www.ese.ipn.mx/"),
                    new(++Id,"https://www.saes.est.ipn.mx/","EST","https://www.saes.est.ipn.mx/Images/logos/42.png","https://www.est.ipn.mx/"),
                    new(++Id,"https://www.saes.upibi.ipn.mx/","UPIBI","https://www.saes.upibi.ipn.mx/Images/logos/62.png","https://www.upibi.ipn.mx/"),
                    new(++Id,"https://www.saes.upiicsa.ipn.mx/","UPIICSA","https://www.saes.upiicsa.ipn.mx/Images/logos/60.png","https://www.upiicsa.ipn.mx/"),
                    new(++Id,"https://www.saes.upiita.ipn.mx/","UPIITA","https://www.saes.upiita.ipn.mx/Images/logos/64.png","https://www.upiita.ipn.mx/"),
                    new(++Id,"https://www.saes.escom.ipn.mx/","ESCOM","https://www.saes.escom.ipn.mx/Images/logos/63.png","https://www.escom.ipn.mx/"),
                    new(++Id,"https://www.saes.esfm.ipn.mx/","ESFM","https://www.saes.esfm.ipn.mx/Images/logos/33.png","https://www.esfm.ipn.mx/"),
                    new(++Id,"https://www.saes.esiqie.ipn.mx/","ESIQUE","https://www.saes.esiqie.ipn.mx/Images/logos/32.png","https://www.esiqie.ipn.mx/"),
                    new(++Id,"https://www.saes.esit.ipn.mx/","ESIT","https://www.saes.esit.ipn.mx/Images/logos/34.png","https://www.esit.ipn.mx/"),
                    new(++Id,"https://www.saes.upiig.ipn.mx/","UPIIG","https://www.saes.upiig.ipn.mx/Images/logos/66.png","https://www.upiig.ipn.mx/"),
                    new(++Id,"https://www.saes.upiih.ipn.mx/","UPIIH","https://www.saes.upiih.ipn.mx/Images/logos/68.png","https://www.upiih.ipn.mx/"),
                    new(++Id,"https://www.saes.upiiz.ipn.mx/","UPIIZ","https://www.saes.upiiz.ipn.mx/Images/logos/67.png","https://www.zacatecas.ipn.mx/"),
                    new(++Id,"https://www.saes.enba.ipn.mx/","ENBA","https://www.saes.enba.ipn.mx/Images/logos/44.png","https://www.enba.ipn.mx/"),
                    new(++Id,"https://www.saes.upiic.ipn.mx/","UPIIC","https://www.saes.upiic.ipn.mx/Images/logos/69.png","https://www.upiic.ipn.mx/"),
                    new(++Id,"https://www.saes.upiip.ipn.mx/","UPIIP","https://www.saes.upiip.ipn.mx/Images/logos/70.png","https://www.upiip.ipn.mx/"),
                    new(++Id,"https://www.saes.upiem.ipn.mx/","UPIEM","https://www.saes.upiem.ipn.mx/Images/logos/45.png","https://www.upiem.ipn.mx/"),
                    new(++Id,"https://www.saes.upiit.ipn.mx/","UPIIT","https://www.saes.upiit.ipn.mx/Images/logos/71.png","https://www.upiit.ipn.mx/")
                });
            SchoolSearch.TrimExcess();
            this.Schools = SchoolSearch.ToArray();
        }
        
            private void SelectSchool(School School)
        {
            AppData.Instance.User.School = School;
            Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage(this.PrivacyAlertDisplayed), true);
        }
    }
}
