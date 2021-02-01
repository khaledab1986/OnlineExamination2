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
    public partial class courses_Admin : ContentPage
    {
        public static DataTable dt_courses = new DataTable();
        public ICommand stackClick { get; private set; }
        public static int idd = 0;
       
        public courses_Admin()
        {
            InitializeComponent();
            nickname.Text  = AdminPage.NickName; 
            stackClick = new Command<Parm>((Parm parameter) => OnTapped(parameter));
            if (dt_courses.Columns.Count == 0)
            {
                dt_courses.Columns.Add("course_id", typeof(int));
                dt_courses.Columns.Add("User_id", typeof(int));
                dt_courses.Columns.Add("course_name", typeof(string));
                dt_courses.Columns.Add("fin", typeof(int));
        
            }
            MessagingCenter.Subscribe<courses_info>(this, "ref", (sender) => {

                IsBusy = true;

                Refsh();
            });
        }
        protected override void OnAppearing()
        {

            Refsh();

        }

        class Parm
        {

            int idd;

            public int Idd { get => idd; set => idd = value; }


            Frame ctk;


            public Frame Ctk { get => ctk; set => ctk = value; }

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

                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_courses.php?idd=" + AdminPage.idd;
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<CourseTech>>(content);
                        if (tr is null)
                        {
                            examjson.IsVisible = false;
                            examjson.IsEnabled = false;
                            examjson.IsRunning = false;

                            return;
                        }
                        ObservableCollection<CourseTech> trends = new ObservableCollection<CourseTech>(tr);
                        dt_courses.Rows.Clear();
                        stk.Children.Clear();
                        string knd = "";
                        for (int i = 0; i < trends.Count; i++)
                        {


                            dt_courses.Rows.Add(trends[i].CourseId, trends[i].UserId, trends[i].CourseName, trends[i].Fin);
                            Frame fer = new Frame {
                                Padding =3,
                                BorderColor =Color.FromHex("#B2B2B2"),
                                BackgroundColor = Color.White ,
                                CornerRadius=7,
                            };

                            StackLayout frm1 = new StackLayout
                            {
                                Margin = new Thickness(10, 5, 10, 5),
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BackgroundColor = Color.Transparent,

                            };
                         //   RoundCornersEffect.SetCornerRadius(frm1, 7);
                            var c1 = new Parm
                            {
                                Ctk = fer ,
                                Idd = trends[i].CourseId,
                            };

                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("stackClick", 0));
                            tapGestureRecognizer.BindingContext = this;
                            tapGestureRecognizer.CommandParameter = c1;
                            fer.GestureRecognizers.Add(tapGestureRecognizer);


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
                                Text = trends[i].CourseName,
                                TextColor = Color.FromHex("#4D4D4D"),
                            };
                            stk1.Children.Add(lb1);
                            frm1.Children.Add(stk1);
                            fer.Content = stk1;
                            stk.Children.Add(fer);


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

        class CourseTech
        {
            [JsonProperty("course_id")]
            public int CourseId { get; set; }

            [JsonProperty("User_id")]
            public long UserId { get; set; }

            [JsonProperty("course_name")]
            public string CourseName { get; set; }

            [JsonProperty("fin")]
            public long Fin { get; set; }
        }

        async void OnTapped(Parm tt2)
        {

            Frame ck = tt2.Ctk;
            await ck.FadeTo(0.7, 100);
            await ck.FadeTo(1, 200);
            idd = tt2.Idd;
            await PopupNavigation.Instance.PushAsync(new courses_info());

        }

       async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            idd = 0; 
            await PopupNavigation.Instance.PushAsync(new courses_info());
        }
    }
}
