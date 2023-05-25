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
using tainicom.Aether.Physics2D.Common;

namespace StardenRPG.Screens
{
    public class GameplayScreen : PhysicsGameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        // scaling
        private readonly Vector2 _scaleFactor;

        protected Player player;
        protected Slime slime;
        
        // New Ground
        private Body _ground;

        // Old Ground
        protected Ground groundOBJ, groundOBJ2;
        private Texture2D _groundTexture, _groundTexture2;
        private Vector2 groundPosition;
        float groundWidth, groundHeight;

        // Physics
        //private World _world;

        // Parallax Background
        private ParallaxBackground _parallaxBackground;

        // Add the input state object
        private InputState input = new InputState();

        public GameplayScreen(World world, Vector2 scaleFactor)
        {
            World = world;
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
                base.Activate(instancePreserved);

                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");

                //_gameFont = _content.Load<SpriteFont>("Fonts/gamefont");

                // Initialize the camera
                EnableCameraControl = true;
                //_camera.CameraBounds = new Rectangle(0, 0, 10000, 1080);
                //_camera.CharacterOffset = new Vector2(399, 0);

                // Initialize the ground texture
                _groundTexture = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);
                _groundTexture2 = new Texture2D(ScreenManager.Game.GraphicsDevice, 1, 1);
                _groundTexture2.SetData(new[] { Color.White });
                CreateGround();

                // terrain
                _ground = World.CreateBody();
                {
                    Vertices terrain = new Vertices();
                    terrain.Add(new Vector2(-20f, 5f));
                    terrain.Add(new Vector2(-20f, 0f));
                    terrain.Add(new Vector2(20f, 0f));
                    terrain.Add(new Vector2(25f, 0.25f));
                    terrain.Add(new Vector2(30f, 1f));
                    terrain.Add(new Vector2(35f, 4f));
                    terrain.Add(new Vector2(40f, 0f));
                    terrain.Add(new Vector2(45f, 0f));
                    terrain.Add(new Vector2(50f, -1f));
                    terrain.Add(new Vector2(55f, -2f));
                    terrain.Add(new Vector2(60f, -2f));
                    terrain.Add(new Vector2(65f, -1.25f));
                    terrain.Add(new Vector2(70f, 0f));
                    terrain.Add(new Vector2(75f, 0.3f));
                    terrain.Add(new Vector2(80f, 1.5f));
                    terrain.Add(new Vector2(85f, 3.5f));
                    terrain.Add(new Vector2(90f, 0f));
                    terrain.Add(new Vector2(95f, -0.5f));
                    terrain.Add(new Vector2(100f, -1f));
                    terrain.Add(new Vector2(105f, -2f));
                    terrain.Add(new Vector2(110f, -2.5f));
                    terrain.Add(new Vector2(115f, -1.3f));
                    terrain.Add(new Vector2(120f, 0f));
                    terrain.Add(new Vector2(160f, 0f));
                    terrain.Add(new Vector2(159f, -10f));
                    terrain.Add(new Vector2(201f, -10f));
                    terrain.Add(new Vector2(200f, 0f));
                    terrain.Add(new Vector2(240f, 0f));
                    terrain.Add(new Vector2(250f, 5f));
                    terrain.Add(new Vector2(250f, -10f));
                    terrain.Add(new Vector2(270f, -10f));
                    terrain.Add(new Vector2(270f, 0));
                    terrain.Add(new Vector2(310f, 0));
                    terrain.Add(new Vector2(310f, 5));

                    for (int i = 0; i < terrain.Count - 1; ++i)
                    {
                        var gfixture = _ground.CreateEdge(terrain[i], terrain[i + 1]);
                        gfixture.Friction = 0.6f;
                    }
                }

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
                Point characterSize = new Point(288 * 3, 128 * 3);
                GeneratePlayerAvatar(characterSize);

                // Create the slime
                Point slimeSize = new Point(64, 48);
                //GenerateSlime(slimeSize);

                Camera.MinRotation = -0.05f;
                Camera.MaxRotation = 0.05f;

                Camera.TrackingBody = player.Body;
                Camera.EnableTracking = true;

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
                
            Texture2D characterSpriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/FireKnight");
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(characterSpriteSheet.Width, characterSpriteSheet.Height), new Vector2(10, 3));

            Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "PlayerIdle", sacg.Generate("PlayerIdle", new Vector2(0, 0), new Vector2(7, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerWalkLeft", sacg.Generate("PlayerWalkLeft", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerWalkRight", sacg.Generate("PlayerWalkRight", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerAttack", sacg.Generate("PlayerAttack", new Vector2(0, 2), new Vector2(9, 2), new TimeSpan(0, 0, 0, 0, 400), false)},
            };
                
            //Vector2 playerStartPosition = new Vector2(100, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight - size.Y);
            Vector2 playerStartPosition = new Vector2(100, groundPosition.Y - size.Y);
            //Vector2 playerStartPosition = new Vector2(100, 500);

            // Player Mass
            float playerMass = 60f;

            player = new Player(characterSpriteSheet, size, new Point(288, 128), World, playerStartPosition, spriteAnimationClips);
            player.ControllingPlayer = PlayerIndex.One;
                
            // Set the player's physics
            //player.Body.Mass = playerMass;
            player.Body.LinearDamping = 10f; // Adjust this value to fine-tune the character's speed
            //player.Body.SetFriction(1f);
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

            slime = new Slime(slimeSpriteSheet, size, new Point(64, 64), World, slimeStartPosition, spriteAnimationClips);

            //// Set the player's physics
            //player.Body.Mass = playerMass;
            //player.Body.LinearDamping = 10f; // Adjust this value to fine-tune the character's speed
            //player.Body.SetFriction(1f);
        }

        private void CreateGround()
        {
            // Create the ground
            groundWidth = ScreenManager.Game.GraphicsDevice.Viewport.Width * 10;
            groundHeight = 350f - 110f - 110; // - 110 is bug
            groundPosition = new Vector2(0, ScreenManager.Game.GraphicsDevice.Viewport.Height - groundHeight);

            groundOBJ = new Ground(_groundTexture2, groundWidth, groundHeight, groundPosition, World);

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

        /*public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
            
            // Pass input to the player's HandleInput method
            player.HandleInput(input);

        }*/

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                base.HandleInput(gameTime, input);
                _player.HandleInput(gameTime, input); //error CS1061
            }
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
                //World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

                // Update the player Avatar
                player.Update(gameTime);

                // Update the slime
                //slime.Update(gameTime, player.Position);

                // Update Camera
                Camera.Update(gameTime);

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
                        movementDirection = new Vector2(0, 0);
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
            var batchEffect = ScreenManager.BatchEffect;

            batchEffect.View = Camera.View;
            batchEffect.Projection = Camera.Projection;

            /* Old Code of draw sprite + camera(Old) */
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, _camera.GetViewMatrix());
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            // Draw Background..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestBG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            // Draw the parallax background
            //_parallaxBackground.Draw(spriteBatch, _camera.Position, _camera.GetViewMatrix());

            // Draw the player Avatar
            player.Draw(gameTime, spriteBatch, SpriteEffects.None); // Replace 'playerAvatar.Draw(gameTime, spriteBatch);' with this line

            // Draw the slime
            //slime.Draw(gameTime, spriteBatch, SpriteEffects.None);

            //groundOBJ.Draw(spriteBatch); // Replace the existing ground drawing code with this line
            //groundOBJ2.Draw(spriteBatch);
            
            // New Ground
            ScreenManager.LineBatch.Begin(Camera.Projection, Camera.View);
            // draw ground
            foreach (Fixture fixture in _ground.FixtureList)
            {
                ScreenManager.LineBatch.DrawLineShape(fixture.Shape, Color.Black);
            }
            ScreenManager.LineBatch.End();


            // Draw Foreground..
            //spriteBatch.Draw(_content.Load<Texture2D>("Backgrounds/TestFG"), new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), null, Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            base.Draw(gameTime);
        }
    }
}
