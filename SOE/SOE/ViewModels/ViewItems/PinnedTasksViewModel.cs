using SchoolOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolOrganizer.Enums;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class PinnedTasksViewModel
    {
        public List<Card> Tasks { get; set; }
        public PinnedTasksViewModel()
        {
            Tasks = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                Tasks.Add(
                    new Card()
                    {
                        Title = "Resumén Egipto",
                        Status = CardStatus.Alert,
                        Description = "Acerca de la poblacion de egipto.\n Minimo 3 cuartillas.\nFuentes APPA\nA mano\nEnviar al correo en formato pdf junto con el código de latex.",
                        DueDate = DateTime.Now.AddDays(i),
                        StatusMessage = "1 Day left!",
                        StatusMessageFileSource = "Alerte.png",
                        ActionMessage = "Resume",
                        ActionMessageFileSource = "Resume.png",
                        DaysLeft = i+1,
                        SubjectColor = "#e6b566",
                        SubjectName = "Base de datos"
                    });
            }
        }
    }
}
