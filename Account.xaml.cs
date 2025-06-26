using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plante2._3
{
    public partial class Account:ContentPage
    {
        public Account() 
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string? username = UsernameEntry.Text?.Trim();
            string? password = PasswordEntry.Text?.Trim();


            if((string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password)))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            if (username.Length > 35)
            {
                await DisplayAlert("Eroare", "Username cannot have more than 35 characters.", "OK");
                return;
            }

            if (password.Length > 8 || !ContainsSpecialChar(password))
            {
                await DisplayAlert("Eroare", "Password has a limit of 8 characters and requires 1 special caracter(+,-,!,#,@,* ect).", "OK");
                return;
            }

            if (await SecureStorage.GetAsync($"user_{username}") != null)
            {
                await DisplayAlert("Error", "User already exists!", "OK");
                return;
            }

            await SecureStorage.SetAsync($"user_{username}", password);

            await DisplayAlert("Success", "Account created!", "OK");


            await Navigation.PushModalAsync(new Login());
        }

        private static bool ContainsSpecialChar(string input)
        {
            return input.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private async void OnCancelClicked(object sender,EventArgs e)
        {
            await Navigation.PushModalAsync(new Login());
        }

    }
}
