using Android.App;
using Android.Content;
using Microsoft.AppCenter.Crashes;

namespace Widram.Helpers
{
    [LogRuntime("HearServices.txt", "Widram")]
    public static class HearServices
    {
        public static bool IsServiceRunning(System.Type typeIntentClass, Context context)
        {
            try
            {
                ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);

                foreach (var service in manager.GetRunningServices(int.MaxValue))
                {
                    if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(typeIntentClass).CanonicalName))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(HearServices), ex);
                #endregion

                return false;
            }
            finally { }           
        }
    }
}