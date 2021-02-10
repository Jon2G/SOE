using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OrganizadorEscolar.Widgets.Horario;
using OrganizadorEscolar.Widgets.Tareas;
using Xamarin.Forms;

namespace OrganizadorEscolar.Droid.Widgets.Horario
{
    public class WidgetHorario : OrganizadorEscolar.Widgets.Horario.WidgetHorario
    {
        private MainActivity MainActivity;
        internal WidgetHorario(MainActivity MainActivity) : base()
        {
            this.MainActivity = MainActivity;
        }
        public static OrganizadorEscolar.Widgets.Horario.WidgetHorario Init(MainActivity MainActivity)
        {
            if (Instance is null)
            {
                Instance = new WidgetHorario(MainActivity);
            }
            ((WidgetHorario)Instance).MainActivity = MainActivity;
            return Instance;
        }
        public override bool IsItentDataSet()
        {
            if (this.MainActivity is null)
            {
                return false;
            }
            int id = MainActivity.Intent.GetIntExtra(AppWidget.EXTRA_ITEM, -1);
            return id != -1;
        }
        public override OrganizadorEscolar.Widgets.Horario.IntentData GetIntentData()
        {
            if (this.MainActivity is null)
            {
                return new OrganizadorEscolar.Widgets.Horario.IntentData(-1, string.Empty);
            }
            return new OrganizadorEscolar.Widgets.Horario.IntentData(
                MainActivity.Intent.GetIntExtra(AppWidget.EXTRA_ITEM, -1),
                MainActivity.Intent.GetStringExtra(AppWidget.EXTRA_LABEL)
                );
        }


        //public IWidget Initializte()
        //{

        //    // RemoteViews updateViews = new RemoteViews(this.MainActivity.PackageName, Resource.Layout.WidgetHorario);

        //    LayoutInflater factory = this.MainActivity.LayoutInflater;
        //    Android.Views.View view = factory.Inflate(Resource.Layout.WidgetHorario, null);
        //    //this.ListViewMaterias = view.FindViewById<Android.Widget.ListView>(Resource.Id.listViewMaterias);

        //    ////landmarkEditNameView = (EditText)textEntryView.findViewById(R.id.landmark_name_dialog_edit);

        //    //mListView = (Android.Widget.ListView)this.MainActivity.FindViewById(Resource.Id.listViewMaterias);
        //    return this;
        //}
        public override OrganizadorEscolar.Widgets.Horario.WidgetHorario Refresh()
        {
            //List<TodoModel> items = new List<TodoModel>();
            //items.Add(new TodoModel(0, "a"));
            //items.Add(new TodoModel(1, "a"));
            //items.Add(new TodoModel(2, "a"));
            //items.Add(new TodoModel(3, "a"));

            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    mTodos = items;

            //    mAdapter = new TodosAdapter(this.Context, mTodos);
            //    if (ListViewMaterias != null)
            //    {
            //        ListViewMaterias.SetAdapter(mAdapter);
            //    }

            //    AppWidget.SendRefreshBroadcast(this.Context);
            //});

            return this;
        }


    }
}