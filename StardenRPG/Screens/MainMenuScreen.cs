using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {
        // Scaling System
        public Vector2 ScaleFactor { get; private set; }

        // Physics
        private World _world;

        public MainMenuScreen() : base("Main Menu")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _world = new World(new Vector2(0, -10f)); // Initialize physics world with gravity.
            ScaleFactor = CalculateScaleFactor(); // Calculate the scale of the game world

            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(_world, ScaleFactor) { ScreenManager = ScreenManager
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new TestScreen(_world, ScaleFactor) { ScreenManager = ScreenManager });
        }

        private Vector2 CalculateScaleFactor()
        {
            int baseWidth = 1920; // Base resolution width: 1920
            int baseHeight = 1080; // Base resolution height: 1080

            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            float scaleX = (float)screenWidth / baseWidth;
            float scaleY = (float)screenHeight / baseHeight;
            return new Vector2(scaleX, scaleY);
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}