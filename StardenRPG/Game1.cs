using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardenRPG.Screens;
using StardenRPG.StateManagement;

using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG
{
        // Sample showing how to manage different game states, with transitions
        // between menu screens, a loading screen, the game itself, and a pause
        // menu. This main game class is extremely simple: all the interesting
        // stuff happens in the ScreenManager component.

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;
        //private SpriteBatch _spriteBatch;

        // Scaling System
        public Vector2 ScaleFactor { get; private set; }

        // Physics
        private World _world;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            AudioManager audioManager = new AudioManager(this);

            _screenManager = new ScreenManager(this);

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // No copyright music - CC0(Public Domain)
            //_screenManager.BackgroundSongAsset = "Audio/Music/battleThemeA";

            // ✰ Star
            _screenManager.BackgroundSongAsset = "Audio/Music/Beyond-Reckoning_Full";

            Components.Add(_screenManager);

            AddInitialScreens();
        }

        public void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);


            //_world = new World(new Vector2(0, -160f)); // Initialize physics world with gravity.

            //_screenManager.AddScreen(new GameplayScreen(_world), new PlayerIndex());
            //_screenManager.AddScreen(new TestScreen(_world), new PlayerIndex());
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "Starden RPG";
            Window.AllowUserResizing = true;

            // Calculate the scale of the game world
            //ScaleFactor = CalculateScaleFactor();

            // Set resolution Fullscreen
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}