using Android.Window;

namespace plante2._3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            bool isDarkMode = Preferences.Get("DarkMode", false);
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

            Application.Current.Resources["LabelTextColor"] = isDarkMode
                ? Application.Current.Resources["DarkMode"]
                : Application.Current.Resources["LightMode"];

        }

         protected override Window CreateWindow(IActivationState? activationState)
         {

            Window s = new Window(new Skia());

            string loginStatus = Preferences.Get("UserLoginStatus", "None");

             ContentPage startPage = loginStatus == "None" ? new Login() : new MainPage();

             Window w= new Window(new AppShell());


             Task.Run(async () =>
             {
                 await Task.Delay(2800);

                 if (loginStatus == "None")
                 {
                     await Shell.Current.Navigation.PushModalAsync(new Login()); 
                 }
                 else
                 {
                     await Shell.Current.Navigation.PushModalAsync(new MainPage()); 
                 }
             });

             return w;
         }


    }
}