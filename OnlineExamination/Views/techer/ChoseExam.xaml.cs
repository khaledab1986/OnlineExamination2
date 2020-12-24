using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Lottie.Forms;
using Newtonsoft.Json;
using OnlineExamination.Models;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class ChoseExam : ContentPage
    {
        public static DataTable dt_Question = new DataTable();
        public static DataTable dt_Answer = new DataTable();

        public static int pic_index = -1;
        public static int QCount;
        public static int QID;
        public ICommand SelectQ { get; private set; }

        public ChoseExam()
        {
            InitializeComponent();
            SelectQ = new Command<Parm>((Parm parameter) => OnTapped(parameter));

            if (dt_Question.Columns.Count == 0)
            {
                dt_Question.Columns.Add("q_id", typeof(int));
                dt_Question.Columns.Add("q_name", typeof(string));
                dt_Question.Columns.Add("q_text", typeof(string));
                dt_Question.Columns.Add("q_type", typeof(int));
                dt_Question.Columns.Add("q_mark", typeof(int));
                dt_Question.Columns.Add("course_id", typeof(int));

            }

            if (dt_Answer.Columns.Count == 0)
            {
                dt_Answer.Columns.Add("id", typeof(int));
                dt_Answer.Columns.Add("a_text", typeof(string));
                dt_Answer.Columns.Add("a_correct", typeof(int));
                dt_Answer.Columns.Add("q_id", typeof(int));
            }
   
            examjson.Animation = "loading.json";
        }

        async void OnTapped(Parm tt2)
        {
            QID = tt2.Q_id;
            ImageButton ck = tt2.Ctk;
            StackLayout man = tt2.Stk_main; 
            await ck.FadeTo(0.7, 100);
            await ck.FadeTo(1, 200);

            DataRow[] fr = AddExam.dt_selectQ.Select("q_id=" + QID);
            if (fr.Length == 0)
            {
                AddExam.dt_selectQ.Rows.Add(QID); 
                ck.Source = "choseok.png";
                man.BackgroundColor = Color.FromHex("#EBFFF3");
                AddExam.mark_qus = AddExam.mark_qus + tt2.Q_mark ;
            }
            if (fr.Length == 1)
            {
                DataRow[] fr2 = AddExam.dt_selectQ.Select();
                for (int i = 0; i < fr2.Length; i++)
                {
                    if (Convert.ToInt32(fr[i]["q_id"].ToString()) == QID)
                    {
                        AddExam.dt_selectQ.Rows.RemoveAt(i);
                        break;
                    }
                }
                ck.Source = "choseno.png";
                man.BackgroundColor = Color.White;
                AddExam.mark_qus = AddExam.mark_qus - tt2.Q_mark;

            }



        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();


        }

        protected override async void OnAppearing()
        {
            //base.OnAppearing();




            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        App.user_name = "";
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_qustionbank.php?course_id=" + Course.course_id;

                        var content = await App.con.GetStringAsync(tt);
                        // Temperatures5 obj = JsonConvert.DeserializeObject<Temperatures5>(content);
                        var tr = JsonConvert.DeserializeObject<IList<Questions_b>>(content);

                        // string json = App.con.GetStringAsync(tt).Result;
                        // var tr = JsonConvert.DeserializeObject<IList<Questions_b>>(json);
                        if (tr is null)
                        {
                            examjson.Animation = "examjson2.json";
                            return;
                        }

                        ObservableCollection<Questions_b> trends = new ObservableCollection<Questions_b>(tr);
                        dt_Answer.Rows.Clear();
                        dt_Question.Rows.Clear();
                        for (int i = 0; i < trends.Count; i++)
                        {
                            dt_Question.Rows.Add(trends[i].QId, trends[i].QName, trends[i].QText, trends[i].QType, trends[i].QMark, trends[i].CourseId);
                            foreach (var a in trends[i].Answers)
                            {
                                dt_Answer.Rows.Add(a.Id, a.AText, a.ACorrect, a.QId);
                            }
                        }

                        examjson.IsVisible = false;
                        if (AddExam.pic_index >= 0)
                        {
                            refrsh(AddExam.pic_index );
                        }
                        else
                        {
                            refrsh(-1);
                        }

                    }
                    catch (Exception ee)
                    {
                        string tt = ee.Message;
                        await DisplayAlert("", tt, "ok");
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

        void refrsh(int ty)
        {
            stk.Children.Clear();
            DataRow[] fr;
            if (ty >= 0)
            {

                fr = dt_Question.Select("q_type=" + ty);


            }
            else
            {
                fr = dt_Question.Select();

            }
            string typ = "";


            DataRow[] fr2;
            int ctm = 0;
            for (int i = 0; i < fr.Length; i++)
            {
                //Frame frm1 = new Frame
                //{
                //    Margin = new Thickness(10, 5, 10, 5),
                //    Padding = 0,
                //    CornerRadius = 7,
                //    HasShadow = true,
                //    Visual = VisualMarker.Default,
                //    HeightRequest = 180,
                //    HorizontalOptions = LayoutOptions.FillAndExpand,
                //    BackgroundColor = Color.White,
                //    BorderColor = Color.FromHex("#D6D6D6"),
                //};
                fr2 = AddExam.dt_selectQ.Select("q_id=" + fr[i]["q_id"].ToString ());
                if (fr2.Length ==1)
                {
                    ctm = 1; 
                }
                else
                {
                    ctm = 0; 
                }
                StackLayout frm1 = new StackLayout
                {
                    Margin = new Thickness(10, 5, 10, 5),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.White,
                
                };

                if (ctm == 0)
                {
                    frm1.BackgroundColor = Color.White;
                }
                else
                {
                    frm1.BackgroundColor = Color.FromHex("#EBFFF3");
                }

                RoundCornersEffect.SetCornerRadius(frm1, 7);


                StackLayout stk1 = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                     
                };

               

       


                StackLayout stkk1 = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand, Orientation = StackOrientation.Vertical};
                //DEscription
                Label lb2 = new Label
                {
                    Margin = 5,
                    Text = fr[i]["q_text"].ToString(),
                    TextColor = Color.FromHex("#393939"),
                    FontSize = 16,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                ImageButton img = new ImageButton
                {
                   
                    HeightRequest = 30 ,
                    WidthRequest = 30,
                    Margin = new Thickness (5,3,5,0),
                    Aspect = Aspect.AspectFit ,
                    VerticalOptions = LayoutOptions.Start ,
                    HorizontalOptions = LayoutOptions .End,
                };

                if (ctm == 0)
                {
                    img.Source = "choseno.png";
                }
                else
                {
                    img.Source = "choseok.png"; 
                }
                img.BindingContext = this;
                img.SetBinding(ImageButton.CommandProperty, new Binding("SelectQ", 0));
                var c1 = new Parm { Q_mark = Convert.ToInt32(fr[i]["q_mark"].ToString()), Q_id =Convert.ToInt32( fr[i]["q_id"].ToString ()) , Ctk = img, Stk_main= frm1 };
                img.CommandParameter = c1;


                stkk1.Children.Add(img);
                stkk1.Children.Add(lb2);
               

                //Footer
                Grid grd1 = new Grid { ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA") };
                grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                StackLayout st1 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand };

                typ =   ExamType.Items[AddExam.pic_index];
                Label lbb1_g1 = new Label
                {
                    Text = typ,
                    TextColor = Color.FromHex("#393939"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };
                Label lbb2_g1 = new Label
                {
                    Text = "Type",
                    TextColor = Color.FromHex("#AAAAAA"),
                    FontSize = 13,
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
                    Text = fr[i]["q_mark"].ToString(),
                    TextColor = Color.FromHex("#393939"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                };
                Label lbb2_g2 = new Label
                {
                    Text = "Mark",
                    TextColor = Color.FromHex("#AAAAAA"),
                    FontSize = 13,
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
                    Text = fr[i]["q_name"].ToString(),
                    TextColor = Color.FromHex("#1941A2"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                };



                StackLayout stk_g3 = new StackLayout { Orientation = StackOrientation.Horizontal };
                stk_g3.Children.Add(lbb1_g3);

                grd1.Children.Add(stk_g1, 0, 0);
                grd1.Children.Add(stk_g2, 1, 0);
                grd1.Children.Add(stk_g3, 2, 0);


                stk1.Children.Add(stkk1);
                stk1.Children.Add(grd1);

                frm1.Children.Add ( stk1);
                stk.Children.Add(frm1);
            }
            if (fr.Length == 0)
            {
                AnimationView nose = new AnimationView
                {
                    Animation = "examjson2.json",
                    AutoPlay = true,
                    IsVisible = true,
                    Loop = true,
                    WidthRequest = 300,
                    HeightRequest = 300,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                stk.Children.Add(nose);
            }


        }

        public void myPickerSelectedIndexChanged(object sender, EventArgs e)
        {

            Picker pic = sender as Picker;


            refrsh(pic.SelectedIndex);





        }

        class Questions_b
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
            public Answer_b[] Answers { get; set; }
        }

        class Answer_b
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
            int q_mark;
            int q_id;
            ImageButton ctk;
            StackLayout stk_main;
            public int Q_id { get => q_id; set => q_id = value; }
            public ImageButton Ctk { get => ctk; set => ctk = value; }
            public StackLayout Stk_main { get => stk_main; set => stk_main = value; }
            public int Q_mark { get => q_mark; set => q_mark = value; }
        }

        async void examjson_OnClick(System.Object sender, System.EventArgs e)
        {
            if (AddExam.pic_index  >= 0)
            {
                pic_index = AddExam.pic_index ;

            }
            else
            {
                pic_index = -1;
            }
            QCount = dt_Question.Rows.Count;
            await Shell.Current.Navigation.PushAsync(new AddQuestions());
        }
    }
}
