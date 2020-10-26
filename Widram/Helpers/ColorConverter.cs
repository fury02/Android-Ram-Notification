using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Widram.Helpers
{
    public class ColorConverter
    {

        public static Color GetColorByString(string clr)
        {
            switch (clr)
            {
                case "ForestGreen":
                    return Color.ForestGreen;
                case "Transparent":
                    return Color.Transparent;

                default:
                    return Color.Lavender;          
            }
        }

    }
}