using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.FirebasePushNotification;
using Android.Content;
using Android.Support.V4.App;

namespace OnlineExamination.Droid
{
   // [Activity(Label = "OnlineExamination", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [Activity(Icon = "@drawable/Icon3", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }


            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, false);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif


            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {


            };


            //  receiver = new SmsListener();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            //intent.setStyle(new Notification.BigTextStyle();
            //intent.SetData.sty
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);


   //         NotificationCompat.BigTextStyle bigTextStyle = new NotificationCompat.BigTextStyle();
   //         NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
   // .SetSmallIcon(Resource.Drawable.icon)
   // .SetStyle(bigTextStyle)
   // //.SetContentTitle(messageTitle)
   // //.SetContentText(messageBody)
   // //.SetSound(Settings.System.DefaultNotificationUri)
   // .SetVibrate(new long[] { 1000, 1000 })
   // //.SetLights(Color.AliceBlue, 3000, 3000)
   // .SetAutoCancel(true)
   ///* .SetContentIntent(pendingIntent)*/;
        

    }

    }
}