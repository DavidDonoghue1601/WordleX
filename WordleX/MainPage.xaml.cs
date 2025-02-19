﻿//removed collections, appeared to make no difference

namespace WordleX
{
    public partial class MainPage : ContentPage
    {
        private string selectedWord; //This is the correct word
        private int attemptsLeft; //The ammount of attempts that is left
        private List<string> words; //List of words (got them from vle file)
        private int currentRow;//Current row that the wordle game is on
        private bool gameOver; //Is game over? (true / false)
        private string PlayerName;

        public MainPage()
        {
            InitializeComponent();
            LoadWords();
            PlayerName = LoadPlayerName();

        }
        private async void LoadWords()
        {
            var fileName = Path.Combine(FileSystem.AppDataDirectory, "words.txt"); //saving to a file (so i dont constantly load from link)

            if (!File.Exists(fileName))
            {
                await DownloadAndSaveWordsAsync();  //Waits until all words are downloaded to start the game
            }

            words = File.ReadAllLines(fileName).Where(word => word.Length == 5).ToList(); // Reads all lines of file, gets the words that are exactly 5 letters long
            StartNewGame(); //start game once game is loaded
        }
        private async Task DownloadAndSaveWordsAsync() //downloading and saving the words once the app is loaded once
        {
            var fileUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
            var fileName = Path.Combine(FileSystem.AppDataDirectory, "words.txt");

            HttpClient client = new HttpClient();
            var content = await client.GetStringAsync(fileUrl);
            await File.WriteAllTextAsync(fileName, content);
        }
        private void InitializeWordleGrid()
        {
            wordleGrid.Children.Clear();

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    var entry = new Entry
                    {
                        BackgroundColor = Colors.White,
                        TextColor = Colors.Black,
                        FontSize = 30,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        MaxLength = 1,
                        TextTransform = TextTransform.Uppercase
                    };

                    entry.TextChanged += OnEntryTextChanged;
                    entry.Completed += (sender, e) => MoveToNextEntry(entry);
                    Grid.SetRow(entry, row);
                    Grid.SetColumn(entry, col);
                    wordleGrid.Children.Add(entry);
                    entry.IsEnabled = (row == 0);
                }
            }
        }
        private void MoveToNextEntry(Entry currentEntry) //Moving entry to next row once filled
        {
            int row = Grid.GetRow(currentEntry);
            int col = Grid.GetColumn(currentEntry);

            if (col < 4)
            {
                var nextEntry = wordleGrid.Children.OfType<Entry>()
                    .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col + 1);

                nextEntry?.Focus();
            }
        }
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!gameOver && currentRow == Grid.GetRow(entry) && entry.IsEnabled)
            {
                if (!string.IsNullOrEmpty(entry.Text) && entry.Text.Length == 1)
                {
                    entry.Text = entry.Text.ToUpper();
                    MoveToNextEntry(entry); //Moves to next entry once filled
                }
                else if (e.OldTextValue != null && e.NewTextValue == string.Empty)
                {
                    MoveToPreviousEntry(entry);
                }
            }
        }
        private void MoveToPreviousEntry(Entry currentEntry)//Moves to previous entry (Couldnt get this to work 100%, get back to this)
        {
            int row = Grid.GetRow(currentEntry);
            int col = Grid.GetColumn(currentEntry);

            var previousEntry = wordleGrid.Children.OfType<Entry>()
                .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col - 1);

            previousEntry?.Focus();//focuses on the first column
        }
        private void OnSubmitGuess(object sender, EventArgs e)
        {
            if (gameOver) return;

            var guess = new string[5]; //your guess is a 5 letter word
            for (int i = 0; i < 5; i++)
            {
                var entry = GetGridEntry(currentRow, i);
                if (entry != null)
                {
                    guess[i] = entry.Text?.Trim().ToLower(); // if entry is full, move to the next row
                }
            }

            if (guess.Any(g => string.IsNullOrEmpty(g))) //fidning out if the guess is invalid
            {
                DisplayAlert("Invalid Guess", "Please fill all 5 boxes with letters.", "OK"); 
                return;
            }

            string guessString = string.Join("", guess);

            if (!words.Contains(guessString))
            {
                DisplayAlert("Invalid Word", $"{guessString} is not a valid word.", "OK"); //word does not exist (at least in file)
                return;
            }
            attemptsLeft--; // decrement the attempts left
            attemptsLabel.Text = $"Attempts left: {attemptsLeft}";
            var feedback = GetGuessFeedback(guessString, selectedWord);
            UpdateWordleGrid(guessString, feedback, currentRow);

            if (guessString == selectedWord)
            {
                EndGame(true);
                SavePlayerHistory(true);
            }
            else if (attemptsLeft == 0) 
            {
                EndGame(false);
                SavePlayerHistory(false);
            }
            else
            {
                LockRow(currentRow);  // Locking the current row AFTER submission
                currentRow++;
                EnableRow(currentRow);  // UNLOCK next row for submission
            }
        }
        private void LockRow(int row)
        {
            foreach (var entry in wordleGrid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) == row))
            {
                entry.IsEnabled = false; // LOCKS row
            }
        }
        private void EnableRow(int row)
        {
            foreach (var entry in wordleGrid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) == row))
            {
                entry.IsEnabled = true; //UNLOCKS row
            }
        }
        private void StartNewGame()
        {
            currentRow = 0;
            attemptsLeft = 6;
            gameOver = false;
            selectedWord = words[new Random().Next(words.Count)];
            InitializeWordleGrid();
            EnableRow(0);  // Enable only the first row at the start
            attemptsLabel.Text = $"Attempts left: {attemptsLeft}";
            feedbackLabel.Text = "Try to guess the word!";

            bool cheatModeActivated = Preferences.Get("CheatMode", false);  

            if (cheatModeActivated)
            {
                answerLabel.Text = $"The Wordle Answer is: {selectedWord}";
                answerLabel.IsVisible = true; // Answer appears when cheat mode is on
            }
            else
            {
                answerLabel.IsVisible = false; // Hidden when cheat mode is off
            }
        }
        private void EndGame(bool won) //ends the game
        {
            gameOver = true;
            feedbackLabel.Text = won ? "You won!" : $"Game Over! The word was: {selectedWord}"; //if you won, print this
        }
        private string[] GetGuessFeedback(string guess, string word)
        {
            var feedback = new string[5];

            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == word[i])
                {
                    feedback[i] = "green";  // correct letter, correct position
                }
                else if (word.Contains(guess[i]))
                {
                    feedback[i] = "yellow";  // correct letter, wrong position
                }
                else
                {
                    feedback[i] = "gray";  // incorrect overall
                }
            }
            return feedback;
        }
        private void UpdateWordleGrid(string guess, string[] feedback, int row)
        {
            for (int i = 0; i < 5; i++)
            {
                var entry = GetGridEntry(row, i);
                if (entry != null)
                {
                    entry.Text = guess[i].ToString().ToUpper(); // updating the text on the grid

                    // Setting background colour based on the guess
                    entry.BackgroundColor = feedback[i] switch
                    {
                        "green" => Colors.Green,
                        "yellow" => Colors.Yellow,
                        _ => Colors.Gray //if wrong (word doesnt exist in correct word)
                    };

                    //making sure text is still visible by setting text colour to black
                    entry.TextColor = Colors.Black;

                    //disable the entry once finished
                    entry.IsEnabled = false;
                }
            }
        }
        private Entry GetGridEntry(int row, int col)
        {
            return wordleGrid.Children.OfType<Entry>()
                .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
        }
        private void OnNewGame(object sender, EventArgs e)
        {
            StartNewGame(); //Starting a new game when "New Game" is pressed
        }

        private async void OnSettingsClicked(object sender, EventArgs e) //Settings button (for settings emoji in top right)
        {
            // Navigate to SettingsPage when the settings icon is clicked
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnPlayerHistoryClicked(object sender, EventArgs e)//Player History Button (for player history page)
        {
            // Navigate to the Player History Page when icon clicked
            await Navigation.PushAsync(new PlayerHistoryPage());
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            // check if cheat mode is on
            bool cheatModeActivated = Preferences.Get("CheatMode", false);  // default is false

            if (cheatModeActivated)
            {
                // show when cheat mode on
                answerLabel.Text = $"The Wordle Answer is: {selectedWord}";
                answerLabel.IsVisible = true;  // visible
            }
            else
            {
                // hide when cheat mode is off
                answerLabel.IsVisible = false;  
            }
        }

        private string LoadPlayerName() //gets player name
        {
            var playerNameFile = Path.Combine(FileSystem.AppDataDirectory, "player_name.txt");

            if (File.Exists(playerNameFile))
            {
                return File.ReadAllText(playerNameFile);
            }

            return "Player";  
        }

        private void SavePlayerHistory(bool won)
        {
            var historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "player_history.txt");

            // line with game details
            string historyLine = $"{PlayerName}|{DateTime.Now:MM/dd/yyyy HH:mm:ss}|{selectedWord}|{attemptsLeft}|{GetEmojiGrid()}";  //storing player history


            File.AppendAllText(historyFilePath, historyLine + Environment.NewLine);
        }



        private string GetEmojiGrid()
        {
            string emojiGrid = "";

            // loop through each row for feedback
            for (int row = 0; row < 6; row++)
            {
                // Get the feedback for the current row
                string rowFeedback = string.Join("", GetRowFeedback(row)); // join the feedback tohether

                // Only add a row if it contains any guess text
                if (!string.IsNullOrWhiteSpace(rowFeedback))
                {
                    emojiGrid += rowFeedback + "\t";
                }
            }

            return emojiGrid.Trim(); // ensures clean end
        }

        private string[] GetRowFeedback(int row)
        {
            var feedback = new string[5]; //string for feedback

            for (int i = 0; i < 5; i++)
            {
                var entry = GetGridEntry(row, i);
                if (entry != null)
                {
                    feedback[i] = entry.BackgroundColor == Colors.Green ? "🟩" :
                                  entry.BackgroundColor == Colors.Yellow ? "🟨" : "⬛"; 
                }
            }

            
            return new string[] { string.Join("", feedback) };
        }

    }
}
