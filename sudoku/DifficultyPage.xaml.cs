namespace sudoku;
  
public partial class DifficultyPage : ContentPage
{
	public DifficultyPage()
	{
		InitializeComponent();
	}

    public async void EasyBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new PlayPage(1));
    }

    public async void MedBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new PlayPage(2));
    }

    public async void HardBtnClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new PlayPage(3));
    }
}