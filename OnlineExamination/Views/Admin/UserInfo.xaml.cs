using System;
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
    public partial class UserInfo : PopupPage
    {
        public UserInfo()
        {
            InitializeComponent();
            user_name.Text = "";
            Pass_Word.Text = "";
            NickName.Text = "";
            KindUser.SelectedIndex = -1 ;

            if (AdminPage.idd > 0)
            {
                DataRow[] fr = AdminPage.dt_AllUser.Select("id=" + AdminPage.idd);
                if (fr.Length > 0)
                {
                    user_name.Text = fr[0]["User_name"].ToString ();
                    Pass_Word.Text = fr[0]["User_pass"].ToString();
                    NickName.Text = fr[0]["User_nickname"].ToString();
                    int knd = Convert.ToInt32( fr[0]["User_kind"].ToString());
                    if (knd == 0) { KindUser.SelectedIndex = 0; }
                    if (knd == 1) { KindUser.SelectedIndex = 1; }
                    if (knd == 2) { KindUser.SelectedIndex = 2; }
                    del.IsVisible = true;
                    if (AdminPage.tech_ == 1)
                    {
                        courses.IsVisible = true; 
                    }
                    else
                    {
                        courses.IsVisible = false ;
                    }
                }
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

      async   void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (KindUser.SelectedIndex == -1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Select a Kind of User !");
                return;
            }
            if (user_name.Text == "" || user_name.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter User Name !");
                user_name.Focus();
                return;
            }
            if (Pass_Word.Text == "" || Pass_Word.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter Password !");
                Pass_Word.Focus();
                return;
            }
            if (NickName.Text == "" || NickName.Text == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Enter NickName !");
                NickName.Focus();
                return;
            }
            string cc = "";
            if (AdminPage.idd == 0)
            {
                cc = "user_name=" + user_name.Text + "&pass=" + Pass_Word.Text + "&nick=" + NickName.Text + "&knd=" + KindUser.SelectedIndex + "&id=" + 0;

            }
            else
            {
                cc = "user_name=" + user_name.Text + "&pass=" + Pass_Word.Text + "&nick=" + NickName.Text + "&knd=" + KindUser.SelectedIndex + "&id=" + AdminPage.idd;

            }
            var action = await DisplayAlert("Online Examination", "Add " + " New User " + " ? ", "Yes", "No");
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
                            string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/insert_user.php?" + cc;
                            var response2 = await App.con.PostAsync(tt, formContent);
                            response2.Dispose();
                            CloseAllPopup();
                        MessagingCenter.Send<UserInfo>(this, "ref");
                    }
                      
                    }
                    catch
                    {

                    }
                    sav.IsEnabled = true;
                }
           
   
        }

       async void del_Clicked(System.Object sender, System.EventArgs e)
        {


          
            if (AdminPage.idd >  0)
            {
                var action = await DisplayAlert("Online Examination", "Delete User ? "  , "Yes", "No");
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
                            string tt = "https://onlineexamination.a2hosted.com/OnlineExamination/delete_user.php?id=" + AdminPage.idd;
                            var response2 = await App.con.PostAsync(tt, formContent);
                            response2.Dispose();
                            CloseAllPopup();
                            MessagingCenter.Send<UserInfo>(this, "ref");
                        }

                    }
                    catch
                    {

                    }
                    del.IsEnabled = true;
                }
            }
   
  
        }

       async void courses_Clicked(System.Object sender, System.EventArgs e)
        {
            CloseAllPopup();
            await Shell.Current.Navigation.PushAsync(new courses_Admin());
        }
    }
}
