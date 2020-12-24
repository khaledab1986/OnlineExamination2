using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Input;
using Newtonsoft.Json;
using OnlineExamination.Views.techer;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;
 

namespace OnlineExamination.Views.Student
{
    public partial class ExamView_s : ContentPage
    {
        public static string user_nickname = "" ;
        public static DataTable dt_selectQ = new DataTable();
        public static DataTable dt_exam = new DataTable();
        public static DataTable qus = new DataTable();
        public static DataTable Answ = new DataTable();
        public static  int mark_of_Exam = 0;
        public ICommand stackClick { get; private set; }
        public static int EID;
        public static int ETime;
        public ExamView_s()
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
                dt_exam.Columns.Add("course_name", typeof(string));
            }
            if (qus.Columns.Count == 0)
            {
                qus.Columns.Add("exam_id", typeof(int));
                qus.Columns.Add("q_id", typeof(int));
                qus.Columns.Add("q_name", typeof(string));
                qus.Columns.Add("q_text", typeof(string));
                qus.Columns.Add("q_type", typeof(int));
                qus.Columns.Add("q_mark", typeof(int));
                qus.Columns.Add("course_id", typeof(int));
            }
            if (Answ.Columns.Count == 0)
            {
                Answ.Columns.Add("exam_id", typeof(int));
                Answ.Columns.Add("id", typeof(int));
                Answ.Columns.Add("a_text", typeof(string));
                Answ.Columns.Add("a_correct", typeof(int));
                Answ.Columns.Add("q_id", typeof(int));
 

            }
            string nName = "";
            DataRow[] fr;
            fr = Login.dt_user.Select();
            if (fr.Length > 0)
            {
                nName = fr[0]["User_nickname"].ToString();
            }
            fr = Login.dt_course.Select();
            Stud_name.Text = nName ;
            user_nickname = nName; 
         }
        async void OnTapped(Parm tt2)
        {
            EID = tt2.Idd;
            ETime = tt2.Hor;
            StackLayout ck = tt2.Ctk;
            await ck.FadeTo(0.7, 100);
            await ck.FadeTo(1, 200);

            DataRow[] fr = dt_exam.Select("id=" + EID);
            if (fr.Length > 0)
            {
                mark_of_Exam = Convert.ToInt32(fr[0]["mark"].ToString());
            }


            await Shell.Current.Navigation.PushAsync(new Start_Exam_View());
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
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/Ref_Exam.php?stud=1";
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<ExamClass2>>(content);
                        if (tr is null)
                        {
                            examjson.Animation = "examjson2.json";

                            return;
                        }
                        ObservableCollection<ExamClass2> trends = new ObservableCollection<ExamClass2>(tr);
                        dt_exam.Rows.Clear();
                        qus.Rows.Clear();
                        Answ.Rows.Clear();
                        stk.Children.Clear();
                        CultureInfo us = new CultureInfo("en-US");
                        System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");
                        string time = DateTime.Now.ToString("h:mm:ss tt");
                        double hr1, hr2,mn1,mn2,sc1,sc2;
                        for (int i = 0; i < trends.Count; i++)
                        {
                            DateTime oDate = trends[i].Dat;
                            string mo = oDate.Date.Month.ToString();
                            string da = oDate.Date.Day.ToString();
                            string ye = oDate.Date.Year.ToString();
                            DateTime tim = trends[i].Tim;
                            if (oDate.Date == DateTime.Now.Date   )
                            {
                                hr1 = tim.TimeOfDay.Hours ;
                                hr2 = DateTime.Now.TimeOfDay.Hours;
                                if (hr1 < hr2)
                                {
                                    goto dod;
                                }
                                if (hr1 > hr2)
                                {
                                    goto dod2;
                                }

                                mn1 = tim.TimeOfDay.Minutes;
                                mn2 = DateTime.Now.TimeOfDay.Minutes;
                                if (mn1 <= mn2)
                                {
                                    goto dod;
                                }
                                sc1 = tim.TimeOfDay.Seconds;
                                sc2 = DateTime.Now.TimeOfDay.Seconds;
                                if ((sc1 < sc2) && (mn1 == mn2))
                                {
                                    goto dod;
                                }
                            dod2:;
                            }

                            dt_exam.Rows.Add(trends[i].Id, trends[i].Dat, trends[i].Tim, trends[i].Des, trends[i].Hor, trends[i].Mark, trends[i].Stat, trends[i].CoursesId,trends[i].QType,trends[i].CourseName);
                            foreach (var aq in trends[i].Questions)
                            {
                                qus.Rows.Add(trends[i].Id,aq.QId, aq.QName,aq.QText,aq.QType,aq.QMark,aq.CourseId);
                  
                                foreach (var an in aq.Answers)
                                {
                                    Answ.Rows.Add(trends[i].Id,an.Id, an.AText, an.ACorrect, an.QId); 
                                }
                            }
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
                                Hor = trends[i].Hor,
                                Idd = trends[i].Id,
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




                            string cn = da + "/" + mo + "/" + ye  ;// "            " + tim.TimeOfDay;
                            // Titel

                            StackLayout stk_dat = new StackLayout
                            {
                                 Orientation = StackOrientation.Horizontal,
                                 HorizontalOptions = LayoutOptions.FillAndExpand,
                            };


                            Label lbd1 = new Label
                            {
                                Margin = 5,
                                Text = cn,
                                VerticalOptions = LayoutOptions.Center,
                                TextColor = Color.FromHex("#1941A2"),
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                FontSize =17,// Device.GetNamedSize(NamedSize.Large, typeof(Label))
                            };
                            Label lbd2 = new Label
                            {
                                Margin = 5,
                                VerticalOptions=LayoutOptions.Center,
                                Text = tim.TimeOfDay.ToString (),
                                TextColor = Color.FromHex("#1941A2"),
                                HorizontalOptions = LayoutOptions.Center,
                                FontSize = 17,// Device.GetNamedSize(NamedSize.Large, typeof(Label))
                            };
                            stk_dat.Children.Add(lbd1);
                            stk_dat.Children.Add(lbd2);

                            //DEscription
                            Label lb2 = new Label
                            {
                                Margin = 5,
                                Text = trends[i].Des,
                                TextColor = Color.FromHex("#393939"),
                                FontSize =14,// Device.GetNamedSize(NamedSize.Small, typeof(Label)),
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
                                FontSize =17,// Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            Label lbb2_g1 = new Label
                            {
                                Text = "Duration",
                                TextColor = Color.FromHex("#AAAAAA"),
                                FontSize =14,// Device.GetNamedSize(NamedSize.Small, typeof(Label)),
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
                                Text = trends[i].Mark.ToString(),
                                TextColor = Color.FromHex("#393939"),
                                FontSize =17,// Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                            };
                            Label lbb2_g2 = new Label
                            {
                                Text = "Mark",
                                TextColor = Color.FromHex("#AAAAAA"),
                                FontSize =14,// Device.GetNamedSize(NamedSize.Small, typeof(Label)),
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
                                Text = trends[i].CourseName,
                                TextColor = Color.FromHex("#1941A2"),
                                FontSize =16,// Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                            };

 
                            StackLayout stk_g3 = new StackLayout { Orientation = StackOrientation.Horizontal };
                            stk_g3.Children.Add(lbb1_g3);

                            grd1.Children.Add(stk_g1, 0, 0);
                            grd1.Children.Add(stk_g2, 1, 0);
                            grd1.Children.Add(stk_g3, 2, 0);

                            stk1.Children.Add(stk_dat);
                            stk1.Children.Add(lb2);
                            stk1.Children.Add(grd1);

                            frm1.Children.Add(stk1);
                            stk.Children.Add(frm1);

                        dod:;
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
                        await DisplayAlert("", eb.Message, "ok");
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


 
        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        async void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            //await stk_Add.FadeTo(0.7, 100);
            //await stk_Add.FadeTo(1, 200);
            await Shell.Current.Navigation.PushAsync(new AddExam());
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

             [JsonProperty("course_name")]
            public string  CourseName { get; set; }

            [JsonProperty("q_type")]
            public int QType { get; set; }
            //q_type
        }
        class Question
        {
            [JsonProperty("q_id")]
            public long QId { get; set; }

            [JsonProperty("q_name")]
            public string QName { get; set; }

            [JsonProperty("q_text")]
            public string QText { get; set; }

            [JsonProperty("q_type")]
            public long QType { get; set; }

            [JsonProperty("q_mark")]
            public long QMark { get; set; }

            [JsonProperty("course_id")]
            public long CourseId { get; set; }

            [JsonProperty("Answers")]
            public Answer[] Answers { get; set; }
        }
        public partial class Answer
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("a_text")]
            public string AText { get; set; }

            [JsonProperty("a_correct")]
            public long ACorrect { get; set; }

            [JsonProperty("q_id")]
            public long QId { get; set; }
        }
        class Parm
        {
            int hor; 
            int idd;
            StackLayout ctk;
            public int Idd { get => idd; set => idd = value; }
            public StackLayout Ctk { get => ctk; set => ctk = value; }
            public int Hor { get => hor; set => hor = value; }
        }
    }
}
