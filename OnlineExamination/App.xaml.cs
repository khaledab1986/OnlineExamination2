using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ModernHttpClient;
using Newtonsoft.Json;
using OnlineExamination.Views;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OnlineExamination
{
    public partial class App : Application
    {
        public static NativeMessageHandler ttk = new NativeMessageHandler { UseProxy = false };
        public static string user_name,test_add_update; 
        public static HttpClient con = new HttpClient(ttk);
        public static string tok;
        public App()
        {
            InitializeComponent();
            try
            {
                con.Timeout = TimeSpan.FromSeconds(360);

            }
            catch
            {
            }

            MainPage = new ExamShell();
        }

        class Data
        {

            //[JsonProperty("message")]
            //public string Message { get; set; }


            //[JsonProperty("to")]
            //public string To { get; set; }
            [JsonProperty("registration_ids")]
            public List<string> RegistrationIds { get; set; }

            [JsonProperty("priority")]
            public string Priority { get; set; }



            [JsonProperty("notification")]
            public Notification Notification { get; set; }





            //Add the `content_available` key to your payload if you continue to use the legacy sender API.
        }

        class Notification
        {
            [JsonProperty("body")]
            public string Body { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("sound")]
            public string Sound { get; set; }

            [JsonProperty("badge")]
            public int Badge { get; set; }

            [JsonProperty("content_available")]
            public Boolean ContentAvailable { get; set; }

            //click_action
        }
        public static  void updateTok(string ids)
        {
            tok = ids; 
        }
        protected override void OnStart()
        {
            string toktok = "";
            CrossFirebasePushNotification.Current.Subscribe("general");
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
                toktok = p.Token.ToString();
                updateTok(p.Token.ToString());
                //   updateTok(p.Token.ToString());
                //var icc = new TodoItem();
                //icc.Tok = toktok;
                //icc.Mobile = "";
                //icc.Namee = "";
                // App.Database.updateMember(icc);
                //List<TodoItem> list_m = await Database.GetItemsAsync();
                //if (list_m.Count > 0)
                //{
                //    //  user_name = list_m[0].Mobile;
                //    string umobile = list_m[0].Mobile;
                //    if (umobile.Length > 2)
                //}
            };
            System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");

            tok = CrossFirebasePushNotification.Current.Token ;

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Received");
                    if (p.Data.ContainsKey("body"))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                         });

                    }
                }
                catch (Exception ex)
                {

                }

            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
 
                System.Diagnostics.Debug.WriteLine("Opened");
 

                MainPage = new MainPage();

 
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }

                }

            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Dismissed");
            };


        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


        public static async void firebase_send_noti(IList<string> List_tok, string msg)
        {


            try
            {





                string apiKey = "";

                apiKey = "AAAAsPKBYGY:APA91bEVrgIaGKhHOvn7RmzeeiMgWQ5ofe1-ny9KYSApvCYVqda0hBSCXO0OSeOrWGarwjW5Kbz3CoJwOwyUplKPrnig81QYtFgnYA3rwf4NwAw7KzhNmjyphhck6EgfEHksRKgWNGJi";
                var data = new Data();
                data.Priority = "high";

                data.RegistrationIds = new List<string>();

                for (int i = 0; i < List_tok.Count; i++)
                {
                    data.RegistrationIds.Add(List_tok[i]);
                }
                data.Notification = new Notification { Body = msg, Title = "OnlineExamination", Sound = "default", Badge = 0, ContentAvailable = true };
                var json = JsonConvert.SerializeObject(data);
                var c = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
                var myContent = JsonConvert.SerializeObject(data);
                App.con.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://fcm.googleapis.com/fcm/send"),
                    Method = HttpMethod.Post,
                };

                request.Content = new StringContent(myContent, Encoding.UTF8, "application/json");//CONTENT-TYPE header
                App.con.DefaultRequestHeaders.Add("Accept", "application/json;charset=utf-8");
                App.con.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + apiKey);

                var response = await App.con.SendAsync(request);
                var responseString = "";
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseString = await response.Content.ReadAsStringAsync();

                }

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
        }

    }
}
