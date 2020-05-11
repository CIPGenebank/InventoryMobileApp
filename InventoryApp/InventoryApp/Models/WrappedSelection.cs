using Prism.Mvvm;
using Xamarin.Forms;

namespace InventoryApp.Models
{
    public class WrappedSelection<T> : BindableBase
    {
        private T _item;
        public T Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }


        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value, () => RaisePropertyChanged(nameof(SelectedColor))); }
        }

        public Color SelectedColor
        {
            get
            {
                if (IsSelected)
                {
                    //return Color.FromRgb(205, 220, 57);
                    return Color.FromHex("#eeeeee");
                }
                else
                    return Color.Transparent;
            }
        }

    }
}
