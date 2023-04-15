﻿using Microsoft.Xna.Framework;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
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
            _world = new World(new Vector2(0, -10f)); // Initialize physics world with gravity.

            _screenManager.AddScreen(new GameplayScreen(_world), new PlayerIndex());
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "Starden RPG";
            Window.AllowUserResizing = true;

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