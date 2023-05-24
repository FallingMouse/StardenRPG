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
using StardenRPG.StateManagement;

namespace StardenRPG.Screens
{
    class TestScreen : PhysicsGameScreen
    {
        // scaling
        private readonly Vector2 _scaleFactor;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        private float _acceleration;
        private Body _car;
        private Body _ground;

        private Body _wheelBack;
        private Body _wheelFront;
        private WheelJoint _springBack;
        private WheelJoint _springFront;

        private Sprite _carBody;
        private Sprite _wheel;

        private const float MaxSpeed = 50.0f;

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

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            EnableCameraControl = true;

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
                _car.BodyType = BodyType.Dynamic;
                _car.Position = new Vector2(0.0f, 1.0f);
                _car.CreateFixture(chassis);

                _wheelBack = World.CreateBody();
                _wheelBack.BodyType = BodyType.Dynamic;
                _wheelBack.Position = new Vector2(-1.709f, 0.78f);
                var wFixture = _wheelBack.CreateFixture(wheelShape);
                wFixture.Friction = 0.9f;

                wheelShape.Density = 1;
                _wheelFront = World.CreateBody();
                _wheelFront.BodyType = BodyType.Dynamic;
                _wheelFront.Position = new Vector2(1.54f, 0.8f);
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

            Camera.MinRotation = -0.05f;
            Camera.MaxRotation = 0.05f;

            Camera.TrackingBody = _car;
            Camera.EnableTracking = true;

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

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

                _springBack.MotorSpeed = Math.Sign(_acceleration) * MathHelper.SmoothStep(0f, MaxSpeed, Math.Abs(_acceleration));
                if (Math.Abs(_springBack.MotorSpeed) < MaxSpeed * 0.06f)
                {
                    _springBack.MotorEnabled = false;
                }
                else
                {
                    _springBack.MotorEnabled = true;
                }
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex player;

            if (input.IsKeyPressed(Keys.A, ControllingPlayer, out player))
                _acceleration = Math.Min(_acceleration + (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds), 1f);
            else if (input.IsKeyPressed(Keys.D, ControllingPlayer, out player))
                _acceleration = Math.Max(_acceleration - (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds), -1f);
            else if (input.IsKeyPressed(Keys.S, ControllingPlayer, out player))
                _acceleration = 0f;
            //_acceleration -= Math.Sign(_acceleration) * (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds);
            else
                _acceleration -= Math.Sign(_acceleration) * (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds);

            base.HandleInput(gameTime, input);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Setup Camera
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone, ScreenManager.BatchEffect);
            // draw car
            ScreenManager.SpriteBatch.Draw(_wheel.TextureTest, _wheelBack.Position, null, Color.White, _wheelBack.Rotation, _wheel.Origin, new Vector2(0.5f) * _wheel.TexelSize, SpriteEffects.FlipVertically, 0f);
            ScreenManager.SpriteBatch.Draw(_wheel.TextureTest, _wheelFront.Position, null, Color.White, _wheelFront.Rotation, _wheel.Origin, new Vector2(0.5f) * _wheel.TexelSize, SpriteEffects.FlipVertically, 0f);
            ScreenManager.SpriteBatch.Draw(_carBody.TextureTest, _car.Position, null, Color.White, _car.Rotation, _carBody.Origin, new Vector2(5f, 1.27f) * _carBody.TexelSize, SpriteEffects.FlipVertically, 0f);

            ScreenManager.SpriteBatch.End();

            ScreenManager.LineBatch.Begin(Camera.Projection, Camera.View);
            
            // draw ground
            foreach (Fixture fixture in _ground.FixtureList)
            {
                ScreenManager.LineBatch.DrawLineShape(fixture.Shape, Color.Black);
            }
            ScreenManager.LineBatch.End();

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