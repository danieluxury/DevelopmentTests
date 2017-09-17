using System;
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

namespace Beacons
{
    class Utilities
    {
        public int REQUEST_ENABLE_BT = 1;
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

    }
}
