using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Xamarin.Forms;

namespace OnlineExamination.Views.Student
{
    public partial class Start_Exam_View : ContentPage
    {
        int test = 0;
        public Start_Exam_View()
        {
            InitializeComponent();

            DataRow[] fr;
            fr = ExamView_s.dt_exam.Select("id=" + ExamView_s.EID);
            if (fr.Length > 0)
            {
                test = 1;
               string  QuizName = ""; 
                switch (fr[0]["q_type"].ToString())
                {
                    case "0":
                        QuizName = "Test1";
                        break;
                    case "1":
                        QuizName= "Test2";
                        break;
                    case "2":
                        QuizName = "Final";
                        break;
                }
            

                Exam_Name.Text = fr[0]["course_name"].ToString() + " / " + QuizName;
                Exam_det.Text = fr[0]["des"].ToString(); 
                DateTime dat2 = DateTime.Parse( fr[0]["dat"].ToString());
 
                DateTime tim = DateTime.Parse(fr[0]["tim"].ToString());
                

               CultureInfo us = new CultureInfo("en-US");
                System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");
                string da = dat2.Date.Day.ToString(new CultureInfo("en-US"));
                string mo = dat2.Date.Month.ToString(new CultureInfo("en-US"));
                string ye = dat2.Date.Year.ToString(new CultureInfo("en-US"));

                DateTime dat = DateTime.Parse(mo + "/" + da + "/" + ye  + " " + tim.TimeOfDay.Hours + ":" + tim.TimeOfDay.Minutes + ":" + tim.TimeOfDay.Seconds, cultureinfo) ;

                //int soc = (dat - DateTime.Now).Days;
                TimeSpan value = dat.Subtract(DateTime.Now);
                int Sec1, Min1, Hour1, Day1;
                Sec1 = value.Seconds;
                Min1 = value.Minutes;
                Hour1 = value.Hours;
                Day1 = value.Days;
                s1.Text = Sec1.ToString();
                m1.Text = Min1.ToString();
                h1.Text = Hour1.ToString();
                dy1.Text = Day1.ToString ();
                StartButton.IsEnabled = true;
                if (Day1 == 0 & Hour1 == 0)
                {
                    if (Min1 <= 1 && Min1 > 0)
                    {
                        StartButton.IsEnabled = true;
                    }
                    if (Min1 <= 0 && Sec1 <= 0)
                    {
                        DependencyService.Get<IMessage>().ShortAlert("Finish Time");

                        Shell.Current.Navigation.PopAsync();
                        return  ;
                    }
                }

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    try
                    {

                        TimeSpan value2 = dat.Subtract(DateTime.Now);
                        int Sec2, Min2, Hour2, Day2;
                        Sec2 = value2.Seconds;
                        Min2 = value2.Minutes;
                        Hour2 = value2.Hours;
                        Day2 = value2.Days;

                        s1.Text = Sec2.ToString();
                        m1.Text = Min2.ToString();
                        h1.Text = Hour2.ToString();
                        dy1.Text = Day2.ToString();

                        if (Day2 == 0 & Hour2 == 0)
                        {
                            if (Min2 <= 1 && Min2 > 0)
                            {
                                StartButton.IsEnabled = true;
                            }
                            if (Min2 <=0 && Sec2 <= 0)
                            {
                                DependencyService.Get<IMessage>().ShortAlert("Finish Time");
                              
                                Shell.Current.Navigation.PopAsync();
                               return false;
                            }
                   
                        }


                    }
                    catch
                    {

                    }

                    return true;  
                });


 

            }

        }
         async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
           await Shell.Current.Navigation.PopAsync();
        }

      async  void StartButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ExamStart());
        }
    }
}
