using Android.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plante2._3
{
    public partial class Settings
    {
        private MainPage _mainpage;

        public Settings(MainPage camerafunction) 
        {
            InitializeComponent();
            _mainpage = camerafunction;
        }

        private int count = 0;
        private void DarkMode(object sender, ToggledEventArgs e)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            Preferences.Set("DarkMode", e.Value);
        }

        private async void StatusChange(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert("Logout",
                "Do you want to log out?", "Yes", "Cancel");

            if (confirmLogout)
            {
                Preferences.Clear();
                await Navigation.PushModalAsync(new Login());
            }
            else
            {
                await DisplayAlert("Status", "You are still logged in.", "OK");
            }
        }


        private async void OnDeleteAccount(object sender, EventArgs e)
        {
            string? username = Preferences.Get("Username", null);

            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "Cannot delete 'Guest' .", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Confirmation", "Your account shall be deleted and cannot be restored ", "Yes", "No");

            if (confirm)
            {
                SecureStorage.Remove($"user_{username}");
                Preferences.Clear();

                await DisplayAlert("", "Account deleted successfuly", "OK");
                await Navigation.PushModalAsync(new Login());
            }
        }

        private void SettingsCaptureClicked(object sender,EventArgs E)
        {

            _mainpage?.CaptureClicked(null, EventArgs.Empty);
        }
        private async void SettingsToMainPage(object sender,EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage());
        }

        private async void SettingsRedirect(object sender,EventArgs e)
        {
            count++;
            if (count == 3)
            {
                count = 0;
                await DisplayAlert("Hello!", "You are currently in Settings!", "OK");
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            string loginStatus = Preferences.Get("UserLoginStatus", "Guest");
            string username = Preferences.Get("Username", "Guest");

            if (loginStatus == "Account")
            {
                Status.Text = username;
            }else
            {
                Status.Text = "Guest";
            }
        }

    }
}
