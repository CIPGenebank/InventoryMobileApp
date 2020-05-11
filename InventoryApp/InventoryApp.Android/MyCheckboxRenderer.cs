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
using InventoryApp;
using InventoryApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyCheckbox), typeof(MyCheckboxRenderer))]
namespace InventoryApp.Droid
{
    public class MyCheckboxRenderer : CheckBoxRenderer
    {
        public MyCheckboxRenderer(Context context) : base(context)
        {
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (Element.InputTransparent)
            {
                return false;
            }
            return base.DispatchTouchEvent(e);
        }
    }
}