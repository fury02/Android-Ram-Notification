using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Google.Android.Material.Button;

using Xamarin.Essentials;

using Widram.Configuration;
using Widram.Helpers;
using Widram.Service;

using Settings = Widram.Configuration.Settings;
using Microsoft.AppCenter.Crashes;

namespace Widram.Sheet.Fragment
{
    [LogRuntime("HomeFragment.txt", "Widram")]
    public class HomeFragment : AndroidX.Fragment.App.Fragment
    {
        private Settings settings;
        private CrudSettings crudSettings;

        private Context context = Android.App.Application.Context;

        private Intent serviceIntentStart;
        private Intent serviceIntentStop;

        private MaterialButton buttonOk;
        private MaterialButton buttonCancel;

        public static HomeFragment NewInstance()
        {
            var homeFragment = new HomeFragment { Arguments = new Bundle() };
            return homeFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            serviceIntentStart = new Intent(context, typeof(NotifyService));
            serviceIntentStart.SetAction(Info.IntentActionStartService);

            serviceIntentStop = new Intent(context, typeof(NotifyService));
            serviceIntentStop.SetAction(Info.IntentActionStopService);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.content_home, null);

            try
            {
                crudSettings = new CrudSettings();
                settings = Settings.Instance;

                buttonOk = view.FindViewById<MaterialButton>(Resource.Id.materialButton1);
                buttonOk.Click += HandlerClickButtonOk;

                buttonCancel = view.FindViewById<MaterialButton>(Resource.Id.materialButton2);
                buttonCancel.Click += HandlerClickButtonCancel;

                return view;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(HomeFragment), ex);
                #endregion

                return view;
            }
        }

        async void HandlerClickButtonOk(object sender, EventArgs e)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(serviceIntentStart);
            }
            else
            {
                context.StartService(serviceIntentStart);
            }

            settings.IsAutoStartWhenRebootOS = true;
            crudSettings.Write(settings);
        }

        async void HandlerClickButtonCancel(object sender, EventArgs e)
        {
            bool result = context.StopService(serviceIntentStop);

            settings.IsAutoStartWhenRebootOS = false;
            crudSettings.Write(settings);
        }
    }
}