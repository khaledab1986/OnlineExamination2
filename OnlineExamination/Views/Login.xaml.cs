using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using OnlineExamination.Models;
using OnlineExamination.Views.Student;
using OnlineExamination.Views.techer;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views
{
    public partial class Login : ContentPage
    {
        public static DataTable dt_user = new DataTable ();
        public static DataTable dt_course = new DataTable();
        public static string  username  = "";

        public Login()
        {
            InitializeComponent();

            if (dt_user.Columns.Count == 0)
            {
                dt_user.Columns.Add("id", typeof(int));
                dt_user.Columns.Add("User_name", typeof(string));
                dt_user.Columns.Add("User_kind", typeof(int));
                dt_user.Columns.Add("User_nickname", typeof(string));
            }
     
            if (dt_course .Columns .Count == 0)
            {
                dt_course.Columns.Add("course_id", typeof(int));
                dt_course.Columns.Add("User_id", typeof(int));
                dt_course.Columns.Add("course_name", typeof(string));
                dt_course.Columns.Add("fin", typeof(int));
            }

 

        }

        void login_Clicked(System.Object sender, System.EventArgs e)
        {
            //  DisplayAlert("", "Hi", "Ok");

            login_(); 


        }

      async  void login_()
        {
            login.IsEnabled = false;
            if (User_name.Text == "" || User_name.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter User Name !!");
                User_name.Focus();
                return;
            }
            if (pass_word.Text == "" || pass_word.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter PassWord !!");
                pass_word.Focus();
                return;
            }
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        App.user_name = "";
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/get_user2.php?User_name=" + User_name.Text + " &User_pass=" + pass_word.Text + "&tok=" + App.tok;
                        var content = await App.con.GetStringAsync(tt);
                        // Temperatures5 obj = JsonConvert.DeserializeObject<Temperatures5>(content);
                        var tr = JsonConvert.DeserializeObject<IList<UserClass>>(content);


                     //   string json = App.con.GetStringAsync(tt).Result;
                       // var tr = JsonConvert.DeserializeObject<IList<UserClass>>(json);
                        if (tr is null)
                        {
                            DependencyService.Get<IMessage>().ShortAlert("User Name or Password Is Incorrect");
                            login.IsEnabled = true;
                            return;
                        }
                        if (tr.Count == 0 )
                        {
                            DependencyService.Get<IMessage>().ShortAlert("User Name or Password Is Incorrect");
                            login.IsEnabled = true ;
                            return;
                        }
                        App.user_name = User_name.Text;
                        ObservableCollection<UserClass> trends = new ObservableCollection<UserClass>(tr);
                        int fin = 0;
                        dt_course.Rows.Clear();
                        dt_user.Rows.Clear();

                        for (int i = 0; i < trends.Count; i++)
                        {
                            if (trends[i].Courses == null) { fin = 0;  } else { fin = 1; }
                            dt_user.Rows.Add(trends[i].Id, trends[i].UserName, trends[i].UserKind, trends[i].UserNickname);

                            if (trends[i].Courses != null)
                            {
                                foreach (var a in trends[i].Courses)
                                {
                                    dt_course.Rows.Add(a.CourseId, a.UserId, a.CourseName, a.Fin);
                                    //if (a.Exams != null)
                                    //{
                                    //    foreach (var b in a.Exams)
                                    //    {
                                    //        dt_exam.Rows.Add(b.Id, b.Dat, b.Tim, b.Des, b.Hor, b.Mark, b.Stat, b.CoursesId);
                                    //    }
                                    //}

                                }
                            }
                        }


                        //IList<string> List_tok = new List<string>();
                        //if (App.tok  != "") { List_tok.Add(App.tok); }
                        //if (List_tok.Count > 0) { App.firebase_send_noti(List_tok, "Welcome"); }


                        if (fin != 0) {await Shell.Current.Navigation.PushAsync(new Course()); }
                        if (fin == 0) { await Shell.Current.Navigation.PushAsync(new ExamView_s()); }
                    }
                    catch (Exception ee)
                    {
                        string tt = ee.Message;
                        DisplayAlert("", tt, "ok");
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
            login.IsEnabled = true; 
        }

        class UserClass
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("User_name")]
            public string UserName { get; set; }

            [JsonProperty("User_pass")]
            public string UserPass { get; set; }

            [JsonProperty("User_kind")]
            public int UserKind { get; set; }

            [JsonProperty("fin")]
            public int Fin { get; set; }

            [JsonProperty("User_nickname")]
            public string UserNickname { get; set; }

            [JsonProperty("courses")]
            public CourseClass[] Courses { get; set; }
        }

        class CourseClass
        {
            [JsonProperty("course_id")]
            public int CourseId { get; set; }

            [JsonProperty("User_id")]
            public int UserId { get; set; }

            [JsonProperty("course_name")]
            public string CourseName { get; set; }

            [JsonProperty("fin")]
            public int Fin { get; set; }

      
        }

 
        void pass_word_Completed(System.Object sender, System.EventArgs e)
        {
            login_();
        }
    }
}
