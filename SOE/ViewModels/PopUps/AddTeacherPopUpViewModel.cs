using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using dotMorten.Xamarin.Forms;
using Kit;
using Kit.Forms.Model;
using Kit.Services.Interfaces;
using SOE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class AddTeacherPopUpViewModel : ValidationsModelbase
    {
        private string _TeacherName;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar un nombre")]
        [Display(Name = "Nombre")]
        public string TeacherName
        {
            get => _TeacherName;
            set => RaiseIfChanged(ref this._TeacherName, value);
        }
        public ICommand TeacherNameChangedCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AutoSuggestBoxTextChangedCommand { get; set; }
        public ICommand AutoSuggestBoxQuerySubmittedCommand { get; set; }
        public ICommand AutoSuggestBoxSuggestionChosenCommand { get; set; }
        public List<string> Suggestions { get; set; }
        public List<Teacher> Teachers { get; set; }
        private readonly AutoSuggestBox AutoSuggestBox;
        private readonly ICrossWindow Window;
        public AddTeacherPopUpViewModel(AutoSuggestBox autoSuggestBox, ICrossWindow window)
        {
            this.AutoSuggestBox = autoSuggestBox;
            this.Window = window;
            TeacherNameChangedCommand = new Command<string>(TeacherNameChanged);
            ConfirmCommand = new Command(Confirm);
            CancelCommand = new Command(Cancel);
            AutoSuggestBoxTextChangedCommand = new AsyncCommand<AutoSuggestBoxTextChangedEventArgs>(AutoSuggestBoxTextChanged);
            AutoSuggestBoxQuerySubmittedCommand = new Command<AutoSuggestBoxQuerySubmittedEventArgs>(AutoSuggestBoxQuerySubmitted);
            AutoSuggestBoxSuggestionChosenCommand = new Command<AutoSuggestBoxSuggestionChosenEventArgs>(AutoSuggestBoxSuggestionChosen);
            this.Init().SafeFireAndForget();
        }

        private async Task Init()
        {
            await Task.Yield();
            Teachers = (await Teacher.GetAll()).ToList();
        }

        private void AutoSuggestBoxSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (this.Suggestions.Any())
            {
                // Set sender.Text. You can use args.SelectedItem to build your text string.
                this.TeacherName = this.Suggestions.Find(x => x == e.SelectedItem.ToString());
            }
        }
        private void AutoSuggestBoxQuerySubmitted(AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion != null && this.Suggestions.Any())
            {
                // User selected an item from the suggestion list, take an action on it here.
                this.TeacherName = this.Suggestions.Find(x => x == e.ChosenSuggestion.ToString());
            }
            else
            {
                // User hit Enter from the search box. Use args.QueryText to determine what to do.
            }
        }
        private async Task AutoSuggestBoxTextChanged(AutoSuggestBoxTextChangedEventArgs e)
        {
            await Task.Yield();
            if (e.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                return;
            string search = this.AutoSuggestBox.Text ?? string.Empty;
            if (search.EndsWith('\n'))
            {
                this.AutoSuggestBox.Text = search.Replace("\n", "");
                this.ConfirmCommand.Execute(this);
            }


            if (this.Teachers.Any())
            {
                //Set the ItemsSource to be your filtered dataset
                search = search.Trim();
                if (string.IsNullOrEmpty(search))
                {
                    return;
                }
                this.Suggestions = Teachers
                    .Where(x => x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Name)
                    .ToList();
                this.AutoSuggestBox.ItemsSource = this.Suggestions;
                this.TeacherName = !string.IsNullOrEmpty(search) ? search : string.Empty;
                //List<string> suggestions = string.IsNullOrEmpty(search)
                //    ? this.TeachersNames
                //    : this.TeachersNames.Where(x => x.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                //        .ToList();
                //this.AutoSuggestBox.ItemsSource = suggestions;
            }
        }
        private void Confirm()
        {
            this.TeacherName = this.AutoSuggestBox.Text?.Trim() ?? string.Empty;
            Window.Close().SafeFireAndForget();
        }

        private void Cancel()
        {
            TeacherName = string.Empty;
            Window.Close().SafeFireAndForget();
        }

        private void TeacherNameChanged(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                TeacherName = name.Trim();
            }
        }
    }
}
