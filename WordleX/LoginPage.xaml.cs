//I used alot of try catches here to try and figure out where an error was popping up
//Not sure if i should leave them in or not (i found where the error was)
//Im sure there is no harm in leaving them in?

namespace WordleX
{
    public partial class LoginPage : ContentPage
    {
        private const string PlayerNameFileName = "player_name.txt";

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            try
            {
                //GET PLAYER NAME
                string playerName = nameEntry.Text?.Trim(); 

                if (string.IsNullOrEmpty(playerName))
                {
                    // If no entry, name is "Guest"
                    playerName = "Guest";
                }

                //SAVING PLAYERS NAME
                SavePlayerName(playerName);

                //go to mainpage
                await NavigateToMainPage();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error in OnStartGameClicked: {ex.Message}");
            }
        }

        private bool SavePlayerName(string playerName)
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, PlayerNameFileName);
                
                Directory.CreateDirectory(FileSystem.AppDataDirectory);

                // put player name into file
                File.WriteAllText(filePath, playerName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving player name: {ex.Message}");
                return false;
            }
        }

        public static string LoadPlayerName()
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, PlayerNameFileName);
                if (File.Exists(filePath))
                {
                    // read player name from file
                    string playerName = File.ReadAllText(filePath).Trim();
                    return playerName; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player name: {ex.Message}");
            }
            return string.Empty; 
        }

        private async Task NavigateToMainPage()
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}
