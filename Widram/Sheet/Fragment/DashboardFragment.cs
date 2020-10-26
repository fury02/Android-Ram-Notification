using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.SE.Omapi;
using Android.Service.Autofill;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using AntonKozyriatskyi.CircularProgressIndicatorLib;
using Google.Android.Material.Button;
using Java.IO;
using Java.Lang;
using Javax.Crypto.Spec;
using Microsoft.AppCenter.Crashes;
using Mono;
using Widram.Helpers;
using Widram.Configuration;
using Widram.Model;
using Xamarin.Essentials;
using static Android.App.ActivityManager;

namespace Widram.Sheet.Fragment
{
    [LogRuntime("DashboardFragment.txt", "Widram")]
    public class DashboardFragment : AndroidX.Fragment.App.Fragment
    {
        private const string ErrorsException = "Errors";
        private const string UriProVersion = "https://play.google.com/store/apps/details?id=com.arbuz.widram_pro";

        private Context context = Android.App.Application.Context;

        private Settings settings;

        #region Popup Dialog
        private Dialog popupDialog;
        private Button buttonOkPopup;
        private Button buttonCancelPopup;
        #endregion

        private MaterialButton buttonOk;
        private MaterialButton buttonCancel;

        private CircularProgressIndicator circularProgress;
        private CircularProgressIndicator circularProgressCpu;
        private Android.Widget.Switch switchTest;
        private Android.Widget.LinearLayout linearLayout;

        PartLoadInfo partLoadInfo;

        private View view;

        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private List<int> listIntRandom = new List<int>();

        private static Random random = new Random();


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static DashboardFragment NewInstance()
        {
            var dashboardFragment = new DashboardFragment { Arguments = new Bundle() };
            return dashboardFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(Resource.Layout.content_dashboard, null);

            settings = Settings.Instance;

            try
            {
                #region Popup Dialog   

                var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.layout_popup);

                var layoutInflater = LayoutInflater.FromContext(context);
                var viewPopup = layoutInflater.Inflate(remoteViews.LayoutId, null);

                popupDialog = new Dialog(Platform.CurrentActivity);
                popupDialog.SetContentView(viewPopup);
                popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);

                var textView = viewPopup.FindViewById<TextView>(Resource.Id.textView1);

                buttonOkPopup = viewPopup.FindViewById<Button>(Resource.Id.button1);
                buttonOkPopup.Click += ButtonOk_Popup_Click;

                buttonCancelPopup = viewPopup.FindViewById<Button>(Resource.Id.button2);
                buttonCancelPopup.Click += ButtonCancel_Popup_Click;

                #endregion

                buttonOk = view.FindViewById<MaterialButton>(Resource.Id.materialButton1);
                buttonOk.Click += async delegate (object sender, EventArgs e) { await ButtonOk_Click(sender, e); };

                buttonCancel = view.FindViewById<MaterialButton>(Resource.Id.materialButton2);
                buttonCancel.Click += async delegate (object sender, EventArgs e) { await ButtonCancel_Click(sender, e); };

                circularProgress = view.FindViewById<CircularProgressIndicator>(Resource.Id.circular_progress);
                circularProgressCpu = view.FindViewById<CircularProgressIndicator>(Resource.Id.circular_progress_cpu);
                switchTest = view.FindViewById<Android.Widget.Switch>(Resource.Id.switch1);
                linearLayout = view.FindViewById<Android.Widget.LinearLayout>(Resource.Id.bt_linearlayout);

                if (settings.ProVersion == false)
                {
                    switchTest.Touch += SwitchTest_Touch;
                }
                else
                {
                    switchTest.CheckedChange += SwitchTest_CheckedChange;
                }

                Task.Run(() => ProgressIndicatorUpdate());

                cancellationToken = new CancellationTokenSource();

                return view;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DashboardFragment), ex);
                #endregion

                return view;
            }
            finally { }            
        }

        private void SwitchTest_Touch(object sender, View.TouchEventArgs e)
        {
            ShowPopup();
        }

        private void SwitchTest_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (linearLayout.Visibility == ViewStates.Gone)
            {
                linearLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                linearLayout.Visibility = ViewStates.Gone;
            }
        }

        private async void ProgressIndicatorUpdate()
        {
            partLoadInfo = new PartLoadInfo();
            partLoadInfo.UpdateContext();

            double cpuUsage = await partLoadInfo.AverageCpuUsage();

            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(
                      delegate
                      {
                          circularProgress.SetProgress((int)partLoadInfo.UsedRamMB, (int)partLoadInfo.TotalRamMB);
                          circularProgressCpu.SetProgress((int)cpuUsage, 100);
                      });
        }

        #region Popup Dialog   
        private void ShowPopup()
        {
            popupDialog.Show();
        }

        private void ButtonCancel_Popup_Click(object sender, EventArgs e)
        {
            popupDialog.Dismiss();
            popupDialog.Hide();
        }

        private void ButtonOk_Popup_Click(object sender, EventArgs e)
        {
            try
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse(UriProVersion);
                Intent intent = new Intent(Android.Content.Intent.ActionView, uri);
                StartActivity(intent);

                popupDialog.Dismiss();
                popupDialog.Hide();
            }
            catch (System.Exception ex)
            {
                popupDialog.Dismiss();
                popupDialog.Hide();

                Toast.MakeText(Application.Context, ErrorsException, ToastLength.Short).Show();
            }

        }
        #endregion

        private async Task ButtonCancel_Click(object sender, EventArgs e)
        {
            cancellationToken.Cancel();
        }

        private async Task ButtonOk_Click(object sender, EventArgs e)
        {
            cancellationToken = new CancellationTokenSource();

            Task.Run(() => FillRamView(cancellationToken.Token));
            Task.Run(() => FillRam(cancellationToken.Token));
        }


        private void FillRamView(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(500);

                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(
                        delegate
                        {
                            partLoadInfo = new PartLoadInfo();
                            partLoadInfo.UpdateContext();

                            if (partLoadInfo.PercentUsed >= 75)
                            {
                                circularProgress.ProgressColor = Color.DarkRed;
                            }
                            if (partLoadInfo.PercentUsed >= 85)
                            {
                                //cancellationToken.Cancel();
                            }
                            else
                            {
                                circularProgress.ProgressColor = Color.DarkGreen;
                            }

                            ProgressIndicatorUpdate();
                        });

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(DashboardFragment), ex);
                #endregion

            }
            finally { }
        }

        private void FillRam(CancellationToken token)
        {
            while (true)
            {
                listIntRandom.Add(random.Next(int.MinValue, int.MaxValue));

                if (token.IsCancellationRequested)
                {
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(
                     delegate
                     {
                         int generationHeap = GC.GetGeneration(listIntRandom);
                         GC.Collect(generationHeap, GCCollectionMode.Forced);
                     });

                    break;
                }
            }
        }
    }
}
