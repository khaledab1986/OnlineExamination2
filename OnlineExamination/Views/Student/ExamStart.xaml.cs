using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.Student
{
    public partial class ExamStart : ContentPage
    {
        public ICommand CorrectAnswer { get; private set; }

        public static DataTable dt_q_answer = new DataTable();

        DataRow[] q_fr = ExamView_s.qus.Select("exam_id=" + ExamView_s.EID);

        int ini = 0;
        int val = 0;
        int test_add = 0;
        int qid = 0; 

        protected override  void OnAppearing()
        {
            dt_q_answer.Rows.Clear();
            get_qus(ini);   
        }
        public ExamStart()
        {
            InitializeComponent();
            CorrectAnswer = new Command<Parm1>((Parm1 parameter) => OnTapped2(parameter));
            if (dt_q_answer.Columns.Count == 0)
            {
                dt_q_answer.Columns.Add("q_id", typeof(int));
                dt_q_answer.Columns.Add("id", typeof(int));
            }
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                //val = val +1 ;
                // TimeSpan result = TimeSpan.FromHours(val);
                // ETimer.Text = result.ToString("hh':'mm");
                val = val + 1 ;
                TimeSpan time = TimeSpan.FromSeconds(val);
                ETimer.Text = time.ToString(@"hh\:mm\:ss");
                int hour = (time.Hours * 60 );
                int mint = time.Minutes;
                if ((hour+ mint) == ExamView_s.ETime)
                {
                    //Shell.Current.Navigation.PushAsync(new FinishExam());
                    post_finish();
                    return false ;
                }
                return true ;
            }) ;
        }

        void get_qus(int ii)
        {
            test_add = 0;
            stk_answer.Children.Clear();
            QuestionText.Text = q_fr[ii]["q_text"].ToString();
            DataRow[] fr ;
            DataRow[] cr;
            qid = Convert.ToInt32(q_fr[ii]["q_id"].ToString());
            qus_Name.Text = "Question -" +( ii+1 ); 
            fr = ExamView_s.Answ.Select("exam_id=" + ExamView_s.EID + " and q_id=" + Convert.ToInt32( q_fr[ii]["q_id"].ToString ()));
            if (fr.Length > 0)
            {
                Grid grd1 = new Grid {RowSpacing=5, ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA") };
                grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                for (int i = 0; i < fr.Length; i++)
                {
                    grd1.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    StackLayout stk_c = new StackLayout { Orientation = StackOrientation.Horizontal, Margin = 5 };
                    ImageButton img2 = new ImageButton
                    {
                        Source = "choise1.png",
                        HeightRequest = 30,
                        WidthRequest = 30,
                        BackgroundColor = Color.Transparent,
                    };
                    cr = dt_q_answer.Select("q_id=" + Convert.ToInt32(q_fr[ii]["q_id"].ToString()) + " and id=" + Convert.ToInt32(fr[i]["id"].ToString()) );
                    if (cr.Length ==1 )
                    {
                        img2.Source = "choise2.png"; 
                    }
                    var c1 = new Parm1 {Img = img2, A_id = Convert.ToInt32(fr[i]["id"].ToString()),Q_id=Convert.ToInt32(q_fr[ii]["q_id"].ToString()) };
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("CorrectAnswer", 0));
                    tapGestureRecognizer.BindingContext = this;
                    tapGestureRecognizer.CommandParameter = c1;

                    img2.BindingContext = this;
                    img2.SetBinding(ImageButton.CommandProperty, new Binding("CorrectAnswer", 0));
                    img2.CommandParameter = c1;


                    stk_c.GestureRecognizers.Add(tapGestureRecognizer);
                    Label lb1 = new Label
                    {
                        VerticalOptions = LayoutOptions.Center ,
                        Margin = new Thickness(5,0,5,0),
                        Text = fr[i]["a_text"].ToString(),
                        TextColor = Color.FromHex("#2E2E2E"),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    };
                    stk_c.Children.Add(lb1);
                    stk_c.Children.Add(img2);
                    grd1.Children.Add(stk_c, 0, i);
                }
                stk_answer.Children.Add(grd1);
                if (ii == 0)
                {
                    prb_but.IsVisible = false;
                    nxt_but.IsVisible = true;
                    finish_but.IsVisible = false;
                }
                if (ii > 0)
                {
                    prb_but.IsVisible = true;
                    nxt_but.IsVisible = true;
                    finish_but.IsVisible = false;
                }
                if (ii == (q_fr.Length-1) )
                {
                    prb_but.IsVisible = true;
                    nxt_but.IsVisible = false;
                    finish_but.IsVisible = true;
                }
            }
        }

        async void OnTapped2(Parm1 tt2)
        {
            test_add = 1;
            await tt2.Img.FadeTo(0.7, 100);
            await tt2.Img.FadeTo(1, 200);

            if (dt_q_answer.Rows.Count == 0)
            {
                dt_q_answer.Rows.Add(tt2.Q_id, tt2.A_id);
                
            }
            else
            {
                int ts = 0;
                try
                {
                    DataRow[] fr = dt_q_answer.Select();
                    for (int i = 0; i < fr.Length; i++)
                    {
                        if (Convert.ToInt32(fr[i]["q_id"].ToString()) == tt2.Q_id)
                        {
                            dt_q_answer.Rows.RemoveAt(i);
                            break;
                        }
                    }
                    ts = 1;
                }
                catch
                {
                    ts = 0;
                }
                if (ts == 0)
                {
                    try
                    {
                        DataRow[] fr = dt_q_answer.Select();
                        for (int i = 0; i < fr.Length; i++)
                        {
                            if (Convert.ToInt32(fr[i]["q_id"].ToString()) == tt2.Q_id)
                            {
                                dt_q_answer.Rows.RemoveAt(i);
                                break;
                            }
                        }
                        ts = 1;
                    }
                    catch
                    {
                        ts = 0;
                    }

                }
                if (ts == 1) {
                    dt_q_answer.Rows.Add(tt2.Q_id, tt2.A_id);
                }
            }
            get_qus(ini);
        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
           // await Shell.Current.Navigation.PopAsync();
        }

        class Parm1
        {
            ImageButton img;
            int a_id;
            int q_id;
            public int A_id { get => a_id; set => a_id = value; }
            public int Q_id { get => q_id; set => q_id = value; }
            public ImageButton Img { get => img; set => img = value; }
        }

        void prb_but_Clicked(System.Object sender, System.EventArgs e)
        {
            ini = ini - 1;
            get_qus(ini);
        }

        void nxt_but_Clicked(System.Object sender, System.EventArgs e)
        {

            DataRow[] fr = dt_q_answer.Select("q_id=" + qid);
            if (fr.Length == 0)
            {
                dt_q_answer.Rows.Add(qid, 0);

            }


            ini = ini + 1;
            get_qus(ini);

        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void finish_but_Clicked(System.Object sender, System.EventArgs e)
        {
            post_finish();
            //this.IsEnabled = false ;

            //var current = Connectivity.NetworkAccess;
            //if (current == NetworkAccess.Internet)
            //{
            //    if (CrossConnectivity.Current.IsConnected)
            //    {
            //        try
            //        {
            //        ret:;

            //            int s_mrk;
            //            s_mrk = 0;
            //            DataRow[] fr = dt_q_answer.Select();
            //            DataRow[] fr2;
            //            DataRow[] fr3;
            //            for (int i = 0; i < fr.Length; i++)
            //            {
            //                fr2 = ExamView_s.Answ.Select("q_id=" + Convert.ToInt32(fr[i]["q_id"].ToString()) + " and id=" + Convert.ToInt32(fr[i]["id"].ToString()) + " and a_correct =1 ");
            //                if (fr2.Length > 0)
            //                {
            //                    fr3 = ExamView_s.qus.Select("q_id=" + Convert.ToInt32(fr[i]["q_id"].ToString()));
            //                    if (fr3.Length == 1)
            //                    {
            //                        s_mrk = s_mrk + Convert.ToInt32(fr3[0]["q_mark"].ToString());
            //                    }
            //                }
            //            }

            //            double suc = (ExamView_s.mark_of_Exam / 2);
            //            int stat = 0; 
            //            if (s_mrk >= suc)
            //            {
            //                stat = 1; 
            //            }
            //            string cont = "?Exam_id=" + ExamView_s.EID + "&student_name=" + ExamView_s.user_nickname + "&student_result=" + s_mrk + "&student_successful=" + stat;
            //            string json = App.con.GetStringAsync("https://onlineexamination.a2hosted.com/OnlineExamination/Insert_Exam_Result.php" + cont).Result;
            //            if (json == "" || json is null) { return; }
            //            var tr = JsonConvert.DeserializeObject<test_insert>(json);
            //            if (tr.Id == -1 )
            //            {
            //                goto ret;
            //            }

            //            await Shell.Current.Navigation.PushAsync(new FinishExam());
            //        }
            //        catch
            //        {

            //        }
            //    }
            //    else
            //    {
            //        DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
            //    }
            //}
            //else
            //{
            //    DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
            //}



        }

       async void post_finish()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                    ret:;
                        int s_mrk;
                        s_mrk = 0;
                        DataRow[] fr = dt_q_answer.Select();
                        DataRow[] fr2;
                        DataRow[] fr3;
                        for (int i = 0; i < fr.Length; i++)
                        {
                            fr2 = ExamView_s.Answ.Select("q_id=" + Convert.ToInt32(fr[i]["q_id"].ToString()) + " and id=" + Convert.ToInt32(fr[i]["id"].ToString()) + " and a_correct =1 ");
                            if (fr2.Length > 0)
                            {
                                fr3 = ExamView_s.qus.Select("q_id=" + Convert.ToInt32(fr[i]["q_id"].ToString()));
                                if (fr3.Length == 1)
                                {
                                    s_mrk = s_mrk + Convert.ToInt32(fr3[0]["q_mark"].ToString());
                                }
                            }
                        }

                        double suc = (ExamView_s.mark_of_Exam / 2);
                        int stat = 0;
                        if (s_mrk >= suc)
                        {
                            stat = 1;
                        }
                        string cont = "?Exam_id=" + ExamView_s.EID + "&student_name=" + ExamView_s.user_nickname + "&student_result=" + s_mrk + "&student_successful=" + stat;
                        string json = App.con.GetStringAsync("https://onlineexamination.a2hosted.com/OnlineExamination/Insert_Exam_Result.php" + cont).Result;
                        if (json == "" || json is null) { return; }
                        var tr = JsonConvert.DeserializeObject<test_insert>(json);
                        if (tr.Id == -1)
                        {
                            goto ret;
                        }

                        await Shell.Current.Navigation.PushAsync(new FinishExam());
                    }
                    catch
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
        }

        class test_insert
        {
            int id;
            [JsonProperty(PropertyName = "id")]
            public int Id { get => id; set => id = value; }
            [JsonProperty(PropertyName = "stat")]
            public string Stat { get => stat; set => stat = value; }
            string stat;
        }
    }
}
