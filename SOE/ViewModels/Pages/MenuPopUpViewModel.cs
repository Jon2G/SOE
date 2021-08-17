using System.Windows.Input;
using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Fonts;
using SOE.Models.TaskFirst;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class MenuPopUpViewModel : ModelBase
    {
        private readonly MenuPopUp PopUp;
        private readonly ToDo todo;
        public MenuPopUpViewModel(MenuPopUp PopUp,ToDo todo)
        {
            this.todo = todo;
            this.PopUp = PopUp;
        }
        private ICommand _TapedCommand;
        public ICommand TapedCommand => _TapedCommand ??= new Command<string>(Tapped);
        public string Action { get; private set; }

        public string ArchiveText
        {
            get
            {
                return todo.Status.HasFlag(Enums.PendingStatus.Archived) ? "Desarchivar" : "Archivar";
            }
        }

        public string DoneText
        {
            get
            {
                if (this.todo.Status== Enums.PendingStatus.Pending)
                {
                    return  "Marcar como completado";
                }

                return "Pendiente";
            }
        }
        public FontImageSource PendingIcon
        {
            get
            {
                var Icon = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = todo.Status.HasFlag(Enums.PendingStatus.Pending) ? FontelloIcons.Hourglass : FontelloIcons.CheckBox
                };
                Icon.SetOnAppTheme(FontImageSource.ColorProperty, Color.Black, Color.White);
                return Icon;

            }
        }
        public FontImageSource ArchivedIcon
        {
            get
            {
                var Icon = new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = todo.Status.HasFlag(Enums.PendingStatus.Archived) ? FontelloIcons.Folder : FontelloIcons.Archive
                };
                Icon.SetOnAppTheme(FontImageSource.ColorProperty, Color.Black, Color.White);

                return Icon;
            }
        }
        private void Tapped(string Action)
        {
            this.Action = Action;
            PopUp.Close().SafeFireAndForget();

        }

    }
}
