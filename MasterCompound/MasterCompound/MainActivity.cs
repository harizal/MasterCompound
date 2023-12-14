using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using MasterCompound.Services;
using Plugin.Permissions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MasterCompound
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView lblInfo, lblTotal, lblPercentage;
        ProgressBar progressBar1, progressBar2;
        Button btnStart;

        const int RequestCameraPermissionId = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;

            lblInfo = FindViewById<TextView>(Resource.Id.lblInfo);
            progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progressBar2 = FindViewById<ProgressBar>(Resource.Id.progressBar2);
            lblTotal = FindViewById<TextView>(Resource.Id.lblTotal);
            lblPercentage = FindViewById<TextView>(Resource.Id.lblPercentage);
            btnStart = FindViewById<Button>(Resource.Id.btnStart);

            IsLoadingMode(false);
            btnStart.Click += StartProcess;
        }

        private void IsLoadingMode(bool isLoading)
        {
            if (!isLoading)
            {
                lblInfo.Visibility = ViewStates.Gone;
                progressBar1.Visibility = ViewStates.Gone;
                progressBar2.Visibility = ViewStates.Gone;
                lblTotal.Visibility = ViewStates.Gone;
                lblPercentage.Visibility = ViewStates.Gone;

                btnStart.Visibility = ViewStates.Visible;
            }
            else
            {
                lblInfo.Visibility = ViewStates.Visible;
                progressBar1.Visibility = ViewStates.Visible;
                progressBar2.Visibility = ViewStates.Visible;
                lblTotal.Visibility = ViewStates.Visible;
                lblPercentage.Visibility = ViewStates.Visible;

                btnStart.Visibility = ViewStates.Gone;
            }
        }

        private async void StartProcess(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            var status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                Snackbar.Make(view, "Need Camera Permission.", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
                return;
            }

            IsLoadingMode(true);

            //GetAkta
            lblTotal.Text = "0";
            lblPercentage.Text = "0%";
            lblInfo.Text = "Getting Akta...";
            var aktas = await DataService.GetAkta();
            if (!aktas.Error)
            {
                var totalDatas = aktas?.Data?.Count() ?? 0;
                lblTotal.Text = totalDatas.ToString();
                progressBar1.Max = totalDatas - 1;
                lblPercentage.Text = "0%";
                lblInfo.Text = "Procesing Akta...";

                foreach (var akta in aktas.Data.Select((value, index) => new { index, value }))
                {
                    lblPercentage.Text = $"{(int)Math.Round((double)(100 * akta.index) / totalDatas)}%";
                    progressBar1.Progress = akta.index;
                    await Task.Delay(1000);
                }

                lblInfo.Text = "Akta Done.";
                await Task.Delay(1000);
            }
            else
                lblInfo.Text = aktas.ErrorMessage;


            IsLoadingMode(false);

            lblInfo.Visibility = ViewStates.Visible;
        }







        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }


    }
}
