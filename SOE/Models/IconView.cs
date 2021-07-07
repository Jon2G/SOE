using System;
using System.Collections.Generic;
using System.Text;
using SOE.Fonts;
using Xamarin.Forms;
using ContentView = Forms9Patch.ContentView;

namespace SOE.Models
{
    public class IconView : ContentView
    {
        public virtual string Icon => FontelloIcons.TrashBin;
        public virtual string Title => string.Empty;

        public static readonly BindableProperty ToolbarItemProperty = 
            BindableProperty.Create(
            propertyName: nameof(ToolbarItem),
            returnType: typeof(ToolbarItem), 
            declaringType: typeof(ToolbarItem),
            defaultValue: null);

        public ToolbarItem ToolbarItem
        {
            get => (ToolbarItem)GetValue(ToolbarItemProperty);
            set
            {
                SetValue(ToolbarItemProperty, value);
                OnPropertyChanged();
            }
        }

        public IconView():base()
        {
            
        }
    }
}
