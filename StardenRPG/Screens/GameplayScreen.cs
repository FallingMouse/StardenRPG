using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Entities;

using tainicom.Aether.Physics2D.Dynamics;
using System.Text.RegularExpressions;

namespace StardenRPG.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        // scaling
        private readonly Vector2 _scaleFactor;

        protected Player player; // Replace 'Sprite playerAvatar;' with this line
        //protected Sprite playerAvatar;
        protected Ground ground; // Add this line

        // Ground
        private Texture2D _groundTexture;
        private Body _groundBody;
        private Vector2 groundPosition;
        float groundWidth, groundHeight;

        // Physics
        private World _world;

        // Add the input state object
        private InputState input = new InputState();

        public GameplayScreen(World world, Vector2 scaleFactor)
        {
            _world = world;
            _scaleFactor = scaleFactor;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");

                //_gameFont = _content.Load<SpriteFont>("Fonts/gamefont");

                // Initialize the ground texture
                _groundTexture = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);
                _groundTexture.SetData(new[] { Color.White });

                CreateGround();

                //Point size = new Point(32, 40);
                Point size = new Point(138, 88);
                //Point size = new Point(278, 176);
                GeneratePlayerAvatar(size);


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected void GeneratePlayerAvatar(Point size)
        {
                /* Old Test character, just an example */
                //Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/test");
                //SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(6, 3));
                //Vector2 playerStartPosition = new Vector2(100, 200);
                
                Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/Warrior_Sheet-Effect");
                SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(6, 17));

                Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
                {
                    { "Idle", sacg.Generate("Idle", new Vector2(0, 0), new Vector2(5, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkLeft", sacg.Generate("WalkLeft", new Vector2(1, 2), new Vector2(0, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkRight", sacg.Generate("WalkRight", new Vector2(0, 1), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                };
                
                //Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y);
                Vector2 playerStartPosition = new Vector2(100, groundPosition.Y - size.Y ); // -100 becuase I wan't to test add Mass

                // Player Mass
                float playerMass = 60f;

                player = new Player(spriteSheet, size, new Point(69, 44), _world, playerStartPosition, spriteAnimationClips);
                player.ControllingPlayer = PlayerIndex.One;
                //playerAvatar = new Sprite(spriteSheet, size, new Point(69, 44), _world, playerStartPosition);

                //playerAvatar.animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
                //playerAvatar.StartAnimation("Idle");
        }

        private void CreateGround()
        {
            // Create the ground
            groundWidth = ScreenManager.Game.GraphicsDevice.Viewport.Width;
            groundHeight = 80f;
            groundPosition = new Vector2(0, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight);

            ground = new Ground(_groundTexture, groundWidth, groundHeight, groundPosition, _world);
        }

        protected override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            // Pass input to the player's HandleInput method
            player.HandleInput(input);
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Update the physics world
                _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

                // Update the player Avatar
                //playerAvatar.Update(gameTime);
                player.Update(gameTime); // Replace 'playerAvatar.Update(gameTime);' with this line

                //64 pixels on your screen should be 1 meter in the physical world
                Vector2 movementDirection = Vector2.Zero;
                float moveSpeed = 25600000f; // Adjust the movement speed as needed

                switch (player.animationPlayer.CurrentClip.Name)
                {
                    case "WalkLeft":
                        movementDirection = new Vector2(-1, 0);
                        break;
                    case "WalkRight":
                        movementDirection = new Vector2(1, 0);
                        break;
                    case "Idle":
                        break;
                }

                //playerAvatar.Position = Vector2.Min(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - playerAvatar.Size.X, ScreenManager.GraphicsDevice.Viewport.Height - playerAvatar.Size.Y), Vector2.Max(Vector2.Zero, playerAvatar.Position));
                player.Body.LinearVelocity = movementDirection * moveSpeed;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            /* Old SpriteBatch Begin Code */
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            /* This change will apply the scaling factor to all the sprites drawn within the spriteBatch.Begin and spriteBatch.End calls. */
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(_scaleFactor.X, _scaleFactor.Y, 1));

            // Draw Background..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            // Draw the player Avatar
            //playerAvatar.Draw(gameTime, spriteBatch);
            player.Draw(gameTime, spriteBatch); // Replace 'playerAvatar.Draw(gameTime, spriteBatch);' with this line

            // Draw the ground
            /*spriteBatch.Draw(
                _groundTexture,
                new Rectangle((int)_groundBody.Position.X, (int)_groundBody.Position.Y, (int)groundWidth, (int)groundHeight),
                Color.White
            );*/
            ground.Draw(spriteBatch); // Replace the existing ground drawing code with this line

            // Draw Foreground..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestFG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
