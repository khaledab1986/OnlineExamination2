using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.Admin
{
    public partial class AdminPage : ContentPage
    {
        public static DataTable dt_AllUser = new DataTable() ;
        public ICommand stackClick { get; private set; }
        public static int idd = 0;
        public static int tech_ = 0;
        public static string  NickName = "";

        public AdminPage()
        {
            InitializeComponent();
            stackClick = new Command<Parm>((Parm parameter) => OnTapped(parameter));
            if (dt_AllUser.Columns.Count == 0)
            {
                dt_AllUser.Columns.Add("id", typeof(int));
                dt_AllUser.Columns.Add("User_name", typeof(string));
                dt_AllUser.Columns.Add("User_pass", typeof(string));
                dt_AllUser.Columns.Add("User_kind", typeof(int));
                dt_AllUser.Columns.Add("fin", typeof(int));
                dt_AllUser.Columns.Add("User_nickname", typeof(string));
            }
            MessagingCenter.Subscribe<UserInfo>(this, "ref", (sender) => {

                IsBusy = true;

                Refsh();
            });
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            idd = 0 ;
            await PopupNavigation.Instance.PushAsync(new UserInfo());
        }
        async void Refsh()
        {
            examjson.IsVisible = true;
            examjson.IsEnabled = true;
            examjson.IsRunning = true;

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {

                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_AllUser.php";
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<AllUsers>>(content);
                        if (tr is null)
                        {
                            examjson.IsVisible = false;
                            examjson.IsEnabled = false;
                            examjson.IsRunning = false;

                            return;
                        }
                        ObservableCollection<AllUsers> trends = new ObservableCollection<AllUsers>(tr);
                        dt_AllUser.Rows.Clear();
                        stk.Children.Clear();
                        string knd = "";
                        int tech = 0 ;
                        for (int i = 0; i < trends.Count; i++)
                        {

                            if (trends[i].UserKind == 0) { knd = "Student"; tech = 0; }
                            if (trends[i].UserKind == 1) { knd = "Teacher"; tech = 1; }
                            if (trends[i].UserKind == 2) { knd = "Admin"; tech = 0; }
                            dt_AllUser.Rows.Add(trends[i].Id, trends[i].UserName, trends[i].UserPass, trends[i].UserKind, trends[i].Fin, trends[i].UserNickname);

                            StackLayout frm1 = new StackLayout
                            {
                                Margin = new Thickness(10, 5, 10, 5),
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BackgroundColor = Color.White,

                            };
                            RoundCornersEffect.SetCornerRadius(frm1, 7);
                            var c1 = new Parm
                            {
                                Nickname = trends[i].UserName ,
                                Ctk = frm1,
                                Tech = tech ,
                                Idd = trends[i].Id,
                            };

                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("stackClick", 0));
                            tapGestureRecognizer.BindingContext = this;
                            tapGestureRecognizer.CommandParameter = c1;
                            frm1.GestureRecognizers.Add(tapGestureRecognizer);


                            StackLayout stk1 = new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Padding = 5,
                            };


                      


                            Label lb1 = new Label
                            {
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                Text = trends[i].UserNickname,
                                TextColor = Color.FromHex("#4D4D4D"),
                            };

                            //DEscription
                            Label lb2 = new Label
                            {
                                Margin = 5,
                                Text = knd,
                                VerticalOptions = LayoutOptions.Center,
                                TextColor = Color.FromHex("#4D4D4D"),
                                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),

                            };

                            if (trends[i].UserKind == 0) { lb1.TextColor = Color.FromHex("#4D4D4D"); lb2.TextColor = Color.FromHex("#4D4D4D"); }
                            if (trends[i].UserKind == 1) { lb1.TextColor = Color.FromHex("#1E48A6"); lb2.TextColor = Color.FromHex("#1E48A6"); }
                            if (trends[i].UserKind == 2) { lb1.TextColor = Color.Green; lb2.TextColor = Color.Green; }

                            stk1.Children.Add(lb1);
                            stk1.Children.Add(lb2);

                            frm1.Children.Add(stk1);
                            stk.Children.Add(frm1);


                        }

                    }
                    catch (Exception eb)
                    {
                    }
                    examjson.IsVisible = false;
                    examjson.IsEnabled = false;
                    examjson.IsRunning = false;
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
                }
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
            }
        }
        async void OnTapped(Parm tt2)
        {

            StackLayout ck = tt2.Ctk;
            await ck.FadeTo(0.7, 100);
            await ck.FadeTo(1, 200);
            idd = tt2.Idd;
            tech_ = tt2.Tech;
            NickName = tt2.Nickname; 
            await PopupNavigation.Instance.PushAsync(new UserInfo());

        }

        protected override   void OnAppearing()
        {

            Refsh();

        }

        class Parm
        {
            int tech;
            int idd;
            string  nickname;
            public int Idd { get => idd; set => idd = value; }

 
            StackLayout ctk;
 

            public StackLayout Ctk { get => ctk; set => ctk = value; }
            public int Tech { get => tech; set => tech = value; }
            public string Nickname { get => nickname; set => nickname = value; }
        }

        class AllUsers
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("User_name")]
            public string UserName { get; set; }

            [JsonProperty("User_pass")]
            public string UserPass { get; set; }

            [JsonProperty("User_kind")]
            public long UserKind { get; set; }

            [JsonProperty("fin")]
            public long Fin { get; set; }

            [JsonProperty("User_nickname")]
            public string UserNickname { get; set; }
        }

    }
}
