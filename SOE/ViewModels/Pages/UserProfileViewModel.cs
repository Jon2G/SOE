using Kit.Model;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Data;

namespace SOE.ViewModels.Pages
{
    public class UserProfileViewModel : ModelBase
    {
        public Credits Credits { get; set; }
        public User User { get; set; }
        public double Progress { get; set; }

        public UserProfileViewModel()
        {
            this.User = AppData.Instance.User;
            this.Credits =
                AppData.Instance.LiteConnection.Table<Credits>().FirstOrDefault();
            if (Credits is not null)
                this.Progress = Credits.Percentage / 100;
        }
    }

}
