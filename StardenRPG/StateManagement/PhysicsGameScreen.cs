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

namespace tainicom.Aether.Physics2D.Samples.ScreenSystem
{
    public class PhysicsGameScreen : GameScreen
    {
        protected DebugView DebugView;
        protected World World;
        protected Body HiddenBody;
        protected FixedMouseJoint _fixedMouseJoint;

        public PlayerIndex ControllingPlayer { get; set; }

        protected PhysicsGameScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            World = null;
            DebugView = null;
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            if (!instancePreserved)
            { 
                if (World == null)
                {
                    World = new World(Vector2.Zero);
                    World.JointRemoved += JointRemoved;
                }
                else
                {
                    World.Clear();
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

                HiddenBody = World.CreateBody(Vector2.Zero);

                // Loading may take a while... so prevent the game from "catching up" once we finished loading
                ScreenManager.Game.ResetElapsedTime();
            }
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

            base.HandleInput(gameTime, input);
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