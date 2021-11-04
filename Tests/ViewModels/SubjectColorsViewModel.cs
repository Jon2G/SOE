using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.ViewModels
{
    public class SubjectColorsViewModel
    {
        public ObservableCollection<AppColor> Colors { get; set; }

        public SubjectColorsViewModel()
        {
            this.Colors = new ObservableCollection<AppColor>();
        }

        public async Task Init()
        {
            await Task.Yield();
            AppColor.GetAll()?.ForEach(this.Colors.Add);
        }

    }
}
