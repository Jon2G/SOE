using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;

namespace SOE.ViewModels
{
    public class NotificationViewModel : ModelBase
    {
        private ObservableCollection<Notificacion> _Notificacions;
        public ObservableCollection<Notificacion> Notificacions
        {
            get => _Notificacions;
            set
            {
                _Notificacions = value;
                Raise(() => Notificacions);
            }
        }

        private ObservableCollection<Notificacion> _NewNotificacions;

        public ObservableCollection<Notificacion> NewNotificacions
        {
            get => _NewNotificacions;
            set
            {
                _NewNotificacions = value;
                Raise(() => NewNotificacions);
            }
        }
        public NotificationViewModel()
        {
            Notificacions = new ObservableCollection<Notificacion>();
            NewNotificacions = new ObservableCollection<Notificacion>();
            Load().SafeFireAndForget();
        }
        private async Task Load()
        {
            await Task.Yield();
            NewNotificacions.Add(new Notificacion("Nueva tarea", "Tarea Pendiente"));
            NewNotificacions.Add(new Notificacion("Nuevo tema", "Nuevo tema desbloqueado"));
            for (int i = 0; i < 10; i++)
            {
                Notificacions.Add(new Notificacion("Nuevo logro", "Obtuviste un nuevo logro", "logo_moneda.png", false));
            }
        }
    }
}
