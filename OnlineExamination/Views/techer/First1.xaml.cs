using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OnlineExamination.Views.techer
{
    public partial class First1 : ContentPage
    {
        public First1()
        {
            InitializeComponent();
            user_name.Text = Login.username; 
        }
    }
}
