using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OnlineExamination.Views.Admin
{
    public partial class courses_info : PopupPage
    {
      
        public courses_info()
        {
            InitializeComponent();
            course_name.Text = "";
            if (AdminPage.idd > 0)
            {
                DataRow[] fr = courses_Admin.dt_courses .Select("User_id=" + AdminPage.idd);
                if (fr.Length > 0)
                {
                    course_name.Text = fr[0]["course_name"].ToString();
                    del.IsVisible = true;
                }
            }
        }

       async void sav_Clicked(System.Object sender, System.EventArgs e)
        {
      
            if (course_name.Text == "" || course_name.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter course Name !");
                course_name.Focus();
                return;
            }
            string cc = "";
            if (AdminPage.idd == 0)
            { 
                cc = "course_name=" + course_name.Text + "&id=" + 0 + "&User_id=" + AdminPage.idd;

            }
            else
            {
                cc = "course_name=" + course_name.Text + "&id=" + courses_Admin.idd + "&User_id=" + AdminPage.idd;
            }
            var action = await DisplayAlert("Online Examination", "Add " + " New course " + " ? ", "Yes", "No");
            if (action)
            {
                sav.IsEnabled = false;

                try
                {
                    var current = Connectivity.NetworkAccess;
                    if (current == NetworkAccess.Internet)
                    {
                        var formContent = new FormUrlEncodedContent(new[]
    {
                                        new KeyValuePair<string, string>("id", "1"),

                                    });
                        string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/insert_courses.php?" + cc;
                        var response2 = await App.con.PostAsync(tt, formContent);
                        response2.Dispose();
                        CloseAllPopup();
                        MessagingCenter.Send<courses_info>(this, "ref");
                    }

                }
                catch
                {

                }
                sav.IsEnabled = true;
            }
        }
        private async void CloseAllPopup()
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any())
            {
                await PopupNavigation.Instance.PopAsync();
            }
            //  await PopupNavigation.Instance.PopAsync();
        }

        async void del_Clicked(System.Object sender, System.EventArgs e)
        {
            if (courses_Admin.idd > 0)
            {
                var action = await DisplayAlert("Online Examination", "Delete courses ? ", "Yes", "No");
                if (action)
                {
                    del.IsEnabled = false;

                    try
                    {
                        var current = Connectivity.NetworkAccess;
                        if (current == NetworkAccess.Internet)
                        {
                            var formContent = new FormUrlEncodedContent(new[]
        {
                                        new KeyValuePair<string, string>("id", "1"),

                                    });
                            string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/delete_courses.php?id=" + courses_Admin.idd;
                            var response2 = await App.con.PostAsync(tt, formContent);
                            response2.Dispose();
                            CloseAllPopup();
                            MessagingCenter.Send<courses_info>(this, "ref");
                        }

                    }
                    catch
                    {

                    }
                    del.IsEnabled = true;
                }
            }

        }
    }
}
