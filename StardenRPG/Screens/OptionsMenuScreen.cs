namespace StardenRPG.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
        private readonly MenuEntry controlsMenu;
        private readonly MenuEntry audioMenu;

        public OptionsMenuScreen() : base("")
        {
            controlsMenu = new MenuEntry(string.Empty);
            audioMenu= new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("BACK");

            controlsMenu.Selected += ControlsMenuEntrySelected;
            audioMenu.Selected += AudioMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(back);
            MenuEntries.Add(controlsMenu);
            MenuEntries.Add(audioMenu);
 
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            controlsMenu.Text = "CONTROLS";
            audioMenu.Text = "AUDIO";
        }

        private void ControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlsOptionsScreen(), e.PlayerIndex);
        }

        private void AudioMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new AudioOptionsScreen(), e.PlayerIndex);
        }
    }
}