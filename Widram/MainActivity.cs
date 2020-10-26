using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Views;

using Google.Android.Material.BottomNavigation;

using Xamarin.Essentials;

using Widram.Service;
using Widram.Helpers;
using Widram.Receivers;
using Widram.Sheet.Fragment;
using Widram.Configuration;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

namespace Widram
{
    [Activity(Label = "Widram", Theme = "@style/AppTheme", MainLauncher = true)]
    [LogRuntime("MainActivity.txt", "Widram")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private Settings settings;

        private Context context = Android.App.Application.Context;

        private string AppCenterId = Info.AppCenterId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);

                SetContentView(Resource.Layout.content_main);

                #region Fragment navigation 

                BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
                navigation?.SetOnNavigationItemSelectedListener(this);

                SupportFragmentManager.BeginTransaction()
                                       .Replace(Resource.Id.content_frame, new HomeFragment())
                                       .Commit();

                #endregion

                #region Settings

                settings = Settings.Instance;

                #endregion

                #region App Center Crash (http: // appcenter.ms )

                if (settings.IsSendCrashes == true)
                {
                    AppCenter.Start(AppCenterId, new Type[] { typeof(Crashes) });

                    Crashes.SendingErrorReport += (sender, e) => CrashesSendingErrorReport(sender, e);
                    
                    Crashes.ShouldAwaitUserConfirmation = () =>
                    {
                        AndroidX.AppCompat.App.AlertDialog.Builder alert = new AndroidX.AppCompat.App.AlertDialog.Builder(Platform.CurrentActivity);
                        alert.SetTitle("Confirm send");
                        alert.SetMessage("Send anonymous data about crashes in the app?");

                        alert.SetPositiveButton("Send", (senderAlert, args) =>
                        {
                            UserConfirmationDialog(UserConfirmation.Send);

                            Toast.MakeText(context, "Send", ToastLength.Short).Show();
                        });

                        alert.SetNegativeButton("Cancel", (senderAlert, args) => 
                        {
                            UserConfirmationDialog(UserConfirmation.DontSend);

                            Toast.MakeText(context, "Not Send", ToastLength.Short).Show();
                        });

                        Dialog dialog = alert.Create();
                        dialog.Show();

                        return true;
                    };
                }              

                #endregion

                if (settings.IsLogErrorStorage == true)
                {
                    Task.Run(() => PermissionsCheckRun());
                }
            }
            catch (Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(MainActivity), ex);
                #endregion              
            }
        }

        #region App Center Crashes
        private void UserConfirmationDialog(UserConfirmation selectionButton)
        {
            Crashes.NotifyUserConfirmation(selectionButton);
        }

        private void CrashesSendingErrorReport(Object sender, SendingErrorReportEventArgs e)
        {


        }
        #endregion

        private async Task PermissionsCheckRun()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status != PermissionStatus.Granted)
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async delegate
                {
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                });
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
 
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region Fragment navigation 

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:

                    SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.content_frame, new HomeFragment())
                                           .Commit();
                    break;

                case Resource.Id.navigation_dashboard:

                    SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.content_frame, new DashboardFragment())
                                           .Commit();
                    break;

                case Resource.Id.navigation_settings:

                    SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.content_frame, new SettingsFragment())
                                           .Commit();
                    break;

                default:

                    SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.content_frame, new HomeFragment())
                                           .Commit();
                    return false;
            }

            return true;
        }

        #endregion
        
    }
}