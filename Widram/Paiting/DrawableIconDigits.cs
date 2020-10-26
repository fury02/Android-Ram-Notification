using System;

using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.AppCenter.Crashes;
using Widram.Configuration;
using Widram.Helpers;
using Xamarin.Essentials;

namespace Widram.Paiting
{
    [LogRuntime("DrawableIconDigits.txt", "Widram")]
    public class DrawableIconDigits<T> : DrawableIcon<T>
    {
        public DrawableIconDigits() : base()
        {
            try
            {
                settings = Settings.Instance;

                this.typefaceDefault = Typeface.CreateFromAsset(Application.Context.Assets, settings.TypefaceDigits);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIconDigits<T>), ex);
                #endregion
            }
            finally { }
        }

        public override Icon GenerateIcon(T obj)
        {
            try
            {
                if (obj == null) return null;

                settings = Settings.Instance;

                drawY = settings.Ordinate;
                drawX = settings.Abscissa;

                textSize = settings.TextSizeDigits;

                typefaceDefault = Typeface.CreateFromAsset(Application.Context.Assets, settings.TypefaceDigits);

                canvasResourceMaterial = CanvasConverter.GetResourceDrawableInt(settings.CanvasMaterial);
                colorText = ColorConverter.GetColorByString(settings.ColorTextDigits);
                canvasColor = ColorConverter.GetColorByString(settings.CanvasColor);

                bmp = DrawableBitmap(obj);
                Icon icon = Icon.CreateWithBitmap(bmp);

                paint.Dispose();
                canvas.Dispose();
                bmp.Dispose();
                mutableBmp.Dispose();

                return icon;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIconDigits<T>), ex);
                #endregion

                return null;
            }
            finally { }          
        }

        protected override Bitmap DrawableBitmap(object obj)
        {
            string text = string.Empty;
            string toStr = obj.ToString();

            try
            {
                settings = Settings.Instance;

                if (settings.IsRamSelectedNotification == true)
                {
                    if (settings.IsShowRelativeRAM)
                    {
                        text = String.Concat(new string[] { toStr, "%" });
                    }
                    else
                    {
                        text = String.Concat(new string[] { toStr, "Gb" });
                    }
                }
                if (settings.IsCpuSelectedNotification == true)
                {
                    text = String.Concat(new string[] { toStr, "%" });
                }
               
                return base.DrawableBitmap(text);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIconDigits<T>), ex);
                #endregion

                return base.DrawableBitmap(text);
            }
            finally { }
        }
    }
}