using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOE.Fonts;
using SOE.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationView
    {
        public override string Title => "Notificaciones";
        public override string Icon => FontelloIcons.Bell;

        public NotificationView()
        {
            InitializeComponent();
            notificacions = Getnotificacions();
            this.BindingContext = this;
        }
        public ObservableCollection<Notificacion> notificacions { get; set; }

        private ObservableCollection<Notificacion> Getnotificacions()
        {
            return new ObservableCollection<Notificacion>
            {
                new Notificacion { Title = "Nueva tarea", Description = "Tarea Pendiente" ,Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo tema",  Description = "Nuevo tema desbloqueado",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"},
                new Notificacion { Title = "Nuevo logro", Description = "Obtuviste un nuevo logro",Image="logo_soe_fill.png"}
            };
        }

    }
}