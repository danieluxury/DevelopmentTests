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

namespace Beacons
{
    [Activity(Label = "Beacons", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait) ]
    public class MainActivity : Activity, IBeaconConsumer
    {
        private const string UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
        private const string monkeyId = "Monkey";
        static readonly string TAG = "FW_TEAM";
        bool _paused;
        View _view;
        Utilities mUtilities;
        IBeaconManager _iBeaconManager;
        MonitorNotifier _monitorNotifier;
        RangeNotifier _rangeNotifier;
        Region _monitoringRegion;
        Region _rangingRegion;
        TextView _text;
        TextView _info;

        int _previousProximity;

        public MainActivity()
        {
            _iBeaconManager = IBeaconManager.GetInstanceForApplication(this);

            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            mUtilities = new Utilities();

            _monitoringRegion = new Region(monkeyId, UUID, null, null);
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
            _iBeaconManager.Bind(this);

            _monitorNotifier.EnterRegionComplete += EnteredRegion;
            _monitorNotifier.ExitRegionComplete += ExitedRegion;

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
            Log.Error(TAG, "App onPause bt enabled{0}", mUtilities.isBluetoothEnabled());
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _monitorNotifier.EnterRegionComplete -= EnteredRegion;
            _monitorNotifier.ExitRegionComplete -= ExitedRegion;

            _rangeNotifier.DidRangeBeaconsInRegionComplete -= RangingBeaconsInRegion;

            _iBeaconManager.StopMonitoringBeaconsInRegion(_monitoringRegion);
            _iBeaconManager.StopRangingBeaconsInRegion(_rangingRegion);
            _iBeaconManager.UnBind(this);
        }

        void EnteredRegion(object sender, MonitorEventArgs e)
        {
            if (_paused)
            {
                ShowNotification();
            }
        }

        void ExitedRegion(object sender, MonitorEventArgs e)
        {
            RunOnUiThread(() => Toast.MakeText(this, "No beacons visible", ToastLength.Short).Show());
        }

        void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            if (e.Beacons.Count > 0)
            {
                var beacon = e.Beacons.FirstOrDefault();
                Log.Error(TAG, "App {0}", e.ToString());
                IBeacon a = e.Beacons.First();
                IBeacon b = e.Beacons.Last();
                ICollection<IBeacon> collection;
                //= e.Beacons.CopyTo();
                List<IBeacon> lista = new List<IBeacon>();
                //lista.Add(e.Beacons.First());
                lista = e.Beacons.ToList();
                var infoMessage = string.Empty; 
                for (int i = 0; i < lista.Count; i++)
                {
                    Log.Error(TAG, "App {0}", lista[i].Major);
                    if ((ProximityType)lista[i].Proximity == ProximityType.Immediate)
                    {
                        infoMessage = lista[i].Major.ToString();
                        UpdateDisplay("You found the monkey!", Color.Green, infoMessage);
                    }
                }

                Log.Error(TAG, "App {0} {1}", a.Rssi, b.Rssi);
                
                var message = string.Empty;
                
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

                _previousProximity = beacon.Proximity;
            }
        }

        #region IBeaconConsumer impl
        public void OnIBeaconServiceConnect()
        {
            _iBeaconManager.SetMonitorNotifier(_monitorNotifier);
            _iBeaconManager.SetRangeNotifier(_rangeNotifier);

            _iBeaconManager.StartMonitoringBeaconsInRegion(_monitoringRegion);
            _iBeaconManager.StartRangingBeaconsInRegion(_rangingRegion);
        }
        #endregion

        private void UpdateDisplay(string message, Color color, string info)
        {
            RunOnUiThread(() =>
            {
                _text.Text = message;
                _info.Text = info;
                Log.Error(TAG, "App UpdateDisplay {0}", info);
                //_view.SetBackgroundColor(color);
            });
        }

        private void ShowNotification()
        {
            var resultIntent = new Intent(this, typeof(MainActivity));
            resultIntent.AddFlags(ActivityFlags.ReorderToFront);
            var pendingIntent = PendingIntent.GetActivity(this, 0, resultIntent, PendingIntentFlags.UpdateCurrent);
            var notificationId = Resource.String.monkey_notification;

            var builder = new NotificationCompat.Builder(this)
                .SetSmallIcon(Resource.Drawable.Xamarin_Icon)
                .SetContentTitle(this.GetText(Resource.String.app_label))
                .SetContentText(this.GetText(Resource.String.monkey_notification))
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

