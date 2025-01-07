namespace WordleX
{
    public partial class LoginPage : ContentPage
    {


        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnStartGameClicked(object sender, EventArgs e) //Button for the Login page
        {
            await NavigateToMainPage(); 
        }

        private async Task NavigateToMainPage() //Goes to the MainPage
        {
            await Navigation.PushAsync(new MainPage());
        }


    }
}
