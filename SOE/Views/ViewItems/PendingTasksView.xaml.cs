using AsyncAwaitBestPractices;
using SOE.Data;
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

        public void OnRefreshComplete()
        {
            if ((this.Model?.DayGroups.Any() ?? false) && (AppData.Instance?.User?.Settings?.ExpandCards ?? false))
            {
                Task task = this.Model.DayGroups?.FirstOrDefault()?.ExpandAll(true);
                if (task is not null)
                    Task.Delay(500).ContinueWith((t)=>task).SafeFireAndForget();
            }
            this.CollectionView?.InvalidateMeasureNonVirtual(InvalidationTrigger.RendererReady);
        }
    }
}