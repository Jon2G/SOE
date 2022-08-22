using dotMorten.Xamarin.Forms;
using SOE.ViewModels.PopUps;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTeacherPopUp
    {
        public AddTeacherPopUpViewModel ViewModel { get; set; }
        public AddTeacherPopUp()
        {
            InitializeComponent();
            ViewModel = new AddTeacherPopUpViewModel(this.AutoSuggestBox, this);
            this.BindingContext = this.ViewModel;
            Task.Delay(100).ContinueWith(t =>
            {
                Dispatcher.BeginInvokeOnMainThread(() => this.AutoSuggestBox.Focus());
            });
        }

        private void AutoSuggestBox_OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e) => this.ViewModel.AutoSuggestBoxTextChangedCommand.Execute(e);

        private void AutoSuggestBox_OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
            => this.ViewModel.AutoSuggestBoxQuerySubmittedCommand.Execute(e);

        private void AutoSuggestBox_OnSuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        => this.ViewModel.AutoSuggestBoxSuggestionChosenCommand.Execute(e);
    }
}