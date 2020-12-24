using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class ExamResultsDetails : ContentPage
    {
        public ExamResultsDetails()
        {
            InitializeComponent();

        }
        protected override  void OnAppearing()
        {
            DataRow[] fr = ExamResults.dt_r.Select();
            stk.Children.Clear();
            Grid grd1 = new Grid { ColumnSpacing = 5, BackgroundColor = Color.FromHex("#FAFAFA"),RowSpacing=15 };
            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) });
            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) });
            grd1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) });

            grd1.RowDefinitions.Add(new RowDefinition { Height =   GridLength.Star });
            string stat = "";
            for (int i =0; i < fr.Length; i++)
            {
                Label lb1 = new Label
                {
                    Text = fr[i]["student_name"].ToString(),
                    TextColor = Color.FromHex("#3C3C3C"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                    
                };
                Label lb2 = new Label
                {
                    Text = fr[i]["student_result"].ToString(),
                    TextColor = Color.FromHex("#3C3C3C"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center

                };
                if (Convert.ToInt32(fr[i]["student_successful"].ToString()) == 1)
                {
                    stat = "Successful"; 
                }
                else
                {
                    stat = "Failing";
                }
                Label lb3 = new Label
                {
                    Text = stat,
                    TextColor = Color.FromHex("#DD3333"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center

                };
                if (stat == "Successful") { lb3.TextColor = Color.FromHex("#1EA229"); }

                grd1.Children.Add(lb1, 0, i);
                grd1.Children.Add(lb2, 1, i);
                grd1.Children.Add(lb3, 2, i);
            }
            stk.Children.Add(grd1); 
        }
       async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
