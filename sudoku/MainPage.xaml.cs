namespace sudoku;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
        InitializeComponent();
    }

    public async void OnStartBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
        await Navigation.PushAsync(new DifficultyPage());
    }

    public async void OnLoginBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
        await Navigation.PushAsync(new LoginPage());
    }

    public async void OnRegisterBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PopAsync();
        await Navigation.PushAsync(new RegisterPage());
    }

}

