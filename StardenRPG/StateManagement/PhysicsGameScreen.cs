/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardenRPG.StateManagement;
using StardenRPG.Utilities;
using static System.TimeZoneInfo;
using StardenRPG.Entities.Character;

namespace StardenRPG.StateManagement
{
    public class PhysicsGameScreen : GameScreen
    {
        public Camera2D Camera;
        protected DebugView DebugView;
        protected World World;
        protected Body HiddenBody;
        protected FixedMouseJoint _fixedMouseJoint;

        /*public new PlayerIndex ControllingPlayer { get; set; }*/

        public bool EnableCameraControl { get; set; }

        protected PhysicsGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            EnableCameraControl = true;
            World = null;
            Camera = null;
            DebugView = null;
        }

        public new PlayerIndex? ControllingPlayer
        {
            protected get => _controllingPlayer;
            set { _controllingPlayer = value; }
        }
        private PlayerIndex? _controllingPlayer;

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (World == null)
            {
                World = new World(Vector2.Zero);
                World.JointRemoved += JointRemoved;
            }

            // enable multithreading
            World.ContactManager.VelocityConstraintsMultithreadThreshold = 256;
            World.ContactManager.PositionConstraintsMultithreadThreshold = 256;
            World.ContactManager.CollideMultithreadThreshold = 256;

            if (DebugView == null)
            {
                DebugView = new DebugView(World);
                DebugView.RemoveFlags(DebugViewFlags.Shape);
                DebugView.RemoveFlags(DebugViewFlags.Joint);
                DebugView.DefaultShapeColor = Color.White;
                DebugView.SleepingShapeColor = Color.LightGray;
                DebugView.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.Content);
            }

            if (Camera == null)
                Camera = new Camera2D(ScreenManager.GraphicsDevice);
            else
                Camera.ResetCamera();

            HiddenBody = World.CreateBody(Vector2.Zero);

            // Loading may take a while... so prevent the game from "catching up" once we finished loading
            ScreenManager.Game.ResetElapsedTime();
        }

        protected virtual void JointRemoved(World sender, Joint joint)
        {
            if (_fixedMouseJoint == joint)
                _fixedMouseJoint = null;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!coveredByOtherScreen && !otherScreenHasFocus)
            {
                // variable time step but never less then 30 Hz
                World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            }

            Camera.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            DebugView.UpdatePerformanceGraph(World.UpdateTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex player;

            // Control debug view
            if (input.IsNewButtonPress(Buttons.Start, ControllingPlayer, out player))
            {
                EnableOrDisableFlag(DebugViewFlags.Shape);
                EnableOrDisableFlag(DebugViewFlags.DebugPanel);
                EnableOrDisableFlag(DebugViewFlags.PerformanceGraph);
                EnableOrDisableFlag(DebugViewFlags.Joint);
                EnableOrDisableFlag(DebugViewFlags.ContactPoints);
                EnableOrDisableFlag(DebugViewFlags.ContactNormals);
                EnableOrDisableFlag(DebugViewFlags.Controllers);
            }

            if (input.IsNewKeyPress(Keys.F1, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.Shape);
            if (input.IsNewKeyPress(Keys.F2, ControllingPlayer, out player))
            {
                EnableOrDisableFlag(DebugViewFlags.DebugPanel);
                EnableOrDisableFlag(DebugViewFlags.PerformanceGraph);
            }
            if (input.IsNewKeyPress(Keys.F3, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.Joint);
            if (input.IsNewKeyPress(Keys.F4, ControllingPlayer, out player))
            {
                EnableOrDisableFlag(DebugViewFlags.ContactPoints);
                EnableOrDisableFlag(DebugViewFlags.ContactNormals);
            }
            if (input.IsNewKeyPress(Keys.F5, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.PolygonPoints);
            if (input.IsNewKeyPress(Keys.F6, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.Controllers);
            if (input.IsNewKeyPress(Keys.F7, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.CenterOfMass);
            if (input.IsNewKeyPress(Keys.F8, ControllingPlayer, out player))
                EnableOrDisableFlag(DebugViewFlags.AABB);

            if (input.IsNewKeyPress(Keys.Escape, ControllingPlayer, out player))
                ExitScreen();

            if (EnableCameraControl)
                HandleCamera(input, gameTime);

            base.HandleInput(gameTime, input);
        }

        private void HandleCamera(InputState input, GameTime gameTime)
        {
            PlayerIndex player;

            Vector2 camMove = Vector2.Zero;

            if (input.IsKeyPressed(Keys.PageUp, ControllingPlayer, out player))
                Camera.Zoom += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            if (input.IsKeyPressed(Keys.PageDown, ControllingPlayer, out player))
                Camera.Zoom -= 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            if (camMove != Vector2.Zero)
                Camera.MoveCamera(camMove);
            if (input.IsNewKeyPress(Keys.Home, ControllingPlayer, out player))
                Camera.ResetCamera();
        }

        private void EnableOrDisableFlag(DebugViewFlags flag)
        {
            if ((DebugView.Flags & flag) == flag)
                DebugView.RemoveFlags(flag);
            else
                DebugView.AppendFlags(flag);
        }

        public override void Draw(GameTime gameTime)
        {
            DebugView.RenderDebugData(Camera.Projection, Camera.View);
            base.Draw(gameTime);
        }
    }
}