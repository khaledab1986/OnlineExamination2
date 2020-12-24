using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class ExamResults : PopupPage
    {
        public static DataTable dt_r = new DataTable();
        public ExamResults()
        {
            InitializeComponent();

            if (dt_r.Columns.Count == 0)
            {
                dt_r.Columns.Add("Exam_id", typeof(int));
                dt_r.Columns.Add("student_name", typeof(string));
                dt_r.Columns.Add("student_result", typeof(int));
                dt_r.Columns.Add("student_successful", typeof(int));
                dt_r.Columns.Add("id", typeof(int));
            }
        }

        protected override async void OnAppearing()
        {
            dt_r.Rows.Clear(); 
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/Get_Exam_Results.php?Exam_id=" + ExamView.Eid;
                        var content = await App.con.GetStringAsync(tt);
                        var tr = JsonConvert.DeserializeObject<IList<ExamRes>>(content);
                        if (tr is null)
                        {
                            return;
                        }
                        ObservableCollection<ExamRes> trends = new ObservableCollection<ExamRes>(tr);
                        int co1, co2, co3;
                        co1 = 0; co2 = 0; co3 = 0;  
                        for (int i = 0; i < trends.Count; i++)
                        {
                            dt_r.Rows.Add(trends[i].ExamId,trends[i].StudentName,trends[i].StudentResult,trends[i].StudentSuccessful,trends[i].Id);
                            if (trends[i].StudentSuccessful == 1)
                            {
                                co2++; 
                            }
                            else
                            {
                                co3++;
                            }
                        }
                        co1 = trends.Count;
                        lab1.Text = co1.ToString();
                        lab2.Text = co2.ToString();
                        lab3.Text = co3.ToString();

                    }
                    catch (Exception eb)
                    {
                        await DisplayAlert("", eb.Message, "ok");
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

       async void View_details_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any())
            {
                await PopupNavigation.Instance.PopAsync();
                await Shell.Current.Navigation.PushAsync(new ExamResultsDetails());
            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            CloseAllPopup(); 
        }

        private async void CloseAllPopup()
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any())
            {
                await PopupNavigation.Instance.PopAsync();
            }
            //  await PopupNavigation.Instance.PopAsync();
        }

        class ExamRes
        {
            [JsonProperty("Exam_id")]
            public long ExamId { get; set; }

            [JsonProperty("student_name")]
            public string StudentName { get; set; }

            [JsonProperty("student_result")]
            public long StudentResult { get; set; }

            [JsonProperty("student_successful")]
            public long StudentSuccessful { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }
        }
    }
}
