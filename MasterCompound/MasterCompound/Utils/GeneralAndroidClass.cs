using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.Content;
using System.Collections.Generic;

namespace MasterCompound.Utils
{
    public static class GeneralAndroidClass
    {
        public static readonly string[] Permissions =
       {
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage,
#if DebugSM29 || ReleaseSM29
 
#else
            Manifest.Permission.ManageExternalStorage,
#endif
            Manifest.Permission.Internet,

        };
        public static readonly string[] PermissionsAndroid11 =
        {
            Manifest.Permission.Internet,
        };

        public static readonly string[] PermissionsAndroid12 =
        {
            Manifest.Permission.Internet,
            Manifest.Permission.ChangeNetworkState,
        };

        public static void ShowToast(Context context, string sMessage)
        {
            Toast.MakeText(context, sMessage, ToastLength.Long).Show();
        }

        public static int GetBuildVersion()
        {
            return (int)Build.VERSION.SdkInt;
        }

        public static List<string> ListPermissions()
        {
            var listPermission = new List<string>();
            var androidVersion = GetBuildVersion();

            if (androidVersion < 23)
            {
                return listPermission;
            }
            if (androidVersion < 30)
            {
                foreach (var permission in Permissions)
                {
                    var result = ContextCompat.CheckSelfPermission(Application.Context, permission);
                    if (result != Permission.Granted)
                    {
                        listPermission.Add(permission);
                    }
                }
            }
            else
            {
                foreach (var permission in PermissionsAndroid11)
                {
                    var result = ContextCompat.CheckSelfPermission(Application.Context, permission);
                    if (result != Permission.Granted)
                    {
                        listPermission.Add(permission);
                    }
                }
            }

            return listPermission;
        }
    }
}