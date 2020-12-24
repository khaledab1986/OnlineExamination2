using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class AddQuestions : ContentPage
    {
       public static DataTable dt_answer = new DataTable ();
        public ICommand DellAnswer { get; private set; }
        public ICommand CorrectAnswer { get; private set; }
        public AddQuestions()
        {
            InitializeComponent();
            App.test_add_update = "AddNew";
            DellAnswer = new Command<Parm1>((Parm1 parameter) => OnTapped(parameter));
            CorrectAnswer = new Command<Parm1>((Parm1 parameter) => OnTapped2(parameter));
            if (dt_answer.Columns.Count == 0)
            {
                dt_answer.Columns.Add("id", typeof(int));
                dt_answer.Columns.Add("answer_text", typeof(string));
                dt_answer.Columns.Add("correct", typeof(int));
            }
            MessagingCenter.Subscribe<AddAnswer>(this, "ref", (sender) => {

                IsBusy = true;

                Refsh();
            });
            int ct = QuestionsBank1.QCount + 1;
            QuestionNo.Text = "Question -" + ct.ToString();

            if (QuestionsBank1.pic_index >= 0 )
            {
                ExamType.SelectedIndex = QuestionsBank1.pic_index;
                DataRow[] fr;
                fr = QuestionsBank1.dt_Question.Select("q_type=" + QuestionsBank1.pic_index);
                  ct = fr.Length + 1;

                QuestionNo.Text = "Question -" + ct.ToString();

            }

       
        }


        class Parm1
        {
            int answer_id;
            public int Answer_id { get => answer_id; set => answer_id = value; }
        }
        protected override async void OnAppearing()
        {
            dt_answer.Rows.Clear(); 
        }
        async void OnTapped(Parm1 tt2)
        {
            DataRow[] fr = dt_answer.Select();
            for (int i = 0; i < fr.Length; i++)
            {
                if (Convert.ToInt32(fr[i]["id"].ToString ()) == tt2.Answer_id)
                {
                    dt_answer.Rows.RemoveAt(i);
                    break; 
                }
            }
            Refsh();
        }

        void OnTapped2(Parm1 tt2)
        {
            DataRow[] fr = dt_answer.Select();
            for (int i = 0; i < fr.Length; i++)
            {
               dt_answer.Rows[i]["correct"] = 0;

            }
            for (int i = 0; i < fr.Length; i++)
            {
                if (Convert.ToInt32(fr[i]["id"].ToString()) == tt2.Answer_id)
                {
                     dt_answer.Rows[i]["correct"] = 1;
                    break;
                }
            }
            Refsh();
        }

        void Refsh()
        {
            stk_answer.Children.Clear();
    

            DataRow[] fr = dt_answer .Select ("","id"); 
            if ( fr.Length >0)
            {
                Grid grd1 = new Grid { ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA") };
                grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
      

                for (int i = 0; i < fr.Length; i++)
                {
                    grd1.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    StackLayout stk_c = new StackLayout {Orientation=StackOrientation.Horizontal,Margin=5};
                    ImageButton img1 = new ImageButton
                    {
                        BackgroundColor = Color.Transparent,
                        Source = "CloseIcon.png",
                        HeightRequest=30,
                        WidthRequest=30,
                    };
                    img1.BindingContext = this;
                    img1.SetBinding(ImageButton.CommandProperty, new Binding("DellAnswer", 0));
                    var c1 = new Parm1 { Answer_id = Convert.ToInt32(fr[i]["id"].ToString()) };
                    img1.CommandParameter = c1;

                    ImageButton img2 = new ImageButton
                    {
                        Source = "choise1.png",
                        HeightRequest = 30,
                        WidthRequest = 30,
                        BackgroundColor=Color.Transparent,
                    };
                    img2.BindingContext = this;
                    img2.SetBinding(ImageButton.CommandProperty, new Binding("CorrectAnswer", 0));
                    img2.CommandParameter = c1;

                    if (Convert.ToInt32( fr[i]["correct"].ToString()) == 1)
                    {
                        img2.Source = "choise2.png"; 
                    }
                    Label lb1 = new Label {
                        Text = fr[i]["answer_text"].ToString(),
                        TextColor = Color.FromHex("#2E2E2E"),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    };

                    stk_c.Children.Add(img1);
                    stk_c.Children.Add(lb1);
                    stk_c.Children.Add(img2);

                    grd1.Children.Add(stk_c, 0, i);
                }
                stk_answer.Children.Add(grd1);
            }
           
           
        }
        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync(); 
        }

        async  void Addanswers_Clicked(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new AddAnswer());
        }



        async void saveBut_Clicked(System.Object sender, System.EventArgs e)
        {
            if (ExamType.SelectedIndex == -1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Select a Type of Exam !");
                return; 
            }
            if (Convert.ToDouble(mark.Text) <= 0  )
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Mark Of Exam");
                mark.Focus();
                return; 
            }

            if (QuestionText.Text  == "" || QuestionText.Text ==null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Question !");
                QuestionText.Focus();
                return;
            }

            if (dt_answer.Rows.Count == 0 )
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Answers !");
                return;
            }

            DataRow[] fr = dt_answer.Select();
            int tst = 0; 
            for (int i = 0; i < fr.Length; i++)
            {
                if (Convert.ToInt32( fr[i]["correct"].ToString ()) == 1)
                {
                    tst = 1; 
                }
            }
            if (tst == 0)
            {
                DependencyService.Get<IMessage>().ShortAlert("Choose The Correct Answer !");
                return;
            }

            var action = await DisplayAlert("Online Examination", "Add " + QuestionNo.Text + " ? ", "Yes", "No");
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


                            string cont = "?q_name=" + QuestionNo.Text + "&q_type=" + ExamType.SelectedIndex.ToString() + "&q_mark=" + mark.Text + "&q_text=" + QuestionText.Text + "&course_id=" + Course.course_id;
                            string Url = "https://onlineexamination.a2hosted.com/OnlineExamination/insert_Question.php";// + cont;

                            DataRow[] fr2 = dt_answer.Select();
                            List<SaveAnswer> list = new List<SaveAnswer> { };
                            for (int i = 0; i < fr.Length; i++)
                            {
                                list.Add(new SaveAnswer {
                                    AnswerText = fr[i]["answer_text"].ToString(),
                                    Correct = Convert.ToInt32(fr[i]["correct"].ToString()),
                                    QName= QuestionNo.Text,
                                    QType = ExamType.SelectedIndex,
                                    QMark=Convert.ToInt32(mark.Text),
                                    QText = QuestionText.Text,
                                    CourseId= Course.course_id,
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

                         
                            var resp  = await _client.SendAsync(request);
                            if ( resp.IsSuccessStatusCode)
                            {
                                 await Shell.Current.Navigation.PopAsync();
                            }
                            else
                            {
                                DependencyService.Get<IMessage>().ShortAlert("تأكد من الاتصال بالانترنت");

                            }

                        }
                        catch (Exception er)
                        {
                            string tt = er.Message ; 
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



                saveBut.IsEnabled = true ;



            }
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

        class SaveAnswer
        {
 

            [JsonProperty("answer_text")]
            public string  AnswerText { get; set; }

            [JsonProperty("correct")]
            public int Correct { get; set; }


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


   



        }

        void ExamType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {

            Picker pic = sender as Picker;
             DataRow[] fr;
            fr = QuestionsBank1. dt_Question.Select("q_type=" + pic.SelectedIndex);
            int ct = fr.Length  + 1;

            QuestionNo.Text = "Question -" + ct.ToString();
        }
    }
}
