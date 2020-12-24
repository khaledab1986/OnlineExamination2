using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class AddAnswer : PopupPage
    {
        public AddAnswer()
        {
            InitializeComponent();
        }
        private async void CloseAllPopup()
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any())
            {
                await PopupNavigation.Instance.PopAsync();
            }
            //  await PopupNavigation.Instance.PopAsync();
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            CloseAllPopup();
        }

        void Addanswers_Clicked(System.Object sender, System.EventArgs e)
        {
           
            int crr = 0;
            if (crt.IsChecked)
            {
                crr = 1;
                DataRow[] fr = AddQuestions.dt_answer.Select ();
                for (int i = 0; i < fr.Length; i++)
                {
                    if (Convert.ToInt32(fr[i]["correct"].ToString()) ==1)
                    {
                        AddQuestions.dt_answer.Rows[i]["correct"] = 0; 
                    }
                }

            } else crr = 0;
            int c = AddQuestions.dt_answer.Rows.Count  + 1;
            if (App.test_add_update == "UpdateQ")
            {
                UpdateQuestion.dt_answer.Rows.Add(c, AnswerText.Text, crr);
            }
            if (App.test_add_update == "AddNew")
            {
                AddQuestions.dt_answer.Rows.Add(c, AnswerText.Text, crr);
            }
       
           
            MessagingCenter.Send<AddAnswer>(this, "ref");
            CloseAllPopup();
        }
    }
}
