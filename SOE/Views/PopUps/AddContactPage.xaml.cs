using dotMorten.Xamarin.Forms;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.ViewModels.PopUps;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kit;
namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddContactPage
    {
        public AddContactViewModel Model { get; set; }
        private List<Departament> Departaments { get; }
        public AddContactPage(List<Departament> Departaments, SchoolContact contact = null)
        {
            this.Departaments = Departaments;
            this.Model = new AddContactViewModel(this, contact);
            this.BindingContext = Model;
            InitializeComponent();
            this.LockModal();
        }

        public override async Task<BasePopUp> Show()
        {
            ScaleAnimation scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Bottom,
                PositionOut = MoveAnimationOptions.Bottom,
                DurationIn = 100,
                DurationOut = 100,
                HasBackgroundAnimation = false
            };
            this.Animation = scaleAnimation;
            await PopupNavigation.Instance.PushAsync(this, true);
            return this;
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                string search = this.AutoSuggestBox.Text?.Trim();
                List<string> suggestions = string.IsNullOrEmpty(search)
                    ? this.Departaments.Select(x => x.Name).ToList()
                    : this.Departaments.Where(x => x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                        .Select(x => x.Name)
                        .ToList();
                this.AutoSuggestBox.ItemsSource = suggestions;
                this.Model.Departament = !string.IsNullOrEmpty(search) ? 
                    new Departament() { Name = search } : null;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                this.Model.Departament = this.Departaments.Find(x => x.Name == e.ChosenSuggestion.ToString());
            }
            else
            {
                // User hit Enter from the search box. Use args.QueryText to determine what to do.
            }
        }

        private void AutoSuggestBox_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            this.Model.Departament = this.Departaments.Find(x => x.Name == e.SelectedItem.ToString());
        }

    }
}