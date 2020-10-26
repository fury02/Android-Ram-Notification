using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.AppCenter.Crashes;
using Widram.Configuration;
using Widram.Helpers;

namespace Widram.Paiting
{
    [LogRuntime("DrawableIcon.txt", "Widram")]
    public class DrawableIcon<T>
    {
        protected float textSize;

        protected float drawX;
        protected float drawY;

        protected int canvasResourceMaterial;

        protected Color colorText;
        protected Color canvasColor;

        protected Bitmap bmp;
        protected Bitmap mutableBmp;
        protected Canvas canvas;
        protected Paint paint;

        protected Context context = Android.App.Application.Context;
        protected Typeface typefaceDefault;

        internal Settings settings;

        public DrawableIcon()
        {
            try
            {
                settings = Settings.Instance;

                drawY = 1;
                drawX = 1;

                textSize = 10;

                typefaceDefault = Typeface.Create(Typeface.Monospace, TypefaceStyle.Normal);

                canvasResourceMaterial = Resource.Drawable.transparent32x32;
                colorText = Color.White;
                canvasColor = Color.Transparent;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIcon<T>), ex);
                #endregion
            }
            finally { }
        }

        public virtual Icon GenerateIcon(T obj)
        {
            try
            {
                if (obj == null) return null;

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
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIcon<T>), ex);
                #endregion

                return null;
            }
            finally { }
        }

        protected virtual Bitmap DrawableBitmap(object obj)
        {
            try
            {
                string text = obj.ToString();

                bmp = BitmapFactory.DecodeResource(context.Resources, canvasResourceMaterial);
                mutableBmp = bmp.Copy(Bitmap.Config.Argb8888, true);

                canvas = new Canvas(mutableBmp);
                canvas.DrawColor(canvasColor);

                paint = new Paint();
                paint.Color = colorText;
                paint.TextSize = textSize;
                paint.SetTypeface(typefaceDefault);

                canvas.DrawText(text, drawX, drawY, paint);

                return mutableBmp;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DrawableIcon<T>), ex);
                #endregion

                return null;
            }
            finally { }
        }
    }
}