using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class Course : ContentPage
    {
        public ICommand CourseClick { get; private set; }
        public static int course_id = 0;
        public static string  course_name = "";
        public Course()
        {
            InitializeComponent();
            CourseClick = new Command<Parm>((Parm parameter) => OnTapped(parameter));
            stk.Children.Clear();

            string nName = "";
            DataRow[] fr;
            fr = Login.dt_user.Select();
            if (fr.Length > 0)
            {
                nName = fr[0]["User_nickname"].ToString(); 
            }
            fr = Login.dt_course.Select ();
            nickname.Text = nName;
       
            for (int i = 0; i < fr.Length; i++)
            {
               
                Button but = new Button {
                    BackgroundColor = Color.White,
                    CornerRadius = 10,
                    BorderWidth = 1,
                    Margin=10,
                    Opacity=.8,
                    Text = fr[i]["course_name"].ToString(),
                    BorderColor=Color.FromHex("#063AC4"),
                    TextColor = Color.FromHex("#2145A6"),
                    FontFamily = (OnPlatform<string>)Application.Current.Resources["Raleway"],

                    CommandParameter = fr[i]["course_id"].ToString(),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))};
                but.BindingContext = this;
                but.SetBinding(Button.CommandProperty, new Binding("CourseClick", 0));
                var c1 = new Parm { Course_id = Convert.ToInt32(fr[i]["course_id"].ToString()), CourseName=fr[i]["course_name"].ToString() };
                but.CommandParameter = c1;// yr.ToString() + "-" + mnt + "-" + ddy;

                //but.Clicked +=  (sender, args) =>  DisplayAlert("", tm, "ok");
                //ButtonClickCommand = new Command(ButtonClicked);
                stk.Children.Add(but);
            }
        }
        async void OnTapped(Parm tt2)
        {
            course_name = tt2.CourseName;
            course_id = tt2.Course_id ;
            await  Shell.Current.Navigation.PushAsync(new ExamView());
        }
        class Parm
        {

            int course_id;
            string courseName; 
            public int Course_id { get => course_id; set => course_id = value; }
            public string CourseName { get => courseName; set => courseName = value; }
        }
    }
}
