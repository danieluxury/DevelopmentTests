using Android.App;
using System;
using System.Linq;
using System.Collections;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Util;
using Color = Android.Graphics.Color;
using RadiusNetworks.IBeaconAndroid;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Content;
using Android.Bluetooth;
using Android.Content.PM;
using Android.Graphics.Drawables;

namespace Beacons
{
    [Activity(Label = "Beacons", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity, IBeaconConsumer
    {
        private const string UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
        private const string monkeyId = "Monkey";
        static readonly string TAG = "FW_TEAM";
        bool _paused;
        int notBeaconDetectedcount = 0;
        View _view;
        Utilities mUtilities;
        IBeaconManager _iBeaconManager;
        IBeacon currentBeacon = new IBeacon("Fake Beacon", 0, 0);
        IBeacon previousBeacon = new IBeacon("Fake Beacon",0,0);
        //MonitorNotifier _monitorNotifier;
        RangeNotifier _rangeNotifier;
        //Region _monitoringRegion;
        Region _rangingRegion;
        TextView _text;
        TextView _info;
        ImageView imageView;
        AnimationDrawable freshiidrawable;
    

        public MainActivity()
        {
            _iBeaconManager = IBeaconManager.GetInstanceForApplication(this);

            //_monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            mUtilities = new Utilities();

            //_monitoringRegion = new Region(monkeyId, UUID, null, null);
            _rangingRegion = new Region(monkeyId, UUID, null, null);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            _view = FindViewById<LinearLayout>(Resource.Id.BeaconsView);
            _text = FindViewById<TextView>(Resource.Id.statusLabel);
            _info = FindViewById<TextView>(Resource.Id.statusLabel2);
            imageView = FindViewById<ImageView>(Resource.Id.demoImageView);
            freshiidrawable = (AnimationDrawable)Resources.GetDrawable(Resource.Drawable.loadingFreshii);
            imageView.SetImageDrawable(freshiidrawable);

            
            freshiidrawable.Start();
  
            _iBeaconManager.Bind(this);

            //_monitorNotifier.EnterRegionComplete += EnteredRegion;
            //_monitorNotifier.ExitRegionComplete += ExitedRegion;

            _rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;
            askToActivateBluetooth();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _paused = false;
            Log.Error(TAG, "App onResume bt enabled{0}", mUtilities.isBluetoothEnabled());
            askToActivateBluetooth();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _paused = true;
            previousBeacon = currentBeacon;
            if (freshiidrawable.IsRunning)
            {
                freshiidrawable.Stop();
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //_monitorNotifier.EnterRegionComplete -= EnteredRegion;
            //_monitorNotifier.ExitRegionComplete -= ExitedRegion;

            _rangeNotifier.DidRangeBeaconsInRegionComplete -= RangingBeaconsInRegion;

            //_iBeaconManager.StopMonitoringBeaconsInRegion(_monitoringRegion);
            _iBeaconManager.StopRangingBeaconsInRegion(_rangingRegion);
            _iBeaconManager.UnBind(this);
            if (freshiidrawable.IsRunning)
            {
                freshiidrawable.Stop();
            }
        }

        //void EnteredRegion(object sender, MonitorEventArgs e)
        //{
        //    if (_paused)
        //    {
        //        ShowNotification();
        //    }
        //}

        //void ExitedRegion(object sender, MonitorEventArgs e)
        //{
        //    RunOnUiThread(() => Toast.MakeText(this, "No beacons visible", ToastLength.Short).Show());
        //}

        void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                
                var infoMessage = string.Empty;
                List<IBeacon> listaZero = new List<IBeacon>();
                listaZero = e.Beacons.ToList();
                List<IBeacon> lista = listaZero.OrderBy(IBeacon => IBeacon.Accuracy).ThenBy(IBeacon => IBeacon.Proximity).ToList();

                for (int i = 0; i < lista.Count; i++)
                {
                    
                    //Log.Error(TAG, "i{0} Color{1} Ma{2} Mi{3} Prox{4} Accur{5} Rssi{6}",i, infoMessage, lista[i].Major, lista[i].Minor, lista[i].Proximity, lista[i].Accuracy, lista[i].Rssi);
                    //Log.Error(TAG, "App JniIdentityHashCode {0} JniPeerMembers{1} ProximityUuid{2}", lista[i].JniIdentityHashCode, lista[i].JniPeerMembers, lista[i].ProximityUuid);
                    if ((ProximityType)lista[i].Proximity == ProximityType.Immediate)
                    {
                        if (freshiidrawable.IsRunning)
                        {
                            freshiidrawable.Stop();
                        }
                        notBeaconDetectedcount = 0;
                        currentBeacon = lista[i];
                        infoMessage = mUtilities.compareBeacon(currentBeacon.Major, currentBeacon.Minor);
                        if (_paused == false && (currentBeacon.Major != previousBeacon.Major) && (currentBeacon.Minor != previousBeacon.Minor)) {
                            UpdateDisplay("Beacon detectado!", Color.Green, infoMessage);
                            previousBeacon = currentBeacon;
                            break;
                        }
                        else if ( (currentBeacon.Major != previousBeacon.Major) && (currentBeacon.Minor != previousBeacon.Minor))
                        {
                            ShowNotification(infoMessage);
                            previousBeacon = currentBeacon;
                        }
                       
                    }
                    if (_paused == false)
                    {
                        notBeaconDetectedcount++;
                        if (notBeaconDetectedcount > 10) {
                            UpdateDisplay("Beacon detectado!", Color.Green, "No Beacon near");
                        }
                    }
                    
                }


                //           switch ((ProximityType)beacon.Proximity)
                //           {
                //               case ProximityType.Immediate:
                //                   UpdateDisplay("You found the monkey!", Color.Green, infoMessage);
                //                   break;
                //               /*case ProximityType.Near:
                //	UpdateDisplay("You're getting warmer", Color.Yellow, infoMessage);
                //	break;
                //case ProximityType.Far:
                //	UpdateDisplay("You're freezing cold", Color.Blue, infoMessage);
                //	break;*/
                //               case ProximityType.Unknown:
                //                   UpdateDisplay("I'm not sure how close you are to the monkey", Color.Red, infoMessage);
                //                   break;
                //           }

            }
        }

        #region IBeaconConsumer impl
        public void OnIBeaconServiceConnect()
        {
            //_iBeaconManager.SetMonitorNotifier(_monitorNotifier);
            _iBeaconManager.SetRangeNotifier(_rangeNotifier);

            //_iBeaconManager.StartMonitoringBeaconsInRegion(_monitoringRegion);
            _iBeaconManager.StartRangingBeaconsInRegion(_rangingRegion);
        }
        #endregion

        private void UpdateDisplay(string message, Color color, string info)
        {
            //int picture = (int)typeof(Resource.Drawable).GetField("Bowls").GetValue(null);
            //imageView.SetImageResource(picture);
            RunOnUiThread(() =>
            {
                //_text.Text = message;
                //_info.Text = info;
                Log.Error(TAG, "App UpdateDisplay {0}", info);
                //freshiidrawable.Stop();
                switch (info)
                {
                    case "Rosado":
                        SetContentView(Resource.Layout.Menu1ScrollView);
                        break;
                    case "Amarillo":
                        SetContentView(Resource.Layout.Novedades);
                        break;
                    case "Morado":
                        var uri = Android.Net.Uri.Parse("https://www.freshii.com/ec/locations");
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                        break;
                    
                    default:
                        break;
                }

             //   string image = mUtilities.imageAccordingBeacon(info);
             //   int picture = (int)typeof(Resource.Drawable).GetField(image).GetValue(null);

                //var imageBitmap = mUtilities.GetImageBitmapFromUrl("https://k61.kn3.net/taringa/7/5/9/4/0/5/8/tecnomayro/495.jpg");
                //imageView.SetImageBitmap(imageBitmap);


                //imageView.SetImageResource(picture);
               
                //_view.SetBackgroundColor(color);
            });
        }

        private void ShowNotification(string color)
        {
            if (freshiidrawable.IsRunning)
            {
                freshiidrawable.Stop();
            }
            var resultIntent = new Intent(this, typeof(MainActivity));
            resultIntent.AddFlags(ActivityFlags.ReorderToFront);
            var pendingIntent = PendingIntent.GetActivity(this, 0, resultIntent, PendingIntentFlags.UpdateCurrent);
            var notificationId = Resource.String.monkey_notification;

            var builder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.Freshii_icon_original)
                .SetContentTitle(this.GetText(Resource.String.app_label))
                .SetContentText("Beacon " + color +" detectado!" )
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true);

            var notification = builder.Build();

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(notificationId, notification);
        }

        public void askToActivateBluetooth()
        {
            if ( mUtilities.isBluetoothEnabled()== false)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, mUtilities.REQUEST_ENABLE_BT);
            }
        }
    }
}

