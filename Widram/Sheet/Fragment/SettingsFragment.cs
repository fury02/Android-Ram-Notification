using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Microsoft.AppCenter.Crashes;
using Widram.Configuration;
using Widram.Helpers;
using Widram.Service;
using Xamarin.Essentials;

namespace Widram.Sheet.Fragment
{
    [LogRuntime("SettingsFragment.txt", "Widram")]
    public class SettingsFragment : AndroidX.Fragment.App.Fragment
    {         
        private const string SaveSettings = "Save Settings";
        private const string DefaultSettings = "Default Settings";
        private const string DefaultSettingsRun = "Service Running";
        private const string ErrorsException = "Errors";

        static int touchSpeenerClick = 0;

        private const string UriProVersion = "https://play.google.com/store/apps/details?id=com.arbuz.widram_pro";

        #region Popup Dialog
        private Dialog popupDialog;
        private Button buttonOkPopup;
        private Button buttonCancelPopup;
        #endregion

        private Context context = Android.App.Application.Context;
        private Settings settings;
        private CrudSettings crudSettings;

        private ArrayAdapter adapterMaterial;
        private ArrayAdapter adapterColor;
        private ArrayAdapter adapter;

        private List<string> listAdaptrer = new List<string>(new string[] { "anton.otf" });
        private List<string> listAdaptrerColor = new List<string>(new string[] {  "ForestGreen", "Transparent" });
        private List<string> listAdaptrerMaterial = new List<string>(new string[] { "transparent32x32", "transparent48x32", "transparent64x32", "transparent96x32", "transparent128x32", "transparent32x128" });

        private PendingIntent updatePendingIntent;
        private Intent updateServiceIntent;

        private MaterialButton materialButtonAppInfo;

        private FloatingActionButton floatingActionButtonSaveSettings;
        private FloatingActionButton floatingActionDefaultSettings;

        RadioGroup radioGroupMain;

        RadioButton radioButtonUseRam;
        RadioButton radioButtonUseCpu;

        private SwitchCompat swithRelativeIsViewRam;
        private SwitchCompat swithShowFreeRam;
        //private SwitchCompat swithLogging;
        //private SwitchCompat swithCrash;
        private SwitchCompat switchCompatUseCpu;
        private SwitchCompat switchAbsoluteIsViewRam;
        private SwitchCompat switchLayoutLotification;

        private Spinner spinnerTypefaceDigits;
        private Spinner spinnerTextColorDigits;
        private Spinner spinnerCanvasColor;
        private Spinner spinnerCanvasMaterial;

        private SeekBar sbSizeDigits;
        private SeekBar sbPositionOrdinate;
        private SeekBar sbPositionAbscissa;
        private SeekBar sbUpdateTimer;

        private TextView tvsbSizeDigits;

        private TextView tvsbTypefaceDigits;
        private TextView tvsbPositionOrdinate;
        private TextView tvsbPositionAbscissa;
        private TextView tvsbUpdateTimer;
        //private TextView tvsbLogging;
        //private TextView tvsbCrash;
        private TextView tvsbSwithIsShowFreeRam;
        private TextView tvsbSwithRelativeIsViewRam;

        private AppCompatTextView tvsbSwithIsShowFreeCpu;
        private AppCompatTextView tvsbLayoutLotification;
        private AppCompatTextView tvsbSwithAbsoluteIsViewRam;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static SettingsFragment NewInstance()
        {
            var settingsFragment = new SettingsFragment { Arguments = new Bundle() };

            return settingsFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            #region Init View

            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.content_settings, null);

            #endregion

            try
            {
                crudSettings = new CrudSettings();
                settings = Settings.Instance;

                updateServiceIntent = new Intent();
                updateServiceIntent.SetAction(Info.IntentActionUpdateSettings);
                updatePendingIntent = PendingIntent.GetBroadcast(context, 0, updateServiceIntent, 0);

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

                #region Сontrol element

                materialButtonAppInfo = view.FindViewById<MaterialButton>(Resource.Id.materialButton1);

                floatingActionButtonSaveSettings = view.FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton1);
                floatingActionDefaultSettings = view.FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton2);

                radioGroupMain = view.FindViewById<RadioGroup>(Resource.Id.radioGroup1);

                radioButtonUseRam = view.FindViewById<RadioButton>(Resource.Id.radioButton1);
                radioButtonUseCpu = view.FindViewById<RadioButton>(Resource.Id.radioButton2);

                swithRelativeIsViewRam = view.FindViewById<SwitchCompat>(Resource.Id.switch1);
                swithShowFreeRam = view.FindViewById<SwitchCompat>(Resource.Id.switch2);
                //swithLogging = view.FindViewById<SwitchCompat>(Resource.Id.switch3);
                //swithCrash = view.FindViewById<SwitchCompat>(Resource.Id.switch4);

                switchCompatUseCpu = view.FindViewById<SwitchCompat>(Resource.Id.switchCompat1);
                switchAbsoluteIsViewRam = view.FindViewById<SwitchCompat>(Resource.Id.switchCompat2);
                switchLayoutLotification = view.FindViewById<SwitchCompat>(Resource.Id.switchCompat3);

                sbSizeDigits = view.FindViewById<AppCompatSeekBar>(Resource.Id.seekBar1);
                sbPositionAbscissa = view.FindViewById<AppCompatSeekBar>(Resource.Id.seekBar3);
                sbPositionOrdinate = view.FindViewById<AppCompatSeekBar>(Resource.Id.seekBar4);
                sbUpdateTimer = view.FindViewById<AppCompatSeekBar>(Resource.Id.seekBar5);

                spinnerTypefaceDigits = view.FindViewById<Spinner>(Resource.Id.spinner1);
                spinnerTextColorDigits = view.FindViewById<Spinner>(Resource.Id.spinner3);
                spinnerCanvasColor = view.FindViewById<Spinner>(Resource.Id.spinner5);
                spinnerCanvasMaterial = view.FindViewById<Spinner>(Resource.Id.spinner6);

                tvsbSizeDigits = view.FindViewById<TextView>(Resource.Id.textView2);
                tvsbSwithRelativeIsViewRam = view.FindViewById<TextView>(Resource.Id.textView4);
                tvsbTypefaceDigits = view.FindViewById<TextView>(Resource.Id.textView5);
                tvsbPositionAbscissa = view.FindViewById<TextView>(Resource.Id.textView10);
                tvsbPositionOrdinate = view.FindViewById<TextView>(Resource.Id.textView11);
                tvsbSwithIsShowFreeRam = view.FindViewById<TextView>(Resource.Id.textView13);
                tvsbUpdateTimer = view.FindViewById<TextView>(Resource.Id.textView14);
                //tvsbLogging = view.FindViewById<TextView>(Resource.Id.textView15);
                //tvsbCrash = view.FindViewById<TextView>(Resource.Id.textView16);

                tvsbLayoutLotification = view.FindViewById<AppCompatTextView>(Resource.Id.appCompatTextView3);
                tvsbSwithIsShowFreeCpu = view.FindViewById<AppCompatTextView>(Resource.Id.appCompatTextView1);
                tvsbSwithAbsoluteIsViewRam = view.FindViewById<AppCompatTextView>(Resource.Id.appCompatTextView2);

                materialButtonAppInfo.Click += MaterialButtonAppInfo_Click;

                floatingActionButtonSaveSettings.Click += ButtonSaveSettings_Click;
                floatingActionDefaultSettings.Click += ButtonDefaultSettings_Click;

                radioButtonUseRam.Click += RadioButtonUseRam_Click;

                adapter = ArrayAdapter.CreateFromResource(context, Resource.Array.array_typeface_style, Android.Resource.Layout.SimpleSpinnerItem);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        
                adapterColor = ArrayAdapter.CreateFromResource(context, Resource.Array.array_color, Android.Resource.Layout.SimpleSpinnerItem);
                adapterColor.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

                adapterMaterial = ArrayAdapter.CreateFromResource(context, Resource.Array.array_material, Android.Resource.Layout.SimpleSpinnerItem);
                adapterMaterial.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

                spinnerTypefaceDigits.Adapter = adapter;
                spinnerTextColorDigits.Adapter = adapterColor;
                spinnerCanvasColor.Adapter = adapterColor;
                spinnerCanvasMaterial.Adapter = adapterMaterial;

                sbSizeDigits.ProgressChanged += SbSizeDigits_ProgressChanged;
                sbPositionAbscissa.ProgressChanged += SbPositionAbscissa_ProgressChanged;
                sbPositionOrdinate.ProgressChanged += SbPositionOrdinate_ProgressChanged;
 
                swithRelativeIsViewRam.CheckedChange += SwithRelativeIsViewRam_CheckedChange;
                swithShowFreeRam.CheckedChange += SwithShowFreeRam_CheckedChange;
                //swithLogging.CheckedChange += SwithLogging_CheckedChange;
                //swithCrash.CheckedChange += SwithCrash_CheckedChange;
                switchCompatUseCpu.CheckedChange += SwitchCompatUseCpu_CheckedChange;
                switchAbsoluteIsViewRam.CheckedChange += SwitchAbsoluteIsViewRam_CheckedChange;
                switchLayoutLotification.CheckedChange += SwitchLayoutLotification_CheckedChange;

                spinnerTypefaceDigits.ItemSelected += SpinnerTypefaceDigits_ItemSelected;
                spinnerCanvasColor.ItemSelected += SpinnerCanvasColor_ItemSelected;
                spinnerTextColorDigits.ItemSelected += SpinnerTextColorDigits_ItemSelected;
                spinnerCanvasMaterial.ItemSelected += SpinnerCanvasMaterial_ItemSelected;

                if (settings.ProVersion == false)
                {
                    sbUpdateTimer.ProgressDrawable.SetColorFilter(Color.Olive, PorterDuff.Mode.SrcAtop);
                    sbUpdateTimer.Thumb.SetColorFilter(Color.Olive, PorterDuff.Mode.SrcAtop);

                    radioButtonUseCpu.SetTextColor(Color.Olive);

                    spinnerTypefaceDigits.SetBackgroundColor(Color.Olive);
                    spinnerCanvasColor.SetBackgroundColor(Color.Olive);
                    spinnerTextColorDigits.SetBackgroundColor(Color.Olive);
                    spinnerCanvasMaterial.SetBackgroundColor(Color.Olive);

                    sbUpdateTimer.Touch += SbUpdateTimer_Touch;
                    radioButtonUseCpu.Touch += RadioButtonUseCpu_Touch;
                    spinnerTypefaceDigits.Touch += Spinner_Touch;
                    spinnerCanvasColor.Touch += Spinner_Touch;
                    spinnerTextColorDigits.Touch += Spinner_Touch;
                    spinnerCanvasMaterial.Touch += Spinner_Touch;
                }
                else
                {                   
                    sbUpdateTimer.ProgressChanged += SbUpdateTimer_ProgressChanged;
                    radioButtonUseCpu.Click += RadioButtonUseCpu_Click;
                    spinnerTypefaceDigits.ItemSelected += SpinnerTypefaceDigits_ItemSelected;
                    spinnerCanvasColor.ItemSelected += SpinnerCanvasColor_ItemSelected;
                    spinnerTextColorDigits.ItemSelected += SpinnerTextColorDigits_ItemSelected;
                    spinnerCanvasMaterial.ItemSelected += SpinnerCanvasMaterial_ItemSelected;
                }

                UpdateView();

                #endregion

                return view;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(SettingsFragment), ex);
                #endregion
        
                return view;
            }
            finally { }
        }

        #region RadioButton
        private void RadioButtonUseCpu_Click(object sender, EventArgs e)
        {
            settings.IsCpuSelectedNotification = radioButtonUseCpu.Checked;

            if (radioButtonUseCpu.Checked == true)
            {
                settings.IsRamSelectedNotification = false;
            }
            else
            {
                settings.IsRamSelectedNotification = true;
            }

            switchCompatUseCpu.Checked = settings.IsShowFreeCpu;
            switchAbsoluteIsViewRam.Checked = settings.IsShowAbsoluteRAM;

            tvsbSwithIsShowFreeCpu.Text = String.Concat(new string[] { "Show free CPU:", " ", settings.IsShowFreeCpu.ToString() });

            tvsbSwithIsShowFreeCpu.Visibility = ViewStates.Visible;
            switchCompatUseCpu.Visibility = ViewStates.Visible;

            switchAbsoluteIsViewRam.Visibility = ViewStates.Gone;
            tvsbSwithAbsoluteIsViewRam.Visibility = ViewStates.Gone;
            swithRelativeIsViewRam.Visibility = ViewStates.Gone;
            swithShowFreeRam.Visibility = ViewStates.Gone;
            tvsbSwithRelativeIsViewRam.Visibility = ViewStates.Gone;
            tvsbSwithIsShowFreeRam.Visibility = ViewStates.Gone;
        }

        private void RadioButtonUseRam_Click(object sender, EventArgs e)
        {
            settings.IsRamSelectedNotification = radioButtonUseRam.Checked;

            if (radioButtonUseRam.Checked == true)
            {
                settings.IsCpuSelectedNotification = false;
            }
            else
            {
                settings.IsCpuSelectedNotification = true;
            }

            swithRelativeIsViewRam.Checked = settings.IsShowRelativeRAM;
            swithShowFreeRam.Checked = settings.IsShowFreeRam;

            tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
            tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", " ", settings.IsShowAbsoluteRAM.ToString() });
            tvsbSwithIsShowFreeRam.Text = String.Concat(new string[] { "Show free RAM:", " ", settings.IsShowFreeRam.ToString() });

            swithRelativeIsViewRam.Visibility = ViewStates.Visible;
            swithShowFreeRam.Visibility = ViewStates.Visible;
            tvsbSwithRelativeIsViewRam.Visibility = ViewStates.Visible;
            tvsbSwithIsShowFreeRam.Visibility = ViewStates.Visible;
            tvsbSwithAbsoluteIsViewRam.Visibility = ViewStates.Visible;
            switchAbsoluteIsViewRam.Visibility = ViewStates.Visible;

            tvsbSwithIsShowFreeCpu.Visibility = ViewStates.Gone;
            switchCompatUseCpu.Visibility = ViewStates.Gone;
        }

        private void RadioButtonUseCpu_Touch(object sender, View.TouchEventArgs e)
        {
            if (settings.ProVersion == false)
            {
                ShowPopup();
            }
        }
        #endregion

        #region Buttons
        private void MaterialButtonAppInfo_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("package:" + Android.App.Application.Context.PackageName);

            Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings, uri);
            intent.AddCategory(Intent.CategoryDefault);
            intent.SetFlags(ActivityFlags.NewTask);

            StartActivity(intent);
        }

        private void ButtonSaveSettings_Click(object sender, EventArgs e)
        {
            updatePendingIntent.Send();
            settings.Write();

            Toast.MakeText(Application.Context, SaveSettings, ToastLength.Short).Show();
        }

        private void ButtonDefaultSettings_Click(object sender, EventArgs e)
        {
            if (HearServices.IsServiceRunning(typeof(NotifyService), context))
            {
                var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.layout_toast);

                var layoutInflater = LayoutInflater.FromContext(context);
                var view = layoutInflater.Inflate(remoteViews.LayoutId, null);

                var textView = view.FindViewById<TextView>(Resource.Id.text);

                textView.Text = DefaultSettingsRun;

                Toast toast = new Toast(context);

                toast.SetGravity(GravityFlags.CenterVertical, 0, 0);
                toast.View = view;
                toast.Show();
            }
            else
            {
                Settings settingsDefault = crudSettings.ReadDefault();
                crudSettings.Write(settingsDefault);

                Settings.SetInstance = null;
                settings = Settings.Instance;

                updatePendingIntent.Send();

                UpdateView();

                Toast.MakeText(Application.Context, DefaultSettings, ToastLength.Short).Show();
            }
        }

        #endregion
        
        #region  Swith
        private void SwithShowFreeRam_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            settings.IsShowFreeRam = e.IsChecked;
            tvsbSwithIsShowFreeRam.Text = String.Concat(new string[] { "Show free RAM:", " ", settings.IsShowFreeRam.ToString() });
        }

        //private void SwithCrash_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        //{
        //    settings.IsSendCrashes = e.IsChecked;
        //    Crashes.SetEnabledAsync(settings.IsSendCrashes);
        //    tvsbCrash.Text = String.Concat(new string[] { "Crashes:", " ", settings.IsSendCrashes.ToString() });
        //}

        //private void SwithLogging_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        //{
        //    settings.IsLogErrorStorage = e.IsChecked;
        //    tvsbLogging.Text = String.Concat(new string[] { "Logging:", " ", settings.IsLogErrorStorage.ToString() });
        //}

        private void SwitchLayoutLotification_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            settings.LayoutNotificationEmpty = e.IsChecked;

            tvsbLayoutLotification.Text = String.Concat(new string[] { "Show the notification area:", " ", settings.LayoutNotificationEmpty.ToString() });
        }

        private void SwitchAbsoluteIsViewRam_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            settings.IsShowAbsoluteRAM = e.IsChecked;

            if (e.IsChecked == true)
            {
                settings.IsShowRelativeRAM = false;
                settings.IsShowAbsoluteRAM = true;

                tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
                tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", settings.IsShowAbsoluteRAM.ToString() });

                swithRelativeIsViewRam.Checked = false;
                switchAbsoluteIsViewRam.Checked = true;
            }
            else
            {
                settings.IsShowRelativeRAM = true;
                settings.IsShowAbsoluteRAM = false;

                tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
                tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", settings.IsShowAbsoluteRAM.ToString() });

                swithRelativeIsViewRam.Checked = true;
                switchAbsoluteIsViewRam.Checked = false;
            }
        }

        private void SwithRelativeIsViewRam_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            settings.IsShowRelativeRAM = e.IsChecked;

            if (e.IsChecked == true)
            {
                settings.IsShowRelativeRAM = true;
                settings.IsShowAbsoluteRAM = false;

                tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
                tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", settings.IsShowAbsoluteRAM.ToString() });

                switchAbsoluteIsViewRam.Checked = false;
                swithRelativeIsViewRam.Checked = true;
            }
            else
            {
                settings.IsShowRelativeRAM = false;
                settings.IsShowAbsoluteRAM = true;

                tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
                tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", settings.IsShowAbsoluteRAM.ToString() });

                switchAbsoluteIsViewRam.Checked = true;
                swithRelativeIsViewRam.Checked = false;
            }
        }

        private void SwitchCompatUseCpu_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            settings.IsShowFreeCpu = e.IsChecked;
            tvsbSwithIsShowFreeCpu.Text = String.Concat(new string[] { "Show free CPU:", " ", settings.IsShowFreeCpu.ToString() });
        }

        #endregion
            
        #region Spinner
        private void SpinnerCanvasMaterial_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string canvasMaterial = spinner.GetItemAtPosition(e.Position).ToString();
            settings.CanvasMaterial = canvasMaterial;
        }

        private void SpinnerCanvasColor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string canvasColor = spinner.GetItemAtPosition(e.Position).ToString();
            settings.CanvasColor = canvasColor;
        }

        private void SpinnerTextColorDigits_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string textColor = spinner.GetItemAtPosition(e.Position).ToString();
            settings.ColorTextDigits = textColor;
        }


        private void SpinnerTypefaceDigits_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string text = spinner.GetItemAtPosition(e.Position).ToString();
            settings.TypefaceDigits = text;
        }

        private void Spinner_Touch(object sender, View.TouchEventArgs e)
        {            
            if(touchSpeenerClick > 3)
            {
                ShowPopup();          
            }
            touchSpeenerClick++;
        }

        #endregion

        #region SeekBar
        private void SbPositionOrdinate_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            settings.Ordinate = e.Progress;
            tvsbPositionOrdinate.Text = String.Concat(new string[] { "Ordinate:", " ", settings.Ordinate.ToString() });
        }
        private void SbPositionAbscissa_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            settings.Abscissa = e.Progress;
            tvsbPositionAbscissa.Text = String.Concat(new string[] { "Abscissa:", " ", settings.Abscissa.ToString() });
        }
        private void SbSizeDigits_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            settings.TextSizeDigits = e.Progress;
            tvsbSizeDigits.Text = String.Concat(new string[] { "Size:", " ", settings.TextSizeDigits.ToString() });
        }
        private void SbUpdateTimer_Touch(object sender, View.TouchEventArgs e)
        {
            if (settings.ProVersion == false)
            {
                ShowPopup();
            }
        }
        private void SbUpdateTimer_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            settings.UpdateTimer = e.Progress;
            tvsbUpdateTimer.Text = String.Concat(new string[] { "Update timer:", " ", settings.UpdateTimer.ToString(), " ", "millisecond" });

        }

        #endregion

        #region Popup Dialog   
        private void ShowPopup()
        {
            touchSpeenerClick = 0;
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
            catch (Exception ex)
            {
                popupDialog.Dismiss();
                popupDialog.Hide();

                Toast.MakeText(Application.Context, ErrorsException, ToastLength.Short).Show();
            }
            
        }
        #endregion

        #region Update View
        private void UpdateView()
        {
            try
            {
                int intIndexOfDigits = listAdaptrer.IndexOf(settings.TypefaceDigits);
                int indexColor = listAdaptrerColor.IndexOf(settings.ColorTextDigits);
                int indexCanvasColor = listAdaptrerColor.IndexOf(settings.CanvasColor);
                int indexCanvasMaterial = listAdaptrerMaterial.IndexOf(settings.CanvasMaterial);

                tvsbSizeDigits.Text = String.Concat(new string[] { "Size:", " ", settings.TextSizeDigits.ToString() });
                tvsbTypefaceDigits.Text = String.Concat(new string[] { "Typeface digits:", " " });
                tvsbPositionOrdinate.Text = String.Concat(new string[] { "Ordinate:", " ", settings.Ordinate.ToString() });
                tvsbPositionAbscissa.Text = String.Concat(new string[] { "Abscissa:", " ", settings.Abscissa.ToString() });
                tvsbUpdateTimer.Text = String.Concat(new string[] { "Update timer:", " ", settings.UpdateTimer.ToString(), " ", "millisecond" });
                //tvsbCrash.Text = String.Concat(new string[] { "Crashes:", " ", settings.IsSendCrashes.ToString() });
                //tvsbLogging.Text = String.Concat(new string[] { "Logging:", " ", settings.IsLogErrorStorage.ToString() });

                spinnerTypefaceDigits.SetSelection(intIndexOfDigits);
                spinnerTextColorDigits.SetSelection(indexColor);
                spinnerCanvasColor.SetSelection(indexCanvasColor);
                spinnerCanvasMaterial.SetSelection(indexCanvasMaterial);

                //swithLogging.Checked = settings.IsLogErrorStorage;
                //swithCrash.Checked = settings.IsSendCrashes;

                sbSizeDigits.Progress = settings.TextSizeDigits;
                sbPositionAbscissa.Progress = (int)settings.Abscissa;
                sbPositionOrdinate.Progress = (int)settings.Ordinate;
                sbUpdateTimer.Progress = settings.UpdateTimer;

                tvsbLayoutLotification.Text = String.Concat(new string[] { "Show the notification area:", " ", settings.LayoutNotificationEmpty.ToString() });
                switchLayoutLotification.Checked = settings.LayoutNotificationEmpty;

                swithRelativeIsViewRam.Checked = settings.IsShowRelativeRAM;
                swithShowFreeRam.Checked = settings.IsShowFreeRam;
                switchAbsoluteIsViewRam.Checked = settings.IsShowAbsoluteRAM;
                switchCompatUseCpu.Checked = settings.IsShowFreeCpu;

                tvsbSwithRelativeIsViewRam.Text = String.Concat(new string[] { "Show relative RAM (%):", " ", settings.IsShowRelativeRAM.ToString() });
                tvsbSwithAbsoluteIsViewRam.Text = String.Concat(new string[] { "Show absolute RAM(Gb):", " ", settings.IsShowAbsoluteRAM.ToString() });
                tvsbSwithIsShowFreeRam.Text = String.Concat(new string[] { "Show free RAM:", " ", settings.IsShowFreeRam.ToString() });
                tvsbSwithIsShowFreeCpu.Text = String.Concat(new string[] { "Show free CPU:", " ", settings.IsShowFreeCpu.ToString() });

                if (settings.IsRamSelectedNotification == true)
                {
                    radioButtonUseRam.Checked = true;

                    swithRelativeIsViewRam.Visibility = ViewStates.Visible;
                    swithShowFreeRam.Visibility = ViewStates.Visible;
                    tvsbSwithRelativeIsViewRam.Visibility = ViewStates.Visible;
                    tvsbSwithIsShowFreeRam.Visibility = ViewStates.Visible;
                    tvsbSwithAbsoluteIsViewRam.Visibility = ViewStates.Visible;
                    switchAbsoluteIsViewRam.Visibility = ViewStates.Visible;

                    tvsbSwithIsShowFreeCpu.Visibility = ViewStates.Gone;
                    switchCompatUseCpu.Visibility = ViewStates.Gone;
                }
                else if (settings.IsCpuSelectedNotification == true)
                {
                    radioButtonUseCpu.Checked = true;

                    tvsbSwithIsShowFreeCpu.Visibility = ViewStates.Visible;
                    switchCompatUseCpu.Visibility = ViewStates.Visible;

                    switchAbsoluteIsViewRam.Visibility = ViewStates.Gone;
                    tvsbSwithAbsoluteIsViewRam.Visibility = ViewStates.Gone;
                    swithRelativeIsViewRam.Visibility = ViewStates.Gone;
                    swithShowFreeRam.Visibility = ViewStates.Gone;
                    tvsbSwithRelativeIsViewRam.Visibility = ViewStates.Gone;
                    tvsbSwithIsShowFreeRam.Visibility = ViewStates.Gone;
                }
                else
                {
                    radioButtonUseRam.Checked = true;
                    settings.IsRamSelectedNotification = true;
                }
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(SettingsFragment), ex);
                #endregion
            }
            finally { }
        }
        #endregion

    }
}