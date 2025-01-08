using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;  // For Preferences
using System;

namespace WordleX
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            // update cheat mode status on page loading
            UpdateCheatModeStatus();
        }

        
        private void OnToggleCheatModeButtonClicked(object sender, EventArgs e)
        {
            // Get the current cheat mode status
            bool isCheatMode = Preferences.Get("CheatMode", false);

            // TToggling cheat mode to opposite mode
            isCheatMode = !isCheatMode;

            // save cheat mode value
            Preferences.Set("CheatMode", isCheatMode);

            // updating button
            UpdateCheatModeStatus();
        }

        // update button based on cheat mode status
        private void UpdateCheatModeStatus()
        {
            // Check the current cheat mode status based on the preference
            bool isCheatMode = Preferences.Get("CheatMode", false);

            // Change text in button based on whether it is on or off
            toggleCheatModeButton.Text = isCheatMode ? "Deactivate Cheat Mode" : "Activate Cheat Mode";
        }
    }
}
