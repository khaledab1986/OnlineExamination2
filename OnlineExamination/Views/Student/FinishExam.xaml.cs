using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;

namespace OnlineExamination.Views.Student
{
    public partial class FinishExam : ContentPage
    {
        public FinishExam()
        {
            InitializeComponent();
            lab1.Text = ExamStart.dt_q_answer.Rows.Count.ToString ();
            // Correct answer
            DataRow[] fr; DataRow[] fr2; DataRow[] fr3 ; 
            int s_ans, r_ans;
            int   s_mrk;
            s_ans = 0; r_ans = 0;
             s_mrk = 0; 
             fr = ExamStart.dt_q_answer.Select();
            for (int i = 0; i< fr.Length; i++)
            {
                fr2 = ExamView_s.Answ.Select("q_id=" + Convert.ToInt32( fr[i]["q_id"].ToString ()) + " and id=" + Convert.ToInt32(fr[i]["id"].ToString()) + " and a_correct =1 ");
                if (fr2.Length > 0)
                {
                    s_ans++;
                    fr3 = ExamView_s.qus.Select("q_id=" + Convert.ToInt32(fr[i]["q_id"].ToString())) ;
                    if (fr3.Length == 1)
                    {
                        s_mrk = s_mrk + Convert.ToInt32(fr3[0]["q_mark"].ToString()); 
                    }
                }
                else
                {
                    r_ans++;
                }
            }
            lab2.Text = s_ans.ToString ();
            lab3.Text = r_ans.ToString();

            string resu = s_mrk + " / " + ExamView_s.mark_of_Exam ;
            lab4.Text = resu; 
            // result Exam /
        }

       async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
