

namespace WordleX
{
    public partial class PlayerHistoryPage : ContentPage
    {
      
        public PlayerHistoryPage()
        {
            InitializeComponent();
            LoadPlayerHistory(); // Load the player's history
        }

        private void LoadPlayerHistory()
        {
            var historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "player_history.txt");

            if (File.Exists(historyFilePath))
            {
                // reads in file
                var historyLines = File.ReadAllLines(historyFilePath);
                var historyItems = new List<PlayerHistoryItem>();

                foreach (var line in historyLines)
                {
                    var parts = line.Split('|');

                    if (parts.Length == 5)
                    {
                        // creates an item for each line
                        var historyItem = new PlayerHistoryItem
                        {
                            PlayerName = parts[0],
                            Timestamp = parts[1],
                            CorrectWord = parts[2],
                            Attempts = parts[3],
                            
                        };
                        historyItems.Add(historyItem);
                    }
                }

                // binding history items to list view
                playerHistoryListView.ItemsSource = historyItems;
            }
            else
            {
               
                playerHistoryListView.ItemsSource = new List<PlayerHistoryItem>();
            }
        }

        private void OnClearHistoryClicked(object sender, EventArgs e)
        {
            var historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "player_info.txt");

            if (File.Exists(historyFilePath))
            {
                File.Delete(historyFilePath); // Delete the history file
                playerHistoryListView.ItemsSource = new List<PlayerHistoryItem>(); // Clear the ListView
            }
        }
    }

    public class PlayerHistoryItem
    {
        public required string PlayerName { get; set; } //I added the "required" part because it told me to, and removes warnings
        public required string Timestamp { get; set; }
        public required string CorrectWord { get; set; }
        public required string Attempts { get; set; }
    }
        
 
}
