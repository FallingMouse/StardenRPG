using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;

using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        protected Sprite playerAvatar;

        // Ground
        private Texture2D _groundTexture;
        private Body _groundBody;
        float groundWidth, groundHeight;

        // Physics
        private World _world;

        public GameplayScreen(World world)
        {
            _world = world;
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
                GeneratePlayerAvatar(size);


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        protected void GeneratePlayerAvatar(Point size)
        {
                Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/Warrior_Sheet-Effect");
                //Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/test");
                SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(6, 17));
                //SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(6, 3));

                Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
                {
                    { "Idle", sacg.Generate("Idle", new Vector2(0, 0), new Vector2(5, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkLeft", sacg.Generate("WalkLeft", new Vector2(1, 2), new Vector2(0, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "WalkRight", sacg.Generate("WalkRight", new Vector2(0, 1), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 500), true) },
                };
                
                //Vector2 playerStartPosition = new Vector2(100, 200);
                //Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y - 5);
                Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y);

                playerAvatar = new Sprite(spriteSheet, size, new Point(69, 44), _world, playerStartPosition);

                playerAvatar.animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
                playerAvatar.StartAnimation("Idle");
                //playerAvatar.Position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width / 2, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2);

                // Create a physics body for the player
                /*float width = size.X;
                float height = size.Y;
                float mass = 10f; // Adjust mass as needed
                Body playerBody = _world.CreateRectangle(width, height, mass, playerStartPosition, 0);
                playerBody.BodyType = BodyType.Dynamic;
                *//*playerBody.LinearDamping= 5f; // Adjust this value as needed
                playerBody.SetRestitution(0.1f); // Set to a value between 0 and 1, lower values will result in less bouncing
                playerBody.SetFriction(0.7f); // Set to a value between 0 and 1, higher values will result in more friction*//*
                playerAvatar.PhysicsBody = playerBody; // Assign the created body to the playerAvatar*/
        }

        private void CreateGround()
        {
            // Create the ground
            groundWidth = ScreenManager.Game.GraphicsDevice.Viewport.Width;
            groundHeight = 40f;
            Vector2 groundPosition = new Vector2(0, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight);
            _groundBody = _world.CreateRectangle(groundWidth, groundHeight, 1, groundPosition);
            _groundBody.BodyType = BodyType.Static;
        }

        protected override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
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
                playerAvatar.Update(gameTime);

                //float translateSpeed = 2.5f;
                Vector2 movementDirection = Vector2.Zero;
                float moveSpeed = 100f; // Adjust the movement speed as needed

                switch (playerAvatar.animationPlayer.CurrentClip.Name)
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
                playerAvatar.Body.LinearVelocity = movementDirection * moveSpeed;
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                if ((input.IsKeyPressed(Keys.Left, ControllingPlayer, out player)) ||
                    (input.IsKeyPressed(Keys.A, ControllingPlayer, out player)))
                    playerAvatar.animationPlayer.StartClip("WalkLeft");
                else if ((input.IsKeyPressed(Keys.Right, ControllingPlayer, out player)) || 
                    (input.IsKeyPressed(Keys.D, ControllingPlayer, out player)))
                    playerAvatar.animationPlayer.StartClip("WalkRight");
                else
                    playerAvatar.animationPlayer.StartClip("Idle");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            // Draw Background..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            // Draw the player Avatar
            playerAvatar.Draw(gameTime, spriteBatch);

            // Draw the ground
            spriteBatch.Draw(
                _groundTexture,
                new Rectangle((int)_groundBody.Position.X, (int)_groundBody.Position.Y, (int)groundWidth, (int)groundHeight),
                Color.White
            );

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
