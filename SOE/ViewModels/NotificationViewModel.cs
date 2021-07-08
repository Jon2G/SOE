using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
        public NotificationViewModel()
        {
            Notificacions = new ObservableCollection<Notificacion>();
            Load().SafeFireAndForget();
        }
        private async Task Load()
        {
            await Task.Yield();
            Notificacions.Add(new Notificacion("Nueva tarea", "Tarea Pendiente"));
            Notificacions.Add(new Notificacion("Nuevo tema", "Nuevo tema desbloqueado"));
            for (int i = 0; i < 10; i++)
            {
                Notificacions.Add(new Notificacion("Nuevo logro", "Obtuviste un nuevo logro",false));
            }
        }
    }
}
