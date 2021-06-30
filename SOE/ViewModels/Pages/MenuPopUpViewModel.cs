using System;
using System.Collections.Generic;
using System.Text;
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
        public string ArchiveText => todo.Archived ? "Desarchivar" : "Archivar";
        public FontImageSource Icon
        {
            get
            {
               var Icon= new FontImageSource()
                {
                    FontFamily = FontelloIcons.Font,
                    Glyph = todo.Archived ? FontelloIcons.Folder : FontelloIcons.Archive
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
