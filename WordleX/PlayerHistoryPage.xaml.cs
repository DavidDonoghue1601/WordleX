using Microsoft.Maui.Controls;
using System;
using System.IO;
using Microsoft.Maui.Storage;
using System.Collections.Generic;

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
                            EmojiGrid = parts[4] // This will hold the emoji grid
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
            var historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "player_history.txt");

            if (File.Exists(historyFilePath))
            {
                File.Delete(historyFilePath); // Delete the history file
                playerHistoryListView.ItemsSource = new List<PlayerHistoryItem>(); // Clear the ListView
            }
        }
    }

    public class PlayerHistoryItem
    {
        public required string PlayerName { get; set; } //I added the "required" part because MAUI told me to, and removes warnings
        public required string Timestamp { get; set; }
        public required string CorrectWord { get; set; }
        public required string Attempts { get; set; }

        public required string EmojiGrid { get; set; }  // This stores the emoji grid

        
        public string FormattedEmojiGrid => FormatEmojiGrid();

        private string FormatEmojiGrid()
        {
             
            var gridRows = EmojiGrid.Split(" "); // Split the different emojis, ensures better UI
            string formattedGrid = string.Empty;

            // Add row for each guess
            for (int i = 0; i < 6; i++)
            {
                if (i < gridRows.Length)
                {
                    formattedGrid += string.Join(" ", gridRows.Skip(i * 5).Take(5)) + Environment.NewLine;
                }
            }

            return formattedGrid.Trim(); // Return the formatted emoji grid
        }
    }
        
 
}
