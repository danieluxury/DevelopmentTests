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

namespace Beacons
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class Welcome : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Welcome);

            Button button1 = FindViewById<Button>(Resource.Id.WelcomeButton);

            button1.Click += (object sender, EventArgs e) =>
            {
                Toast.MakeText(this, "You hit me", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            };
            // Create your application here
        }
    }
}