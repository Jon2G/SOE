using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Data;
using System.Threading.Tasks;

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
            this.Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            this.Credits = await Credits.Get();
            Raise(() => Credits);
            if (Credits is not null)
                this.Progress = Credits.Percentage / 100;
            Raise(() => Progress);
        }
    }

}
