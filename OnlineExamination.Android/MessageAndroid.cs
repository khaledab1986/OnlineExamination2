using System;
using System.Drawing;
using Android.App;
using Android.Graphics;
using Android.Widget;
using OnlineExamination.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace OnlineExamination.Droid
{
    public class MessageAndroid : IMessage
    {


        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
         //   Toast.MakeText(Application.Context, message, ToastLength.Short).Show();

            Toast t = Toast.MakeText(Application.Context, message, ToastLength.Short);
            System.Drawing.Color c = System.Drawing.Color.FromArgb(88, 197,255);
            ColorMatrixColorFilter CM = new ColorMatrixColorFilter(new float[]
    {
        0,0,0,0,c.R,
        0,0,0,0,c.G,
        0,0,0,0,c.B,
        0,0,0,1,0            
    });
            t.View.Background.SetColorFilter(CM);

            t.Show();

            
            //float cc = (((float)(c.R) + (float)(c.G) + (float)(c.B)) / 3);
            //if (cc >= 80)
            //    t.View.FindViewById<TextView>(Android.Resource.Id.Message).SetTextColor((Android.Content.Res.ColorStateList)Android.Resource.Color.White);
            //else
            //    t.View.FindViewById<TextView>(Android.Resource.Id.Message).SetTextColor((Android.Content.Res.ColorStateList)Android.Resource.Color.White);




        }
    }
}
