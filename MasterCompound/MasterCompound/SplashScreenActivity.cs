using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Google.Android.Material.Snackbar;
using MasterCompound.Utils;
using System;
using System.Data;
using System.IO;
using System.Threading;
using static MasterCompound.Utils.Constants;
namespace MasterCompound
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        private static int PERMISSION_REQUEST_CODE = 10;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.splash_screen_ayout);
            // Create your application here

            new Thread(() =>
            {
                Thread.Sleep(500);
                RunOnUiThread(SetInit);
            }).Start();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PERMISSION_REQUEST_CODE)
            {

                bool isGranted = false;
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                {
                    if (Android.OS.Environment.IsExternalStorageManager)
                    {
                        // perform action when allow permission success
                        isGranted = true;
                    }

                }
                if (isGranted)
                {
                    SetInit();
                }
                else
                {
                    ShowSnackBar();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            var view = FindViewById(Resource.Id.linearSnack);
            if (requestCode == PERMISSION_REQUEST_CODE)
            {
                bool isGranted = true;
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                {
                    if (Android.OS.Environment.IsExternalStorageManager)
                    {

                    }
                    else
                    {
                        // isGranted = false;
                        // perform action when allow permission success
                        Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                        intent.AddCategory("android.intent.category.DEFAULT");

                        intent.SetData(Android.Net.Uri.Parse(string.Format("package:{0}", ApplicationContext.PackageName)));
                        StartActivityForResult(intent, PERMISSION_REQUEST_CODE);
                        return;
                    }
                }

                foreach (var grantResult in grantResults)
                {
                    if (grantResult == Permission.Denied)
                    {
                        isGranted = false;
                        break;
                    }
                }
                if (isGranted)
                {
                    SetInit();
                }
                else
                {
                    ShowSnackBar();
                }
            }
        }

        private void ExitApps()
        {
            var intent = new Intent(this, typeof(SplashScreenActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();

        }

        private void ShowSnackBar()
        {
            var view = FindViewById(Resource.Id.linearSnack);
            Snackbar.Make(view, "Permission Denied", Snackbar.LengthIndefinite)
                      .SetAction("OK", v => ExitApps())
                      .Show();
        }

        private void SetInit()
        {
            try
            {
                //SQLitePCL.Batteries_V2.Init();

                bool isGranted = false;
                var androidVersion = GeneralAndroidClass.GetBuildVersion();
                var listPermission = GeneralAndroidClass.ListPermissions();



                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                {
                    //check special permission for access files
                    if (!Android.OS.Environment.IsExternalStorageManager)
                    {
                        Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                        intent.AddCategory("android.intent.category.DEFAULT");

                        intent.SetData(Android.Net.Uri.Parse(string.Format("package:{0}", ApplicationContext.PackageName)));
                        StartActivityForResult(intent, PERMISSION_REQUEST_CODE);
                        return;
                    }
                    else
                    {
                        isGranted = true;
                    }
                }


                bool showSnackBar = true;

                //check permission first
                if (listPermission.Count > 0)
                {
                    isGranted = false;
                    bool isRationale = false;

                    foreach (var permission in listPermission)
                    {
                        if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
                        {
                            isRationale = true;
                            break;
                        }
                    }
                    if (isRationale)
                    {
                        var view = FindViewById(Resource.Id.linearSnack);
                        //Explain to the user why we need to read the contacts
                        Snackbar.Make(view, "Permission Denied", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => ActivityCompat.RequestPermissions(this, listPermission.ToArray(), PERMISSION_REQUEST_CODE))
                            .Show();

                        return;
                    }

                    ActivityCompat.RequestPermissions(this, listPermission.ToArray(), PERMISSION_REQUEST_CODE);
                    showSnackBar = false;
                }
                else
                {
                    isGranted = true;
                }

                if (isGranted)
                {
                    var programLocation = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Constants.ProgramPath);
                    if (!Directory.Exists(programLocation))
                        Directory.CreateDirectory(programLocation);

                    var configFolder = Path.Combine(programLocation, Constants.ConfigPath);
                    if (!Directory.Exists(configFolder))
                        Directory.CreateDirectory(configFolder);


                    var configPath = Path.Combine(configFolder, Constants.ConfigName);
                    if (!File.Exists(configPath))
                    {
                        using BinaryReader br = new BinaryReader(Application.Context.Assets.Open(Constants.ConfigName));
                        using BinaryWriter bw = new BinaryWriter(new FileStream(configPath, FileMode.Create));
                        byte[] buffer = new byte[2048];
                        int len = 0;
                        while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, len);
                        }
                    }

                    DataSet dsData = new DataSet();
                    dsData.ReadXml(configPath);

                    SharedPreferences.RemoveKey(SharedPreferencesKeys.Key);
                    SharedPreferences.RemoveKey(SharedPreferencesKeys.Url);
                    

                    string settingName = "";
                    foreach (DataRow myRow in dsData.Tables[0].Rows)
                    {
                        foreach (DataColumn myCol in dsData.Tables[0].Columns)
                        {
                            settingName = myCol.ColumnName.ToUpper();
                            if (string.Compare(settingName, "Key".ToUpper(), StringComparison.Ordinal) == 0)
                                SharedPreferences.SaveString(SharedPreferencesKeys.Key, myRow[myCol].ToString());
                            else if (string.Compare(settingName, "WebServiceUrl".ToUpper(), StringComparison.Ordinal) == 0)
                                SharedPreferences.SaveString(SharedPreferencesKeys.Url, myRow[myCol].ToString());
                        }
                    }

                    ShowMainActivity();
                }
                else
                {
                    if (showSnackBar)
                    {
                        ShowSnackBar();
                    }

                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long);
                ShowMainActivity();
            }
        }

        private void ShowMainActivity()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }
    }
}