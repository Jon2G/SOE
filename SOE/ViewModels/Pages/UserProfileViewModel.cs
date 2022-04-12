using Kit.Model;
using SOE.Data;
using SOE.Models.Data;

namespace SOE.ViewModels.Pages
{
    public class UserProfileViewModel : ModelBase
    {
        public User User { get; set; }
        public double Progress { get; set; }

        public UserProfileViewModel()
        {
            this.User = AppData.Instance.User;
        }
    }

}
