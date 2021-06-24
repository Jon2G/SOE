using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.Views.ViewItems
{
    public class SwipeContainer : ContentView
    {
        public static readonly BindableProperty SwipeCommandProperty =
            BindableProperty.Create(nameof(SwipeCommand), typeof(ICommand), typeof(SwipeContainer), null);
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SwipeContainer), null);

        public ICommand SwipeCommand
        {
            get => (ICommand)GetValue(SwipeCommandProperty);
            set => SetValue(SwipeCommandProperty, value);
        }
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public SwipeContainer()
        {
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Left));
            GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Right));
            //GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Up));
            //GestureRecognizers.Add(GetSwipeGestureRecognizer(SwipeDirection.Down));
        }
        SwipeGestureRecognizer GetSwipeGestureRecognizer(SwipeDirection direction)
        {
            var swipe = new SwipeGestureRecognizer {Direction = direction};
            swipe.Threshold = 15;
            swipe.Swiped += Swipe_Swiped;
            return swipe;
        }

        private void Swipe_Swiped(object sender, SwipedEventArgs e)
        {
            SwipeCommand?.Execute(CommandParameter);
        }
    }
}
