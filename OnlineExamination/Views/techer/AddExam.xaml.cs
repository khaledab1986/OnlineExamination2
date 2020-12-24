using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Http;
using System.Text;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class AddExam : ContentPage
    {
        public static int mark_qus = 0; 

        public static int pic_index;

        public static DataTable dt_selectQ = new DataTable();

        public AddExam()
        {
            InitializeComponent();
            if (dt_selectQ.Columns.Count == 0)
            {

                dt_selectQ.Columns.Add("q_id", typeof(int));
            }
            dt_selectQ.Rows.Clear(); 
            pic_index = -1;
        }

        async  void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync(); 
        }

        protected override   void OnAppearing()
        {
            if (dt_selectQ .Rows .Count > 0)
            {
                img.Source = "oks.png";
            }
            else
            {
                img.Source = "error.png"; 
            }


            mark.Text = mark_qus.ToString();
        }

        async  void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (ExamType.SelectedIndex < 0)
            {
                await DisplayAlert("","Chose Exam Time Befor !!","ok");
            }
            
                else {
                await Shell.Current.Navigation.PushAsync(new ChoseExam()); }
        }

        void ExamType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            Picker pic = sender as Picker;


            pic_index = pic.SelectedIndex ;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (ExamType.SelectedIndex == -1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Select a Type of Exam !");
                return;
            }
            if (Convert.ToInt32(mark.Text) <= 0)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Mark Of Exam");
                mark.Focus();
                return;
            }
            if (Hour.Text == "" || Hour.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Duration Of The Exam  !");
                Hour.Focus();
                return;
            }
            if (Detail.Text == "" || Detail.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Detail Of The Exam  !");
                Detail.Focus();
                return;
            }
            if (dt_selectQ.Rows.Count   == 0  )
            {
                DependencyService.Get<IMessage>().ShortAlert("Select Question From QuestionBank!");
                return;
            }


            var action = await DisplayAlert("Online Examination", "Add New Exam" + " ? ", "Yes", "No");
            if (action)
            {
                saveBut.IsEnabled = false;
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        try
                        {
                            NativeMessageHandler ttk = new NativeMessageHandler { UseProxy = false };
                            HttpClient _client = new HttpClient(ttk);
                            _client.Timeout = TimeSpan.FromMilliseconds(60000);

                            string dtt = dateExam.Date.Year.ToString () + "-" + dateExam.Date.Month.ToString () + "-" + dateExam.Date.Day.ToString ();
                            string cont = "?mark=" + mark.Text + "&q_type=" + ExamType.SelectedIndex.ToString() + "&Des=" + Detail.Text  + "&hour=" + Hour.Text  + "&dat=" + dtt + "&tim=" + timeExam.Time + "&course_id=" + Course.course_id;
                            string Url = "https://onlineexamination.a2hosted.com/OnlineExamination/insert_Exam.php" + cont;

                            DataRow[] fr2 = dt_selectQ.Select();
                            List<SaveAnswer> list = new List<SaveAnswer> { };
                            for (int i = 0; i < fr2.Length; i++)
                            {
                                list.Add(new SaveAnswer
                                {
                                    QId = fr2[i]["q_id"].ToString(),
                                });
                            }
                            var myContent = JsonConvert.SerializeObject(list);
                            _client.DefaultRequestHeaders.Clear();
                            _client.DefaultRequestHeaders.Add("Accept", "application/json;charset=utf-8");
                            _client.DefaultRequestHeaders.Add("Accept-Language", "ar");
                            var request = new HttpRequestMessage()
                            {
                                RequestUri = new Uri(Url),
                                Method = HttpMethod.Post,
                            };
                            request.Content = new StringContent(myContent, Encoding.UTF8, "application/json");//CONTENT-TYPE header
                          

                            var resp = await _client.SendAsync(request);
                            if (resp.IsSuccessStatusCode)
                            {

                                string ddt = dtt.ToString() + " Time : " + timeExam.Time.ToString();
                                send_nav(Course.course_name, ddt);
                                await Shell.Current.Navigation.PopAsync();
                                
                            }
                            else
                            {
                                DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");

                            }

                        }
                        catch (Exception er)
                        {
                            string tt = er.Message;
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
                saveBut.IsEnabled = true;
            }
        }

        async void send_nav(string curs,string dat)
        {
          
            //var current = Connectivity.NetworkAccess;
            //if (current == NetworkAccess.Internet)
            //{
            //    if (CrossConnectivity.Current.IsConnected)
            //    {
            //        try
            //        {
            //            App.user_name = "";
            //            string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_tok.php";
            //            var content = await App.con.GetStringAsync(tt);
            //            var tr = JsonConvert.DeserializeObject<IList<GetTok>>(content);
            //            if (tr is null)
            //            {
            //                return;
            //            }
            //            ObservableCollection<GetTok> trends = new ObservableCollection<GetTok>(tr);
            //            IList<string> List_tok = new List<string>();
            //            for (int i = 0; i < trends.Count; i++)
            //            {
            //                if (trends[i].Tok != null)
            //                {
            //                    if (trends[i].Tok != "")
            //                    {
            //                        List_tok.Add(trends[i].Tok);
            //                    }
            //                }
            //            }
 


            //            string msg = "New Exam : " + curs + " / " + dat;
 

            //            if (List_tok.Count > 0) { App.firebase_send_noti(List_tok, msg); }
            //        }
            //        catch (Exception ee)
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

        class GetTok
        {
            [JsonProperty("tok")]
            public string Tok { get; set; }
        }


        class SaveAnswer
        {


            [JsonProperty("q_id")]
            public string QId { get; set; }
 
        }
        //async void Reffsh()
        //{
        //    var current = Connectivity.NetworkAccess;
        //    if (current == NetworkAccess.Internet)
        //    {
        //        if (CrossConnectivity.Current.IsConnected)
        //        {
        //            try
        //            {
        //                App.user_name = "";
        //                string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/Ref_Exam.php?courses_id=" + Course.course_id;
        //                var content = await App.con.GetStringAsync(tt);
        //                var tr = JsonConvert.DeserializeObject<IList<ExamClass2>>(content);
        //                if (tr is null)
        //                {
        //                    DependencyService.Get<IMessage>().ShortAlert("User Name or Password Is Incorrect");
        //                    return;
        //                }
        //                ObservableCollection<ExamClass2> trends = new ObservableCollection<ExamClass2>(tr);
        //                int fin = 0;
        //                Login.dt_exam.Rows.Clear();
        //                for (int i = 0; i < trends.Count; i++)
        //                {
        //                    Login.dt_exam.Rows.Add(trends[i].Id, trends[i].Dat, trends[i].Tim, trends[i].Des, trends[i].Hor, trends[i].Mark, trends[i].Stat, trends[i].CoursesId);
        //                }
        //            }
        //            catch (Exception ee)
        //            {
        //            }

        //        }
        //        else
        //        {
        //            DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
        //        }
        //    }
        //    else
        //    {
        //        DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");
        //    }
        //}
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

    }
}
