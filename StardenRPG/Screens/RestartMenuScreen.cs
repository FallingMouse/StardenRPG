using StardenRPG.Entities.Character;

namespace StardenRPG.Screens
{
    // The restart menu comes up over the top of the game,
    // giving the player options to restart or quit.
    public class RestratMenuScreen : MenuScreen
    {
        public RestratMenuScreen() : base("YOUR   DIED")
        {
            var resumeGameMenuEntry = new MenuEntry("RESTART   GAME");
            var quitGameMenuEntry = new MenuEntry("QUIT   GAME");
            
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "ARE   YOU   SURE   YOU   WANT   TO   QUIT   THIS   GAME?";
            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        // This uses the loading screen to transition from the game back to the main menu screen.
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}