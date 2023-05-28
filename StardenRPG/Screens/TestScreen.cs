/* Original source Farseer Physics Engine:
* Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
* Microsoft Permissive License (Ms-PL) v1.1
*/

using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Utilities;
using StardenRPG.Entities.Character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics.Tracing;
using StardenRPG.Entities.Monster;
using StardenRPG.Entities.Bar;

namespace StardenRPG.Screens
{
    class TestScreen : PhysicsGameScreen
    {
        #region fields 
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        // scaling
        private readonly Vector2 _scaleFactor;

        protected Player _player;
        protected Slime slime, slime2;

        private Body _ground;

        /* Car and Wheels */
        private Body _car;
        private Body _wheelBack;
        private Body _wheelFront;
        private WheelJoint _springBack;
        private WheelJoint _springFront;

        private Sprite _carBody;
        private Sprite _wheel;

        //import bg
        private Sprite _bgForestTexture, _bgForest2Texture, _bgLabatory1Texture, _bgLabatory2Texture, _bgForesttoLabTexture;
        private Vector2 _bgBodySize = new Vector2(80f, 26f); 
        private World _world;

        //all about the ground & bg
        //private Texture2D _groundForestTexture;
        private Sprite _groundForestTexture, _groundLabTexture;
        private Vector2 _groundTextureSize, _bgTextureSize;
        private Vector2 _groundTextureOrigin, _bgTextureOrigin;

        private Body _groundBody, _bgBody;
        private Vector2 _groundBodySize = new Vector2(80f ,20f); //divide 16


        private float _acceleration;
        private const float MaxSpeed = 50.0f;

        // Health Bar
        private HealthBar _healthBar, _healthBarTestNewSlime;
        private Texture2D _healthBox;

        // Add the input state object
        private InputState input = new InputState();
        #endregion


        #region constructors
        public TestScreen(World world, Vector2 scaleFactor)
        {
            World = world;
            _scaleFactor = scaleFactor;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);

        }
        #endregion

        // Load graphics content for the game
        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            EnableCameraControl = true;

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            #region Create Object
            //BG
            //_background = new Sprite(ScreenManager.Content.Load<Texture2D>("Samples/Map1"), new Vector2(0f, 0f));
            //_background = new ContentManager.Load<Texture2D>("Samples/Map1");

            CreateBackground();
            // terrain
            CreateGround();

            CreateNewGround();

            // Car
            CreateCar();

            // Create the player
            Point characterSize = new Point(288 / 16, 128 / 16); // Default = 288, 128
            GeneratePlayerAvatar(characterSize);

            // Create the slime
            Point slimeSize = new Point(64 / 16, 48 / 16); // Default = 64, 48
            GenerateSlime(slimeSize);
            #endregion

            #region Camera
            Camera.MinRotation = -0.05f;
            Camera.MaxRotation = 0.05f;

            //Camera.TrackingBody = _car;
            Camera.TrackingBody = _player.Body;
            Camera.EnableTracking = true;
            #endregion

            #region Load Content
            // Health Bar
            _healthBar = new HealthBar(ScreenManager.Game.GraphicsDevice, _player);
            _healthBar = new HealthBar(ScreenManager.Game.GraphicsDevice, _player, slime);
            _healthBox = ScreenManager.Content.Load<Texture2D>("Backgrounds/Bar/HealthBox");
            _healthBar.SetHealthBox(_healthBox);

            _healthBarTestNewSlime = new HealthBar(ScreenManager.Game.GraphicsDevice, _player, slime2);
            //_healthBox = ScreenManager.Content.Load<Texture2D>("Backgrounds/Bar/HealthBox");
            _healthBarTestNewSlime.SetHealthBox(_healthBox);
            #endregion

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        #region Create Object
        private void CreateBackground()
        {
            //create the world
            _world = new World();

            Vector2 bgPosition = new Vector2(0, (_bgBodySize.Y / 2f));

            //create the bg
            _bgBody = _world.CreateBody(bgPosition, 0, BodyType.Static);
            var gfixture = _bgBody.CreateRectangle(_bgBodySize.X, _bgBodySize.Y, 1f, Vector2.Zero);

            _bgForestTexture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/mapbackground"));
            _bgForest2Texture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/map2bg"));
            _bgLabatory1Texture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/map41bg"));
            _bgLabatory2Texture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/map42bg"));
            _bgForesttoLabTexture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/map3bg"));

            _bgTextureSize = new Vector2(_bgForestTexture.Size.X, _bgForestTexture.Size.Y);

            _bgTextureOrigin = _bgTextureSize / 2f;
        }
        private void CreateNewGround()
        {
            //create the world
            _world = new World();

            Vector2 groundPosition = new Vector2(0, -(_groundBodySize.Y / 2f ));

            //create the ground
            _groundBody = _world.CreateBody(groundPosition, 0, BodyType.Static);
            var gfixture = _groundBody.CreateRectangle(_groundBodySize.X, _groundBodySize.Y, 1f, Vector2.Zero);

            _groundForestTexture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/fgfinall"));
            _groundLabTexture = new Sprite(ScreenManager.Content.Load<Texture2D>("Backgrounds/map3fg"));

            _groundTextureSize = new Vector2(_groundForestTexture.Size.X, _groundForestTexture.Size.Y);

            _groundTextureOrigin = _groundTextureSize / 2f;
        }
        private void CreateGround()
        {
            // terrain
            _ground = World.CreateBody();
            {
                Vertices terrain = new Vertices();
                terrain.Add(new Vector2(-25f, 10f));
                terrain.Add(new Vector2(-25f, 0f));
                terrain.Add(new Vector2(20f, 0f));
                terrain.Add(new Vector2(25f, 0f));
                terrain.Add(new Vector2(30f, 0f));
                terrain.Add(new Vector2(35f, 0f));
                terrain.Add(new Vector2(40f, 0f));
                terrain.Add(new Vector2(45f, 0f));
                terrain.Add(new Vector2(50f, 0f));
                terrain.Add(new Vector2(55f, 0f));
                terrain.Add(new Vector2(60f, 0f));
                terrain.Add(new Vector2(65f, 0f));
                terrain.Add(new Vector2(70f, 0f));
                terrain.Add(new Vector2(75f, 0f));
                terrain.Add(new Vector2(80f, 0f));
                terrain.Add(new Vector2(85f, 0f));
                terrain.Add(new Vector2(90f, 0f));
                terrain.Add(new Vector2(95f, 0f));
                terrain.Add(new Vector2(100f, 0f));
                terrain.Add(new Vector2(105f, 0f));
                terrain.Add(new Vector2(110f, 0f));
                terrain.Add(new Vector2(115f, 0f));
                terrain.Add(new Vector2(120f, 0f));
                terrain.Add(new Vector2(160f, 0f));
                terrain.Add(new Vector2(170f, 0f));
                terrain.Add(new Vector2(201f, 0f));
                terrain.Add(new Vector2(200f, 0f));
                terrain.Add(new Vector2(240f, 0f));
                terrain.Add(new Vector2(250f, 0f));
                terrain.Add(new Vector2(250f, 0f));
                terrain.Add(new Vector2(270f, 0f));
                terrain.Add(new Vector2(270f, 0));
                terrain.Add(new Vector2(420f, 0f));
                terrain.Add(new Vector2(420f, 10f));

                for (int i = 0; i < terrain.Count - 1; ++i)
                {
                    var gfixture = _ground.CreateEdge(terrain[i], terrain[i + 1]);
                    gfixture.Friction = 0.6f;
                }
            }
        }

        private void CreateCar()
        {
            // car
            {
                Vertices vertices = new Vertices(8);
                vertices.Add(new Vector2(-2.5f, -0.08f));
                vertices.Add(new Vector2(-2.375f, 0.46f));
                vertices.Add(new Vector2(-0.58f, 0.92f));
                vertices.Add(new Vector2(0.46f, 0.92f));
                vertices.Add(new Vector2(2.5f, 0.17f));
                vertices.Add(new Vector2(2.5f, -0.205f));
                vertices.Add(new Vector2(2.3f, -0.33f));
                vertices.Add(new Vector2(-2.25f, -0.35f));

                PolygonShape chassis = new PolygonShape(vertices, 2);
                CircleShape wheelShape = new CircleShape(0.5f, 0.8f);

                _car = World.CreateBody();
                _car.BodyType = BodyType.Static;
                _car.Position = new Vector2(0.0f + 10, 0.8f);
                _car.CreateFixture(chassis);
                _car.Tag = "Car";

                _wheelBack = World.CreateBody();
                _wheelBack.BodyType = BodyType.Dynamic;
                _wheelBack.Position = new Vector2(-1.709f + 10, 0.58f);
                var wFixture = _wheelBack.CreateFixture(wheelShape);
                wFixture.Friction = 0.9f;

                wheelShape.Density = 1;
                _wheelFront = World.CreateBody();
                _wheelFront.BodyType = BodyType.Dynamic;
                _wheelFront.Position = new Vector2(1.54f + 10, 0.6f);
                _wheelFront.CreateFixture(wheelShape);

                Vector2 axis = new Vector2(0.0f, 1.2f);
                _springBack = new WheelJoint(_car, _wheelBack, _wheelBack.Position, axis, true);
                _springBack.MotorSpeed = 0.0f;
                _springBack.MaxMotorTorque = 20.0f;
                _springBack.MotorEnabled = true;
                _springBack.Frequency = 4.0f;
                _springBack.DampingRatio = 0.7f;
                World.Add(_springBack);

                _springFront = new WheelJoint(_car, _wheelFront, _wheelFront.Position, axis, true);
                _springFront.MotorSpeed = 0.0f;
                _springFront.MaxMotorTorque = 10.0f;
                _springFront.MotorEnabled = false;
                _springFront.Frequency = 4.0f;
                _springFront.DampingRatio = 0.7f;
                World.Add(_springFront);

                _carBody = new Sprite(ScreenManager.Content.Load<Texture2D>("Samples/car"), Sprite.CalculateOrigin(_car, 24f));
                _wheel = new Sprite(ScreenManager.Content.Load<Texture2D>("Samples/wheel"));
            }
        }

        protected void GeneratePlayerAvatar(Point size)
        {
            /* Old Test character, just an example */
            //Texture2D spriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/test");
            //SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(spriteSheet.Width, spriteSheet.Height), new Vector2(6, 3));
            //Vector2 playerStartPosition = new Vector2(100, 200);

            //Texture2D characterSpriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/FireKnight");
            Texture2D characterSpriteSheet = _content.Load<Texture2D>("Sprites/Character/MainCharacter/FireKnight");
            SpriteAnimationClipGenerator sacg = new SpriteAnimationClipGenerator(new Vector2(characterSpriteSheet.Width, characterSpriteSheet.Height), new Vector2(13, 5));

            Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "PlayerIdle", sacg.Generate("PlayerIdle", new Vector2(0, 0), new Vector2(7, 0), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerWalkLeft", sacg.Generate("PlayerWalkLeft", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerWalkRight", sacg.Generate("PlayerWalkRight", new Vector2(0, 1), new Vector2(7, 1), new TimeSpan(0, 0, 0, 0, 500), true) },
                { "PlayerAttack", sacg.Generate("PlayerAttack", new Vector2(0, 2), new Vector2(9, 2), new TimeSpan(0, 0, 0, 0, 400), false)},
                { "PlayerTakeHit", sacg.Generate("PlayerTakeHit", new Vector2(0, 3), new Vector2(5, 3), new TimeSpan(0, 0, 0, 0, 500), false)},
                { "PlayerDeath", sacg.Generate("PlayerDeath", new Vector2(0, 4), new Vector2(12, 4), new TimeSpan(0, 0, 0, 0, 500), false)},
            };

            Vector2 playerStartPosition = new Vector2(31, 1); // default = 1, 1
            //Vector2 playerStartPosition = new Vector2(100, 500);

            _player = new Player(characterSpriteSheet, size, new Point(288, 128), World, playerStartPosition, spriteAnimationClips);
            _player.ControllingPlayer = PlayerIndex.One;

            // Set the player's physics
            _player.Body.LinearDamping = 10f; // Adjust this value to fine-tune the character's speed
            _player.Body.SetFriction(1f);
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

            Vector2 slimeStartPosition = new Vector2(20, 1); // default = 30, 1

            slime = new Slime(slimeSpriteSheet, size, new Point(64, 41), World, slimeStartPosition, spriteAnimationClips);
            slime.setPlayer(_player);
            slime.Body.LinearDamping = 10f;

            //another slime
            slime2 = new Slime(slimeSpriteSheet, size, new Point(64, 41), World, new Vector2(50, 1), spriteAnimationClips);
            slime2.setPlayer(_player);
            slime2.Body.LinearDamping = 10f;
        }
        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Update Camera
                Camera.Update(gameTime);

                // Update the player Avatar
                _player.Update(gameTime);

                // Update the Health Bar
                _healthBar.Update(gameTime, _player, slime);
                _healthBarTestNewSlime.Update(gameTime, _player, slime2);

                // Update the slime
                slime.Update(gameTime, _player);
                slime2.Update(gameTime, _player);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex player;

            // Pass input to the player's HandleInput method
            _player.HandleInput(gameTime, input);

            base.HandleInput(gameTime, input);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;
            var batchEffect = ScreenManager.BatchEffect;

            // Setup Camera
            batchEffect.View = Camera.View;
            batchEffect.Projection = Camera.Projection;


            #region SpriteBatch
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);

            spriteBatch.Draw(_bgForestTexture.TextureForSprite, new Vector2(-108f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgForestTexture.TextureForSprite, new Vector2(-28f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgForest2Texture.TextureForSprite, new Vector2(52f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgForesttoLabTexture.TextureForSprite, new Vector2(132f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgLabatory1Texture.TextureForSprite, new Vector2(212f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgLabatory2Texture.TextureForSprite, new Vector2(292f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_bgLabatory2Texture.TextureForSprite, new Vector2(372f, 0f), null, Color.White, 0f, _bgTextureOrigin, new Vector2(80f, 26f) * _bgForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);

            // draw car
            spriteBatch.Draw(_wheel.TextureForSprite, _wheelBack.Position, null, Color.White, _wheelBack.Rotation, _wheel.Origin, new Vector2(0.5f) * _wheel.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_wheel.TextureForSprite, _wheelFront.Position, null, Color.White, _wheelFront.Rotation, _wheel.Origin, new Vector2(0.5f) * _wheel.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_carBody.TextureForSprite, _car.Position, null, Color.White, _car.Rotation, _carBody.Origin, new Vector2(5f, 1.27f) * _carBody.TexelSize, SpriteEffects.FlipVertically, 0f);

            // Draw the player Avatar
            //ScreenManager.SpriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, ScreenManager.BatchEffect);
            _player.Draw(gameTime, spriteBatch, SpriteEffects.None);

            // Draw the slime
            slime.Draw(gameTime, spriteBatch, SpriteEffects.None);
            slime2.Draw(gameTime, spriteBatch, SpriteEffects.None);

            //draw gound texture
            //ScreenManager.SpriteBatch.Draw(_background.TextureTest, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(0.5f) * _background.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundForestTexture.TextureForSprite, new Vector2(-108f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundForestTexture.TextureForSprite, new Vector2(-28f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundForestTexture.TextureForSprite, new Vector2(52f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundLabTexture.TextureForSprite, new Vector2(132f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundLabTexture.TextureForSprite, new Vector2(212f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundLabTexture.TextureForSprite, new Vector2(292f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(_groundLabTexture.TextureForSprite, new Vector2(372f, -(_groundBodySize.Y)), null, Color.White, 0f, _groundTextureOrigin, new Vector2(80f, 20f) * _groundForestTexture.TexelSize, SpriteEffects.FlipVertically, 0f);

            _healthBar.Draw(spriteBatch, _player);
            _healthBarTestNewSlime.Draw(spriteBatch, _player);

            spriteBatch.End();
            #endregion


            #region LineBatch
            ScreenManager.LineBatch.Begin(Camera.Projection, Camera.View);

            // draw ground
            foreach (Fixture fixture in _ground.FixtureList)
            {
                ScreenManager.LineBatch.DrawLineShape(fixture.Shape, Color.Black);
            }
            ScreenManager.LineBatch.End();
            #endregion


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