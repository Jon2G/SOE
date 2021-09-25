using AsyncAwaitBestPractices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingTasksView
    {
        public static PendingTasksView Instance { get; private set; }
        public override string Title => "TAREAS";

        public PendingTasksView()
        {
            Instance = this;
            InitializeComponent();
            this.Model.Refresh(OnRefreshComplete);
        }
        public Action OnRefreshCompleteAction => this.OnRefreshComplete;

        public async void OnRefreshComplete()
        {
            await Task.Delay(500);
            if (this.Model.DayGroups.Any())
            {
                await this.Model.DayGroups.First().ExpandAll(true);
            }
            this.CollectionView.InvalidateMeasureNonVirtual(InvalidationTrigger.RendererReady);
        }
    }
}