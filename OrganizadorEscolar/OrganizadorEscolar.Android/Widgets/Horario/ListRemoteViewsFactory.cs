using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using static Android.Widget.RemoteViewsService;
using Exception = Java.Lang.Exception;
using Math = Java.Lang.Math;

namespace OrganizadorEscolar.Droid.Widgets.Horario
{
    /// <summary>
    /// RemoteViewsFactory serves the purpose of an adapter in the widget’s context.
    /// An adapter is used to connect the collection items(for example, ListView items or GridView items) with the data set.
    /// </summary>
    public class ListRemoteViewsFactory : Java.Lang.Object, IRemoteViewsFactory
    {
        //private List<OrganizadorEscolar.Widgets.Horario.Materia> Materias;
        private Context mContext;
        private int mAppWidgetId;
        /// <summary>
        /// returns the number of records in the cursor.
        /// (In our case, the number of task items that need to be displayed in the app widget)
        /// </summary>
        public int Count
        {
            get
            {
                if (OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance is null)
                {
                    return 0;
                }
                return OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance.Materias.Count;
            }
        }
        /// <summary>
        ///  Indicates whether the item ids are stable across changes to the underlying data. 
        ///  It returns True if the same id always refers to the same object.
        /// </summary>
        public bool HasStableIds => true;

        public RemoteViews LoadingView => GetLoadingView();

        /// <summary>
        /// allows for the use of a custom loading view which appears between 
        /// the time that getViewAt(int) is called and returns.
        /// </summary>
        /// <returns></returns>
        private RemoteViews GetLoadingView()
        {
            // You can create a custom loading view (for instance when getViewAt() is slow.) If you
            // return null here, you will get the default loading view.
            return null;
        }
        /// <summary>
        ///  Returns the number of types of views we have in ListView.
        ///  In our case, we have same view type for each ListView item so we return 1 there.
        /// </summary>
        public int ViewTypeCount => 1;

        public JniManagedPeerStates JniManagedPeerState => throw new NotImplementedException();

        public ListRemoteViewsFactory(Context context, Intent intent)
        {
            //Materias = new List<OrganizadorEscolar.Widgets.Horario.Materia>();
            mContext = context;
            mAppWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId,
                    AppWidgetManager.InvalidAppwidgetId);
        }
        // Initialize the data set.
        /// <summary>
        ///In onCreate() you setup any connections / cursors to your data source.
        ///Heavy lifting, for example downloading or creating content etc, should 
        ///be deferred to onDataSetChanged() or getViewAt(). 
        ///Taking more than 20 seconds in this call will result in an ANR.
        ///Description:
        ///Is called by the system when creating your factory for the first time. This is where you set up any connections and/or cursors to your data source.
        /// </summary>
        public void OnCreate()
        {


            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("14:30 - 16:00", "Ingenieria de Software", "#b9abff"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("16:00 - 17:30", "Sistemas Operativos", "#acf2c8"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("17:30 - 19:00", "Modulacion Digital", "#ede19f"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("19:00 - 20:30", "Bases de datos", "#9dbfeb"));
            //Materias.Add(new OrganizadorEscolar.Widgets.Horario.Materia("20:30 - 22:00", "Arquitectura de computadoras", "#de998e"));

            //// We sleep for 3 seconds here to show how the empty view appears in the interim.
            //// The empty view is set in the ListWidgetProvider and should be a sibling of the
            //// collection view.
            //try
            //{
            //    Thread.Sleep(3000);
            //}
            //catch (InterruptedException e)
            //{
            //    e.PrintStackTrace();
            //}
        }
        /// <summary>
        /// In onDestroy() you should tear down anything that was setup for your data source,
        /// eg. cursors, connections, etc.
        /// Description:
        /// Is called when the last RemoteViewsAdapter that is associated with this 
        /// factory is unbound. Here you should tear down anything that was set up for
        /// your data source eg. cursors, connections, etc.
        /// </summary>
        public void OnDestroy()
        {
            OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance?.Materias.Clear();
        }
        /// <summary>
        /// Given the position (index) of a WidgetItem in the array,
        /// use the item's text value in
        /// combination with the app widget item XML file to construct
        /// a RemoteViews object.
        /// Description:
        ///  handles all the processing work. 
        ///  It returns a RemoteViews object which in our case is the single list item.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public RemoteViews GetViewAt(int position)
        {



            // position will always range from 0 to getCount() - 1.
            // construct a remote views item based on our widget item xml file, and set the
            // text based on the position.
            RemoteViews rv = new RemoteViews(mContext.PackageName, Resource.Layout.widget_item);
            rv.SetTextViewText(Resource.Id.widget_item_horario_materia, OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance.Materias[position].NombreMateria);
            rv.SetTextViewText(Resource.Id.widget_item_horario_horario, OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance.Materias[position].Horario);

            Color color = Color.ParseColor(OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance.Materias[position].Color);
            rv.SetInt(Resource.Id.widget_item_horario, "setBackgroundColor", color);

            // Next, we set a fill-intent which will be used to fill-in the pending intent template
            // which is set on the collection view in ListWidgetProvider.
            Bundle extras = new Bundle();
            extras.PutInt(AppWidget.EXTRA_ITEM, position);
            extras.PutString(AppWidget.EXTRA_LABEL, OrganizadorEscolar.Widgets.Horario.WidgetHorario.Instance.Materias[position].NombreMateria);

            Intent fillInIntent = new Intent();
            fillInIntent.PutExtras(extras);
            // Make it possible to distinguish the individual on-click
            // action of a given item
            rv.SetOnClickFillInIntent(Resource.Id.widget_item_horario, fillInIntent);

            // You can do heaving lifting in here, synchronously. For example, if you need to
            // process an image, fetch something from the network, etc., it is ok to do it here,
            // synchronously. A loading view will show up in lieu of the actual contents in the
            // interim.
            //try
            //{
            //    JavaSystem.Out.Println("Loading view " + position);
            //    Thread.Sleep(500);
            //}
            //catch (InterruptedException e)
            //{
            //    e.PrintStackTrace();
            //}

            // Return the remote views object.
            return rv;
        }


        public static Bitmap GetBackground(Color bgColor, int width, int height, Context context)
        {
            try
            {
                // convert to HSV to lighten and darken
                int alpha = Color.GetAlphaComponent(bgColor);
                float[] hsv = new float[3];
                Color.ColorToHSV(bgColor, hsv);
                hsv[2] -= .1f;
                int darker = Color.HSVToColor(alpha, hsv);
                hsv[2] += .3f;
                int lighter = Color.HSVToColor(alpha, hsv);

                // create gradient useng lighter and darker colors
                GradientDrawable gd = new GradientDrawable(
                        GradientDrawable.Orientation.LeftRight, new int[] { darker, lighter });
                gd.SetGradientType(GradientType.LinearGradient);
                // set corner size
                gd.SetCornerRadii(new float[] { 4, 4, 4, 4, 4, 4, 4, 4 });

                // get density to scale bitmap for device
                float dp = context.Resources.DisplayMetrics.Density;

                // create bitmap based on width and height of widget
                Bitmap bitmap = Bitmap.CreateBitmap(Math.Round(width * dp), Math.Round(height * dp),
                        Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                gd.SetBounds(0, 0, canvas.Width, canvas.Height);
                gd.Draw(canvas);
                return bitmap;
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                return null;
            }
        }


        /// <summary>
        ///  Gets the row id associated with the specified position in the list.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public long GetItemId(int position)
        {
            return position;
        }
        /// <summary>
        ///Description:
        /// Is called when notifyDataSetChanged() is triggered on the remote adapter.
        /// </summary>
        public void OnDataSetChanged()
        {
            // This is triggered when you call AppWidgetManager notifyAppWidgetViewDataChanged
            // on the collection view corresponding to this factory. You can do heaving lifting in
            // here, synchronously. For example, if you need to process an image, fetch something
            // from the network, etc., it is ok to do it here, synchronously. The widget will remain
            // in its current state while work is being done here, so you don't need to worry about
            // locking up the widget.
        }


        public void Disposed()
        {
            //throw new NotImplementedException();
        }

        public void DisposeUnlessReferenced()
        {
            //throw new NotImplementedException();
        }

        public void Finalized()
        {
            //throw new NotImplementedException();
        }

        public void SetJniIdentityHashCode(int value)
        {
            // throw new NotImplementedException();
        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {
            // throw new NotImplementedException();
        }

        public void SetPeerReference(JniObjectReference reference)
        {
            //   throw new NotImplementedException();
        }


    }
}