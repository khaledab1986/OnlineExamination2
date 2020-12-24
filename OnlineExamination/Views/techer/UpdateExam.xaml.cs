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
    public partial class UpdateExam : ContentPage
    {
        public static DataTable dt_selectQ = new DataTable();

        public UpdateExam()
        {
            InitializeComponent();
            ExamType.SelectedIndex = ExamView.qtyp;
            Detail.Text = ExamView.des;
            dateExam.Date = ExamView.qdat;
            timeExam.Time = ExamView.qtim.TimeOfDay;
            Hour.Text = ExamView.qhour.ToString();
            mark.Text = ExamView.qmark.ToString();
            img.Source = "oks.png";
            if (dt_selectQ.Columns.Count == 0)
            {
                dt_selectQ.Columns.Add("q_id", typeof(int));
            }
            dt_selectQ.Rows.Clear();
            DataRow[] vr = ExamView.qus.Select("");
            for (int i = 0; i < vr.Length; i++)
            {
                dt_selectQ.Rows.Add(vr[i]["q_id"]); 
            }

        }

       async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        void ExamType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
        }

       async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (ExamType.SelectedIndex < 0)
            {
                await DisplayAlert("", "Chose Exam Time Befor !!", "ok");
            }

            else
            {
                await Shell.Current.Navigation.PushAsync(new ChoseExam());
            }
        }

        async  void saveBut_Clicked(System.Object sender, System.EventArgs e)
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
            if (dt_selectQ.Rows.Count == 0)
            {
                DependencyService.Get<IMessage>().ShortAlert("Select Question From QuestionBank!");
                return;
            }


            var action = await DisplayAlert("Online Examination", "Update Exam" + " ? ", "Yes", "No");
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

                            string dtt = dateExam.Date.Year.ToString() + "-" + dateExam.Date.Month.ToString() + "-" + dateExam.Date.Day.ToString();
                            string cont = "?EId=" + ExamView.Eid + "&mark=" + mark.Text + "&q_type=" + ExamType.SelectedIndex.ToString() + "&Des=" + Detail.Text + "&hour=" + Hour.Text + "&dat=" + dtt + "&tim=" + timeExam.Time + "&course_id=" + Course.course_id;
                            string Url = "https://onlineexamination.a2hosted.com/OnlineExamination/update_Exam.php" + cont;

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
                                string QuizName = "";
                                if (ExamType.SelectedIndex >= 0)
                                {
                                    switch (ExamType.SelectedIndex)
                                    {
                                        case 0:
                                            QuizName = "Test1";
                                            break;
                                        case 1:
                                            QuizName = "Test2";
                                            break;
                                        case 2:
                                            QuizName = "Final";
                                            break;
                                    }
                                }
                                string msg = "Update Exam : " + Course.course_name + " / " + QuizName;
                                send_nav( msg);
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
        class SaveAnswer
        {


            [JsonProperty("q_id")]
            public string QId { get; set; }

        }
        async void send_nav(string msg)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        App.user_name = "";
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_tok.php";
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<GetTok>>(content);
                        if (tr is null)
                        {
                            return;
                        }
                        ObservableCollection<GetTok> trends = new ObservableCollection<GetTok>(tr);
                        IList<string> List_tok = new List<string>();
                        for (int i = 0; i < trends.Count; i++)
                        {
                            if (trends[i].Tok != null)
                            {
                                if (trends[i].Tok != "")
                                {
                                    List_tok.Add(trends[i].Tok);
                                }
                            }
                        }



                       


                        if (List_tok.Count > 0) { App.firebase_send_noti(List_tok, msg); }
                    }
                    catch (Exception ee)
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
        class GetTok
        {
            [JsonProperty("tok")]
            public string Tok { get; set; }
        }




       async void DeleteBut_Clicked(System.Object sender, System.EventArgs e)
        {

            var action = await DisplayAlert("Online Examination", "Delete Exam ? ", "Yes", "No");
            if (action)
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        try
                        {

                            string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/Delete_Exam.php?EId=" + ExamView.Eid;
                            string json = App.con.GetStringAsync(tt).Result;
                            string QuizName = "";
                            if (ExamType.SelectedIndex >=0 )
                            {
                                switch (ExamType.SelectedIndex)
                                {
                                    case 0:
                                        QuizName = "Test1";
                                        break;
                                    case 1:
                                        QuizName = "Test2";
                                        break;
                                    case 2:
                                        QuizName = "Final";
                                        break;
                                }
                            }




                            string msg = "Delete Exam : " + Course.course_name + " / " + QuizName ;
                            send_nav(msg);
                            await Shell.Current.Navigation.PopAsync();




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
        }
    }
}
