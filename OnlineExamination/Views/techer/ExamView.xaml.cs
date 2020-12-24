using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class ExamView : ContentPage
    {
        public ICommand stackClick { get; private set; }

        public static int qtyp, qhour, qmark,Eid;
        public static DateTime qdat, qtim;
        public static string des;
        public static DataTable dt_selectQ = new DataTable();
        public static DataTable dt_exam = new DataTable();
        public static DataTable qus = new DataTable();
        public ExamView()
        {
            InitializeComponent();
            examjson.Animation = "loading.json";
            stackClick = new Command<Parm>((Parm parameter) => OnTapped(parameter));
            if (dt_exam.Columns.Count == 0)
            {
                dt_exam.Columns.Add("id", typeof(int));
                dt_exam.Columns.Add("dat", typeof(DateTime));
                dt_exam.Columns.Add("tim", typeof(DateTime));
                dt_exam.Columns.Add("des", typeof(string));
                dt_exam.Columns.Add("hor", typeof(int));
                dt_exam.Columns.Add("mark", typeof(int));
                dt_exam.Columns.Add("stat", typeof(string));
                dt_exam.Columns.Add("courses_id", typeof(int));
                dt_exam.Columns.Add("q_type", typeof(int)); 
            }
            if (qus.Columns.Count == 0)
            {
                qus.Columns.Add("q_id", typeof(int));
                qus.Columns.Add("exam_id", typeof(int));
            }
            CourseName.Text = Course.course_name; 
        }


        async void OnTapped(Parm tt2)
        {
            //QID = tt2.Q_id;


            StackLayout ck = tt2.Ctk;
            await ck.FadeTo(0.7, 100);
            await ck.FadeTo(1, 200);


            qtyp = tt2.Qtyp;
            qhour = tt2.Qhour;
            qmark = tt2.Qmark ;
            qdat = tt2.Qdat;
            qtim = tt2.Qtim ;
            des  = tt2.Des ;
            Eid = tt2.E_id;

            if (tt2.Stat != "Finish")
            {
                await Shell.Current.Navigation.PushAsync(new UpdateExam());
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new ExamResults());

            }


        }

        protected override async void OnAppearing()
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/Ref_Exam.php?courses_id=" + Course.course_id;
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<ExamClass2>>(content);
                        if (tr is null)
                        {
                            examjson.Animation = "examjson2.json";

                            return;
                        }
                        ObservableCollection<ExamClass2> trends = new ObservableCollection<ExamClass2>(tr);
                        dt_exam.Rows.Clear();
                        stk.Children.Clear();
                        CultureInfo us = new CultureInfo("en-US");
                        System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");

                        string time = DateTime.Now.ToString("h:mm:ss tt");
                        double hr1, hr2, mn1, mn2, sc1, sc2;
                        int finsh = 0;
                        DateTime oDate2, tim2, oDate, tim;
                        string mo2, da2, ye2 ;

                      
                        string mo, da, ye  ;
                        string stt = "";

                        for (int i = 0; i < trends.Count; i++)
                        {
                            finsh = 0;
                              oDate2 = trends[i].Dat;
                              mo2 = oDate2.Date.Month.ToString();
                              da2 = oDate2.Date.Day.ToString();
                              ye2 = oDate2.Date.Year.ToString();
                              tim2 = trends[i].Tim;
                            stt = trends[i].Stat; 
                            if (oDate2.Date == DateTime.Now.Date)
                            {
                                hr1 = tim2.TimeOfDay.Hours;
                                hr2 = DateTime.Now.TimeOfDay.Hours;
                                if (hr1 < hr2)
                                {
                                    finsh = 1; 
                                }
                                if (hr1 > hr2)
                                {
                                    goto dod2;
                                }

                                mn1 = tim2.TimeOfDay.Minutes;
                                mn2 = DateTime.Now.TimeOfDay.Minutes;
                                if (mn1 <= mn2)
                                {
                                    finsh = 1;
                                }

                                sc1 = tim2.TimeOfDay.Seconds;
                                sc2 = DateTime.Now.TimeOfDay.Seconds;
                                if ((sc1 < sc2) && (mn1 == mn2))
                                {
                                    finsh = 1;
                                }
 
                            dod2:;
                            }
                            if (oDate2.Date < DateTime.Now.Date)
                            {
                                finsh = 1;
                            }
                            foreach (var aq in trends[i].Questions)
                            {
                                qus.Rows.Add(aq.QId,trends[i].Id);
                            }

                            if (finsh == 1)
                            {
                                stt = "Finish";
                            }
                            dt_exam.Rows.Add(trends[i].Id, trends[i].Dat, trends[i].Tim, trends[i].Des, trends[i].Hor, trends[i].Mark, stt, trends[i].CoursesId);

                            StackLayout frm1 = new StackLayout
                            {
                                Margin = new Thickness(10, 5, 10, 5),
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                BackgroundColor = Color.White,

                            };
                            RoundCornersEffect.SetCornerRadius(frm1, 7);
                            var c1 = new Parm
                            {
                                Ctk = frm1,
                                Qhour = Convert.ToInt32(trends[i].Hor),
                                Qtyp = Convert.ToInt32(trends[i].Hor),
                                Qmark = Convert.ToInt32(trends[i].Mark),
                                Des = trends[i].Des,
                                Qdat =  trends[i].Dat ,
                                Qtim = trends[i].Tim ,
                                E_id = trends[i].Id,
                                Stat = stt,
                            };

                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("stackClick", 0));
                            tapGestureRecognizer.BindingContext = this;
                            tapGestureRecognizer.CommandParameter = c1;
                            frm1.GestureRecognizers.Add(tapGestureRecognizer);


                            StackLayout stk1 = new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                            };

                              oDate = trends[i].Dat;
                              mo = oDate.Date.Month.ToString();
                              da = oDate.Date.Day.ToString();
                              ye = oDate.Date.Year.ToString();
                              tim = trends[i].Tim;


                            string cn = da + "/" + mo + "/" + ye + "  " + tim.TimeOfDay;
                            // Titel
                            Label lb1 = new Label
                            {
                                Margin = 5,
                                Text = cn,
                                TextColor = Color.FromHex("#1941A2"),
                                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                            };

                            //DEscription
                            Label lb2 = new Label
                            {
                                Margin = 5,
                                Text = trends[i].Des,
                                TextColor = Color.FromHex("#393939"),
                                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                                VerticalOptions = LayoutOptions.CenterAndExpand,

                            };


                            //Footer
                            Grid grd1 = new Grid { ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA") };
                            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                            StackLayout st1 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand };
                            Label lbb1_g1 = new Label
                            {
                                Text = trends[i].Hor + " Minute",
                                TextColor = Color.FromHex("#393939"),
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            Label lbb2_g1 = new Label
                            {
                                Text = "Duration",
                                TextColor = Color.FromHex("#AAAAAA"),
                                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            st1.Children.Add(lbb1_g1);
                            st1.Children.Add(lbb2_g1);
                            StackLayout stk_g1 = new StackLayout { Orientation = StackOrientation.Horizontal };
                            stk_g1.Children.Add(st1);
                            BoxView bx = new BoxView
                            {
                                WidthRequest = 1,
                                BackgroundColor = Color.FromHex("#DBDBDB"),
                                Margin = 5,
                            };
                            stk_g1.Children.Add(bx);

                            StackLayout st2 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand };
                            Label lbb1_g2 = new Label
                            {
                                Text = trends[i].Mark.ToString (),
                                TextColor = Color.FromHex("#393939"),
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            Label lbb2_g2 = new Label
                            {
                                Text = "Mark",
                                TextColor = Color.FromHex("#AAAAAA"),
                                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            st2.Children.Add(lbb1_g2);
                            st2.Children.Add(lbb2_g2);
                            StackLayout stk_g2 = new StackLayout { Orientation = StackOrientation.Horizontal };
                            stk_g2.Children.Add(st2);
                            BoxView bx2 = new BoxView
                            {
                                WidthRequest = 1,
                                BackgroundColor = Color.FromHex("#DBDBDB"),
                                Margin = 5,
                            };
                            stk_g2.Children.Add(bx2);



                            Label lbb1_g3 = new Label
                            {
                                Text = trends[i].Stat,
                                TextColor = Color.FromHex("#EF880F"),
                                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                            };

                            if (trends[i].Stat == "Finish")
                            {
                                lbb1_g3.TextColor = Color.FromHex("#1ECE2C");
                            }
                            if (finsh == 1)
                            {
                               
                                lbb1_g3.Text = "Finish";
                                lbb1_g3.TextColor = Color.FromHex("#1ECE2C");
                            }



                            StackLayout stk_g3 = new StackLayout { Orientation = StackOrientation.Horizontal };
                            stk_g3.Children.Add(lbb1_g3);

                            grd1.Children.Add(stk_g1, 0, 0);
                            grd1.Children.Add(stk_g2, 1, 0);
                            grd1.Children.Add(stk_g3, 2, 0);

                            stk1.Children.Add(lb1);
                            stk1.Children.Add(lb2);
                            stk1.Children.Add(grd1);

                            frm1.Children.Add(stk1);
                            stk.Children.Add(frm1);


                        }
                        if (trends.Count == 0)
                        {
                            examjson.Animation = "examjson2.json";

                        }
                        else
                        {
                            examjson.IsVisible = false; 
                        }
                    }
                    catch (Exception eb)
                    {
                    }

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


            //DataRow[] fr = Login.dt_exam.Select("courses_id=" + Course.course_id);
            //CourseName.Text = Course.course_name;
            //if (fr.Length == 0) { examjson.IsVisible = true; DependencyService.Get<IMessage>().ShortAlert("There Are No Exams For This Course"); return; }
            //fr = Login.dt_exam.Select("courses_id=" + Course.course_id);
            //stk.Children.Clear();
            //CultureInfo us = new CultureInfo("en-US");
            //System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");

            //for (int i = 0; i < fr.Length; i++)
            //{
    
            //    StackLayout frm1 = new StackLayout
            //    {
            //        Margin = new Thickness(10, 5, 10, 5),
            //        HorizontalOptions = LayoutOptions.FillAndExpand,
            //        BackgroundColor = Color.White,

            //    };
            //    RoundCornersEffect.SetCornerRadius(frm1, 7);
            //    var c1 = new Parm {
            //        Ctk = frm1,
            //        Qhour = Convert.ToInt32(fr[i]["hor"]),
            //        Qtyp = Convert.ToInt32(fr[i]["hor"]),
            //        Qmark = Convert.ToInt32(fr[i]["mark"]),
            //        Des =  fr[i]["des"].ToString(),
            //        Qdat = DateTime.Parse(fr[i]["dat"].ToString()),
            //        Qtim = DateTime.Parse(fr[i]["tim"].ToString()),

            //    };

          
            //    var tapGestureRecognizer = new TapGestureRecognizer();
            //    tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("stackClick", 0));
            //    tapGestureRecognizer.BindingContext = this;
            //    tapGestureRecognizer.CommandParameter = c1;
            //    frm1.GestureRecognizers.Add(tapGestureRecognizer);


            //    StackLayout stk1 = new StackLayout
            //    {
            //        HorizontalOptions = LayoutOptions.FillAndExpand,
            //        VerticalOptions = LayoutOptions.FillAndExpand,
            //    };

            //    DateTime oDate = DateTime.Parse(fr[i]["dat"].ToString());
            //    string mo = oDate.Date.Month.ToString();
            //    string da = oDate.Date.Day.ToString();
            //    string ye = oDate.Date.Year.ToString();
            //    DateTime tim = DateTime.Parse(fr[i]["tim"].ToString());


            //    string cn = da + "/" + mo + "/" + ye + "  " + tim.TimeOfDay;
            //    // Titel
            //    Label lb1 = new Label
            //    {
            //        Margin = 5,
            //        Text = cn,
            //        TextColor = Color.FromHex("#1941A2"),
            //        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            //    };

            //    //DEscription
            //    Label lb2 = new Label
            //    {
            //        Margin = 5,
            //        Text = fr[i]["des"].ToString(),
            //        TextColor = Color.FromHex("#393939"),
            //        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            //        VerticalOptions = LayoutOptions.CenterAndExpand,

            //    };


            //    //Footer
            //    Grid grd1 = new Grid { ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA") };
            //    grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            //    grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            //    grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            //    StackLayout st1 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand };
            //    Label lbb1_g1 = new Label
            //    {
            //        Text = fr[i]["hor"].ToString() + " Hour",
            //        TextColor = Color.FromHex("#393939"),
            //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    };
            //    Label lbb2_g1 = new Label
            //    {
            //        Text = "Duration",
            //        TextColor = Color.FromHex("#AAAAAA"),
            //        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    };
            //    st1.Children.Add(lbb1_g1);
            //    st1.Children.Add(lbb2_g1);
            //    StackLayout stk_g1 = new StackLayout { Orientation = StackOrientation.Horizontal };
            //    stk_g1.Children.Add(st1);
            //    BoxView bx = new BoxView
            //    {
            //        WidthRequest = 1,
            //        BackgroundColor = Color.FromHex("#DBDBDB"),
            //        Margin = 5,
            //    };
            //    stk_g1.Children.Add(bx);

            //    StackLayout st2 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand };
            //    Label lbb1_g2 = new Label
            //    {
            //        Text = fr[i]["mark"].ToString(),
            //        TextColor = Color.FromHex("#393939"),
            //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    };
            //    Label lbb2_g2 = new Label
            //    {
            //        Text = "Mark",
            //        TextColor = Color.FromHex("#AAAAAA"),
            //        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //    };
            //    st2.Children.Add(lbb1_g2);
            //    st2.Children.Add(lbb2_g2);
            //    StackLayout stk_g2 = new StackLayout { Orientation = StackOrientation.Horizontal };
            //    stk_g2.Children.Add(st2);
            //    BoxView bx2 = new BoxView
            //    {
            //        WidthRequest = 1,
            //        BackgroundColor = Color.FromHex("#DBDBDB"),
            //        Margin = 5,
            //    };
            //    stk_g2.Children.Add(bx2);



            //    Label lbb1_g3 = new Label
            //    {
            //        Text = fr[i]["stat"].ToString(),
            //        TextColor = Color.FromHex("#EF880F"),
            //        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //        VerticalOptions = LayoutOptions.Center,
            //    };

            //    if (fr[i]["stat"].ToString() == "Finish")
            //    {
            //        lbb1_g3.TextColor = Color.FromHex("#1ECE2C");
            //    }

            //    StackLayout stk_g3 = new StackLayout { Orientation = StackOrientation.Horizontal };
            //    stk_g3.Children.Add(lbb1_g3);

            //    grd1.Children.Add(stk_g1, 0, 0);
            //    grd1.Children.Add(stk_g2, 1, 0);
            //    grd1.Children.Add(stk_g3, 2, 0);

            //    stk1.Children.Add(lb1);
            //    stk1.Children.Add(lb2);
            //    stk1.Children.Add(grd1);

            //    frm1.Children.Add(stk1);
            //    stk.Children.Add(frm1);
            //}

        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        async  void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            await stk_Add.FadeTo(0.7, 100);
            await stk_Add.FadeTo(1, 200);
            await Shell.Current.Navigation.PushAsync(new AddExam());
        }

        async void QqBank_Clicked(System.Object sender, System.EventArgs e)
        {
            QqBank.IsEnabled = false; 
            await Shell.Current.Navigation.PushAsync(new QuestionsBank1());
            QqBank.IsEnabled = true ;
        }

        class ExamClass2
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("dat")]
            public DateTime Dat { get; set; }

            [JsonProperty("tim")]
            public DateTime Tim { get; set; }

            [JsonProperty("des")]
            public string Des { get; set; }

            [JsonProperty("hor")]
            public int Hor { get; set; }

            [JsonProperty("mark")]
            public int Mark { get; set; }

            [JsonProperty("stat")]
            public string Stat { get; set; }

            [JsonProperty("courses_id")]
            public int CoursesId { get; set; }

            [JsonProperty("Questions")]
            public Question[] Questions { get; set; }
        }
        class Question
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("q_id")]
            public long QId { get; set; }

            [JsonProperty("exam_id")]
            public long ExamId { get; set; }
        }
        class Parm
        {


            int qtyp, qhour, qmark;
            DateTime qdat;
            DateTime qtim;
            string des;
            int q_id;
            StackLayout ctk;
            int e_id;
            string stat; 
      
            public StackLayout Ctk { get => ctk; set => ctk = value; }
            public int Qmark { get => qmark; set => qmark = value; }
            public int Qtyp { get => qtyp; set => qtyp = value; }
            public int Qhour { get => qhour; set => qhour = value; }
 
            public string Des { get => des; set => des = value; }
            public DateTime Qdat { get => qdat; set => qdat = value; }
            public DateTime Qtim { get => qtim; set => qtim = value; }
            public int E_id { get => e_id; set => e_id = value; }
            public string Stat { get => stat; set => stat = value; }
        }

       async void Addexam_Clicked(System.Object sender, System.EventArgs e)
        {
            Addexam.IsEnabled = false; 
       
            await Shell.Current.Navigation.PushAsync(new AddExam());
            Addexam.IsEnabled = true;
        }

    }
}
