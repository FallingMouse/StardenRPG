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

        // Physics
        private World _world;
        // Ground Test
        private Texture2D _rect;
        private SpriteBatch _spriteBatch;
        private Body groundBody;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            /*_graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();*/
            
            AudioManager audioManager = new AudioManager(this);

            _screenManager = new ScreenManager(this);
            // No copyright music - CC0(Public Domain)
            //_screenManager.BackgroundSongAsset = "Audio/Music/battleThemeA";

            // ✰ Star
            // Don't for get to delete this music cause it copyright
            _screenManager.BackgroundSongAsset = "Audio/Music/Delete_ReleaseTheFire";

            Components.Add(_screenManager);

            AddInitialScreens();
        }

        public void AddInitialScreens()
        {
            _world = new World(new Vector2(0, 9.81f)); // Initialize physics world with gravity.
            
            // Add Ground
            CreateGround();

            _screenManager.AddScreen(new GameplayScreen(_world), new PlayerIndex());
        }

        private void CreateGround()
        {
            //Vector2 groundPosition = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height - 10f);
            Vector2 groundPosition = new Vector2(0, 300);
            //Vector2 groundSize = new Vector2(GraphicsDevice.Viewport.Width, 20f);
            Vector2 groundSize = new Vector2(_graphics.PreferredBackBufferWidth, 20f);

            groundBody = _world.CreateRectangle(groundSize.X, groundSize.Y, 1f, groundPosition, 0, BodyType.Static);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            /*_spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _rect = new Texture2D(_graphics.GraphicsDevice, 1, 1);

            Color[] data = new Color[1];
            data[0] = Color.White;
            _rect.SetData(data);*/
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
            //_spriteBatch.Draw(_rect, groundBody.Position, null, Color.White, groundBody.Rotation, Vector2.Zero, new Vector2(800, 20), SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}