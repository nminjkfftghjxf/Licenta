namespace plante2._3;

public partial class Skia : ContentPage
{
	public Skia()
	{
        InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
    }

}