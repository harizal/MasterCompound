using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Snackbar;
using MasterCompound.Models;
using MasterCompound.Services;
using MasterCompound.Utils;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
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
            lblInfo.Visibility = !isLoading ? ViewStates.Gone : ViewStates.Visible;
            progressBar1.Visibility = !isLoading ? ViewStates.Gone : ViewStates.Visible;
            progressBar2.Visibility = !isLoading ? ViewStates.Gone : ViewStates.Visible;
            lblTotal.Visibility = !isLoading ? ViewStates.Gone : ViewStates.Visible;
            lblPercentage.Visibility = !isLoading ? ViewStates.Gone : ViewStates.Visible;

            btnStart.Visibility = !isLoading ? ViewStates.Visible : ViewStates.Gone;
        }

        private async Task<bool> CheckPermissions(object sender)
        {
            //var status = await CrossPermissions.Current.RequestPermissionAsync<Plugin.Permissions.CameraPermission>();
            //if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            //{
            //    View view = (View)sender;
            //    Snackbar.Make(view, "Need Camera Permission.", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
            //    return false;
            //}

            var status = await CrossPermissions.Current.RequestPermissionAsync<Plugin.Permissions.StoragePermission>();
            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                View view = (View)sender;
                Snackbar.Make(view, "Storage Permission.", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
                return false;
            }

            return true;
        }

        private async Task<List<string>> ProcessData<T>(List<T> datas, string processMessage)
        {
            var result = new List<string>();

            lblTotal.Text = "0";
            lblPercentage.Text = "0%";
            lblInfo.Text = processMessage;

            var totalDatas = datas.Count();
            lblTotal.Text = totalDatas.ToString();
            progressBar1.Max = totalDatas - 1;
            lblPercentage.Text = "0%";
            lblInfo.Text = $"Processing {processMessage}...";

            foreach (var dataItem in datas.Select((value, index) => new { index, value }))
            {
                lblPercentage.Text = $"{(int)Math.Round((double)(100 * dataItem.index) / totalDatas)}%";
                progressBar1.Progress = dataItem.index;
                await Task.Delay(10);
                switch (processMessage)
                {
                    case Constants.Process.JenisKendaraan:
                        if (dataItem.value is JenisKenderaan jenisKendaraan)
                        {
                            result.Add($"{jenisKendaraan.JENKEN.Trim()}{jenisKendaraan.PRGN.Trim().PadRight(40)}");
                        }
                        break;

                    case Constants.Process.Kendaraan:
                        if (dataItem.value is Kenderaan kenderaan)
                        {
                            result.Add($"{kenderaan.KODKEN.Trim().PadRight(3)}{kenderaan.JENKEN.Trim() + kenderaan.PRGN.PadRight(40)}{kenderaan.PRGNDOL.PadRight(40)}");
                        }
                        break;
                    case Constants.Process.Warna:
                        if (dataItem.value is Warna warna)
                        {
                            result.Add($"{warna.KODWARNA.Trim().PadRight(2)}{warna.PRGN.Trim().PadRight(40)}{warna.PRGNDOL.PadRight(17)}");
                        }
                        break;
                    case Constants.Process.Kesalahan:
                        if (dataItem.value is Kesalahan kesalaha)
                        {
                            result.Add($"{kesalaha.KODAKTA.Trim().PadRight(10)}{kesalaha.KODSALAH.Trim().PadRight(4)}{kesalaha.KODHASIL,-8}{kesalaha.PRGNDOL,-15}{kesalaha.PRGNPANJANG,-1000}" +
                                       $"{kesalaha.HARGA * 100,-10}{kesalaha.HARGA28 * 100,-10}{kesalaha.HARGA30 * 100,-10}{kesalaha.KODORI}");
                        }
                        break;
                    case Constants.Process.Kawasan:
                        if (dataItem.value is Kawasan kawasan)
                        {
                            result.Add($"{kawasan.KODKAW.Trim().PadRight(4)}{kawasan.KODZON.Trim().PadRight(2)}{kawasan.PRGN,-40}{kawasan.PRGNDOL,-15}");
                        }
                        break;
                    case Constants.Process.Akta:
                        if (dataItem.value is Akta akta)
                        {
                            result.Add($"{akta.KODAKTA.Trim().PadRight(10)}{akta.PRGN.Trim().PadRight(10)}{akta.PRGN,-40}{akta.PRGNPANJANG,-255}");
                        }
                        break;
                    case Constants.Process.KodHantar:
                        if (dataItem.value is KodHantar kodHantar)
                        {
                            result.Add($"{kodHantar.KODHANTAR.Trim().PadRight(2)}{kodHantar.PRGNDOL.Trim().PadRight(50)}");
                        }
                        break;
                    case Constants.Process.Zon:
                        if (dataItem.value is Zon zon)
                        {
                            result.Add($"{zon.KODZON.Trim().PadRight(2)}{zon.PRGN.Trim().PadRight(40)}");
                        }
                        break;
                    case Constants.Process.TempatJadi:
                        if (dataItem.value is TempatJadi tempatJadi)
                        {
                            result.Add($"{tempatJadi.KODTEMPATJADI.Trim().PadRight(4)}{tempatJadi.PRGN.Trim().PadRight(100)}");
                        }
                        break;
                    case Constants.Process.KodSita:
                        if (dataItem.value is KodSita kodSita)
                        {
                            result.Add($"{kodSita.KODSITA.Trim().PadRight(4)}{kodSita.PRGN.Trim().PadRight(40)}");
                        }
                        break;
                }
            }

            lblInfo.Text = $"{processMessage} Done.";
            await Task.Delay(50);

            return result;
        }

        private async void StartProcess(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!await CheckPermissions(sender))
                    return;

                IsLoadingMode(true);

                //DH04.FIL
                #region DH04.FIL
                var dh04Result = new List<string>();

                var jenisKenderaanDatas = await DataService.GetJenisKenderaan();
                var jenisKenderaans = await ProcessData(jenisKenderaanDatas.Data, Constants.Process.JenisKendaraan);
                dh04Result.AddRange(jenisKenderaans);
                dh04Result.Add(Constants.SplitCode);

                var kenderaanDatas = await DataService.GetKenderaan();
                var kenderaans = await ProcessData(kenderaanDatas.Data, Constants.Process.Kendaraan);
                dh04Result.AddRange(kenderaans);
                dh04Result.Add(Constants.SplitCode);

                var warnaDatas = await DataService.GetWarna();
                var warnas = await ProcessData(warnaDatas.Data, Constants.Process.Warna);
                dh04Result.AddRange(warnas);
                dh04Result.Add(Constants.SplitCode);

                var kesalahanDatas = await DataService.GetKesalahan();
                var kesalahans = await ProcessData(kesalahanDatas.Data, Constants.Process.Kesalahan);
                dh04Result.AddRange(kesalahans);
                dh04Result.Add(Constants.SplitCode);

                var kawasanDatas = await DataService.GetKawasan();
                var kawasans = await ProcessData(kawasanDatas.Data, Constants.Process.Kawasan);
                dh04Result.AddRange(kawasans);
                dh04Result.Add(Constants.SplitCode);

                var jenisAktaDatas = await DataService.GetAkta();
                var jenisAktas = await ProcessData(jenisAktaDatas.Data, Constants.Process.Akta);
                dh04Result.AddRange(jenisAktas);
                dh04Result.Add(Constants.SplitCode);

                var KodHantarDatas = await DataService.GetKodHantar();
                var KodHantars = await ProcessData(KodHantarDatas.Data, Constants.Process.KodHantar);
                dh04Result.AddRange(KodHantars);
                dh04Result.Add(Constants.SplitCode);

                var zonDatas = await DataService.GetZon();
                var zons = await ProcessData(zonDatas.Data, Constants.Process.Zon);
                dh04Result.AddRange(zons);
                dh04Result.Add(Constants.SplitCode);

                var tempatJadiDatas = await DataService.GetTempatJadi();
                var tempatJadis = await ProcessData(tempatJadiDatas.Data, Constants.Process.TempatJadi);
                dh04Result.AddRange(tempatJadis);
                dh04Result.Add(Constants.SplitCode);

                var kodSitaDatas = await DataService.GetKodSita();
                var kodSitas = await ProcessData(kodSitaDatas.Data, Constants.Process.KodSita);
                dh04Result.AddRange(kodSitas);
                dh04Result.Add(Constants.SplitCode);

                IsLoadingMode(false);
                lblInfo.Visibility = ViewStates.Visible;
                lblInfo.Text = "Insert data to DH04.FIL";
                await SaveAsync(Constants.TableFil, dh04Result);
                lblInfo.Text = "Insert data to DH04.FIL - DONE";
                #endregion
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"{ex.Message} - {ex.StackTrace}";
            }

            IsLoadingMode(false);
            lblInfo.Visibility = ViewStates.Visible;
        }

        private async Task SaveAsync(string fileName, List<string> datas)
        {
            var transLocation = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Constants.ProgramPath);
            if (!Directory.Exists(transLocation))
                Directory.CreateDirectory(transLocation);

            var fileLocation = Path.Combine(transLocation, Constants.TableFil);
            if (!File.Exists(fileLocation))
                File.Create(fileLocation);

            using var fileStream = new FileStream(fileLocation, FileMode.Open);
            using var streamWriter = new StreamWriter(fileStream);
            foreach (var item in datas)
            {
                await streamWriter.WriteAsync(item);
            }
        }





        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
