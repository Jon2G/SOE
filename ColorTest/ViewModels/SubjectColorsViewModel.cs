using ColorTest.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorTest.ViewModels
{
    public class SubjectColorsViewModel
    {
        public ObservableCollection<AppColor> Colors { get; set; }

        public SubjectColorsViewModel()
        {
            Colors = new ObservableCollection<AppColor>();
        }

        public async Task Init()
        {
            await Task.Yield();
            AppColor.GetAll()?.ForEach(Colors.Add);
        }

    }
}
