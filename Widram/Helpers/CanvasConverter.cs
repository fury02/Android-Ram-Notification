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

namespace Widram.Helpers
{
   public class CanvasConverter
    {
        public static int GetResourceDrawableInt(string material)
        {
            switch (material)
            {
                case "transparent32x32":
                    return Resource.Drawable.transparent32x32;
                case "transparent48x32":
                    return Resource.Drawable.transparent48x32;
                case "transparent64x32":
                    return Resource.Drawable.transparent64x32;
                case "transparent96x32":
                    return Resource.Drawable.transparent96x32;
                case "transparent128x32":
                    return Resource.Drawable.transparent128x32;
                case "transparent32x128":
                    return Resource.Drawable.transparent32x128;

                default:
                    return Resource.Drawable.transparent32x32; 
            }
        }
    }
}