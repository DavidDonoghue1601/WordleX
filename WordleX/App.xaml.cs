namespace WordleX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage()); //This makes sure that LoginPage opens first
        }
    }
}
