using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Utilities;
using StardenRPG.Entities.Character;

using tainicom.Aether.Physics2D.Dynamics;
using System.Text.RegularExpressions;
using StardenRPG.Entities.Monster;

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

        protected Player player;
        protected Slime slime;
        protected Ground ground, ground2; // Add this line

        // Ground
        private Texture2D _groundTexture, _groundTexture2;
        private Body _groundBody;
        private Vector2 groundPosition;
        float groundWidth, groundHeight;

        // Physics
        private World _world;

        // Camera
        private Camera2D _camera;

        // Parallax Background
        private ParallaxBackground _parallaxBackground;

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

                // Initialize the camera
                _camera = new Camera2D(ScreenManager.Game.GraphicsDevice);
                _camera.CameraBounds = new Rectangle(0, 0, 10000, 1080);
                _camera.CharacterOffset = new Vector2(100, -50);

                // Initialize the ground texture
                _groundTexture = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);
                _groundTexture2 = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);
                _groundTexture2.SetData(new[] { Color.White });

                CreateGround();

                // Load background textures
                Texture2D[] backgroundLayers = new Texture2D[12];
                backgroundLayers[11] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0000_9");
                backgroundLayers[10] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0001_8");
                backgroundLayers[9] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0002_7");
                backgroundLayers[8] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0003_6");
                backgroundLayers[7] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0004_Lights");
                backgroundLayers[6] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0005_5");
                backgroundLayers[5] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0006_4");
                backgroundLayers[4] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0007_Lights");
                backgroundLayers[3] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0008_3");
                backgroundLayers[2] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0009_2");
                backgroundLayers[1] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0010_1");
                backgroundLayers[0] = _content.Load<Texture2D>("Backgrounds/Background layers/Layer_0011_0");

                // Create ParallaxBackground instance
                float[] parallaxFactors = new float[] { 1f, 0.9f, 0.8f, 0.8f, 0.7f, 0.6f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0f };
                _parallaxBackground = new ParallaxBackground(backgroundLayers, parallaxFactors, ScreenManager.GraphicsDevice.Viewport);

                // Create the player
                Point characterSize = new Point(72, 132);
                GeneratePlayerAvatar(characterSize);

                // Create the slime
                Point slimeSize = new Point(64, 48);
                GenerateSlime(slimeSize);

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
                
                Texture2D characterSpriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/Warrior_Sheet-Effect");
                SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(characterSpriteSheet.Width, characterSpriteSheet.Height), new Vector2(8, 3));

                Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
                {
                    { "PlayerIdle", sacg.Generate("PlayerIdle", new Vector2(0, 0), new Vector2(5, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    { "PlayerWalkLeft", sacg.Generate("PlayerWalkLeft", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 1000), true) },
                    { "PlayerWalkRight", sacg.Generate("PlayerWalkRight", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 1000), true) },
                    { "PlayerAttack", sacg.Generate("PlayerAttack", new Vector2(0, 2), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 120), false)},
                };
                
                //Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y);
                Vector2 playerStartPosition = new Vector2(100, groundPosition.Y - size.Y);
                //Vector2 playerStartPosition = new Vector2(100, 500);

                // Player Mass
                float playerMass = 60f;

                player = new Player(characterSpriteSheet, size, new Point(69, 44), _world, playerStartPosition, spriteAnimationClips);
                player.ControllingPlayer = PlayerIndex.One;
                
                // Set the player's physics
                player.Body.Mass = playerMass;
                player.Body.LinearDamping = 10f; // Adjust this value to fine-tune the character's speed
                //player.Body.SetFriction(1f);
                
                // Tell the camera to follow the player
                _camera.Follow(player);
        }

        protected void GenerateSlime(Point size)
        {
            Texture2D slimeSpriteSheet = _content.Load<Texture2D>("Sprites/Monster/Slime/Normal/Slime");
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(slimeSpriteSheet.Width, slimeSpriteSheet.Height), new Vector2(18, 4));

            Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
                {
                    { "SlimeIdle", sacg.Generate("SlimeIdle", new Vector2(0, 0), new Vector2(3, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                    //{ "PlayerWalkLeft", sacg.Generate("PlayerWalkLeft", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 1000), true) },
                    //{ "PlayerWalkRight", sacg.Generate("PlayerWalkRight", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 1000), true) },
                    //{ "PlayerAttack", sacg.Generate("PlayerAttack", new Vector2(0, 2), new Vector2(1, 2), new TimeSpan(0, 0, 0, 0, 120), false)},
                };

            //Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y);
            Vector2 slimeStartPosition = new Vector2(1000, groundPosition.Y - size.Y);
            //Vector2 playerStartPosition = new Vector2(100, 500);

            // Player Mass
            float playerMass = 60f;

            slime = new Slime(slimeSpriteSheet, size, new Point(64, 41), _world, slimeStartPosition, spriteAnimationClips);

            //// Set the player's physics
            //player.Body.Mass = playerMass;
            //player.Body.LinearDamping = 10f; // Adjust this value to fine-tune the character's speed
            //player.Body.SetFriction(1f);
        }

        private void CreateGround()
        {
            // Create the ground
            groundWidth = ScreenManager.Game.GraphicsDevice.Viewport.Width * 10;
            groundHeight = 350f - 110f; // - 110 is bug
            groundPosition = new Vector2(0, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight);

            ground = new Ground(_groundTexture, groundWidth, groundHeight, groundPosition, _world);

            // test player collider
            //ground2 = new Ground(_groundTexture2, 80f, 500f, new Vector2(600, 500), _world);
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
                player.Update(gameTime);

                // Update the slime
                slime.Update(gameTime, player.Position);

                // Update Camera
                _camera.Update(gameTime);

                //64 pixels on your screen should be 1 meter in the physical world
                Vector2 movementDirection = Vector2.Zero;

                float baseSpeed = 55000f;
                float runningMultiplier = baseSpeed * 64f;
                float moveSpeed = player.IsRunning ? baseSpeed * runningMultiplier : baseSpeed * runningMultiplier;

                switch (player.animationPlayer.CurrentClip.Name)
                {
                    case "PlayerWalkLeft":
                        movementDirection = new Vector2(-1, 0);
                        break;
                    case "PlayerWalkRight":
                        movementDirection = new Vector2(1, 0);
                        break;
                    case "PlayerIdle":
                        break;
                    case "PlayerAttack":
                        break;
                }

                //player.Body.LinearVelocity = movementDirection * moveSpeed;
                player.Body.ApplyForce(movementDirection * moveSpeed);
                //player.Body.ApplyLinearImpulse(movementDirection * moveSpeed);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            /* Old SpriteBatch Begin Code */
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            /* This change will apply the scaling factor to all the sprites drawn within the spriteBatch.Begin and spriteBatch.End calls. */
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.CreateScale(_scaleFactor.X, _scaleFactor.Y, 1));
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetViewMatrix());
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, _camera.GetViewMatrix());

            // Draw Background..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            // Draw the parallax background
            _parallaxBackground.Draw(spriteBatch, _camera.Position, _camera.GetViewMatrix());

            // Draw the player Avatar
            player.Draw(gameTime, spriteBatch, SpriteEffects.None); // Replace 'playerAvatar.Draw(gameTime, spriteBatch);' with this line

            // Draw the slime
            slime.Draw(gameTime, spriteBatch, SpriteEffects.None);

            ground.Draw(spriteBatch); // Replace the existing ground drawing code with this line
            //ground2.Draw(spriteBatch);

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
