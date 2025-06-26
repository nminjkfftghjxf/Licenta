using Android.App;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace plante2._3
{
    
    public partial class Login: ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void AccountClicked(object sender, EventArgs e)
        {
            string? username = UserName.Text?.Trim();  
            string? password = Password.Text?.Trim();


            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Could not login in", "Type in username and password", "OK");
                return;
            }

            var storedPassword = await SecureStorage.GetAsync($"user_{username}");

            if (storedPassword == null)
            {
                await DisplayAlert("Login Failed", "Password not found!", "OK");
                return;
            }

            if (storedPassword != password)
            {
                await DisplayAlert("Login Failed", "Incorrect password.", "OK");
                return;
            }

            if (storedPassword != null && storedPassword== storedPassword)
            {
                await SecureStorage.SetAsync("current_user", username);
            }


            Preferences.Set("UserLoginStatus", "Account");
            Preferences.Set("Username", username);

            await Navigation.PushModalAsync(new MainPage());
        }

        private async void GuestClicked(object sender, EventArgs e)
        {

            await SecureStorage.SetAsync("current_user", "Guest");

            Preferences.Set("UserLoginStatus", "Guest");
            await Navigation.PushModalAsync(new MainPage());

        }

        private async void OnCreateAccount(object sebder,EventArgs e)
        {
            await Navigation.PushModalAsync(new Account());
        }
        
        private DateTime lasttap;

        protected override bool OnBackButtonPressed()
        {
            if ((DateTime.Now - lasttap).TotalMilliseconds < 500)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                return true;
            }
            lasttap = DateTime.Now;
            Android.Widget.Toast.MakeText(Android.App.Application.Context, "Double tap to exit", Android.Widget.ToastLength.Short).Show();
            return true;
        }

    }
}
