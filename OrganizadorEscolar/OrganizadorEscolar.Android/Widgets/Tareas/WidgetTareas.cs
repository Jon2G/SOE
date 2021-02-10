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

namespace OrganizadorEscolar.Droid.Widgets.Tareas
{
    public class WidgetTareas : OrganizadorEscolar.Widgets.Tareas.WidgetTareas
    {
        private MainActivity MainActivity;
        internal WidgetTareas(MainActivity MainActivity) : base()
        {
            this.MainActivity = MainActivity;
        }
        public static OrganizadorEscolar.Widgets.Tareas.WidgetTareas Init(MainActivity MainActivity)
        {
            if (Instance is null)
            {
                Instance = new WidgetTareas(MainActivity);
            }
            ((WidgetTareas)Instance).MainActivity = MainActivity;
            return Instance;
        }
        public override bool IsItentDataSet()
        {
            if (MainActivity is null)
            {
                return false;
            }
            int id = MainActivity.Intent.GetIntExtra(AppWidgetTareas.EXTRA_ITEM, -1);
            return id != -1;
        }
        public override OrganizadorEscolar.Widgets.Tareas.IntentData GetIntentData()
        {
            if (MainActivity is null)
            {
                return new OrganizadorEscolar.Widgets.Tareas.IntentData(-1, string.Empty);
            }
            return new OrganizadorEscolar.Widgets.Tareas.IntentData(
                MainActivity.Intent.GetIntExtra(AppWidgetTareas.EXTRA_ITEM, -1),
                MainActivity.Intent.GetStringExtra(AppWidgetTareas.EXTRA_LABEL)
                );
        }
        public override OrganizadorEscolar.Widgets.Tareas.WidgetTareas Refresh()
        {

            return this;
        }


    }
}