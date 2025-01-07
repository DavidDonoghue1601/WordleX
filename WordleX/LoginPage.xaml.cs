namespace WordleX
{
    public partial class LoginPage : ContentPage
    {


        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            await NavigateToMainPage();
        }

        private async Task NavigateToMainPage()
        {
            await Navigation.PushAsync(new MainPage());
        }


    }
}
