﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using RadiusNetworks.IBeaconAndroid;

namespace Beacons
{
    class Utilities
    {
        public int REQUEST_ENABLE_BT = 1;
        IBeacon currentBeacon = null;

        class FakeBeacon
        {
            public int mayor { get; set; }
            public int minor { get; set; }
            public string color { get; set; }

            public FakeBeacon (int a, int b, string c)
            {
                mayor = a;
                minor = b;
                color = c;
            }
        };

        static FakeBeacon[] listofBeacons = { new FakeBeacon (60521, 27865, "Rosado"),
                                             new FakeBeacon (3161, 57706, "Amarillo"),
                                            new FakeBeacon (8273, 6227, "Morado")};
            
        BluetoothAdapter mBluetoothAdapter = getBluetoothAdapter();
        public bool isBluetoothEnabled()
        {
            if (mBluetoothAdapter != null)
            {
                return mBluetoothAdapter.IsEnabled;
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            else
                return false;

        }

        private static BluetoothAdapter getBluetoothAdapter()
        {
            return BluetoothAdapter.DefaultAdapter;
        }

        public string compareBeacon (int Mayor, int Minor)
        {
            for (int i=0; i<listofBeacons.Length;i++)
            {
                if (listofBeacons[i].mayor==Mayor && listofBeacons[i].minor == Minor)
                {
                    return listofBeacons[i].color;
                }
            }

            return "desconocido";
        }

    }
}
