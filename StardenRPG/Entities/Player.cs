using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities
{
    public class Player : Sprite
    {
        public PlayerIndex ControllingPlayer { get; set; }

        // Check if the player is running
        public bool IsRunning { get; set; }

        public Player(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition)
        {
            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("Idle");

            _frameSizes = new Dictionary<string, List<Rectangle>>
            {
                { "Idle", new List<Rectangle> { 
                    new Rectangle(0 + 18, 0, 18, 33),
                    new Rectangle(69 + 18, 0, 18, 33),
                    new Rectangle(138 + 17, 0, 20, 32),
                    new Rectangle(207 + 17, 0, 20, 31),
                    new Rectangle(276 + 18, 0, 18, 31),
                    new Rectangle(345 + 18, 0, 18, 32), } },
                { "WalkLeft", new List<Rectangle> { 
                    new Rectangle(0 + 15, 44, 22, 29),
                    new Rectangle(69 + 15, 44, 22, 26),
                    new Rectangle(138 + 15, 44, 24, 26),
                    new Rectangle(207 + 15, 44, 22, 27),
                    new Rectangle(276 + 14, 44, 25, 28),
                    new Rectangle(345 + 15, 44, 26, 32),
                    new Rectangle(18 + 18, 88, 22, 28),
                    new Rectangle(69 + 15, 88, 22, 28), } },
                { "WalkRight", new List<Rectangle> {
                    new Rectangle(0 + 15, 44, 22, 28),
                    new Rectangle(69 + 15, 44, 22, 28),
                    new Rectangle(138 + 15, 44, 26, 32),
                    new Rectangle(207 + 15, 44, 25, 28),
                    new Rectangle(276 + 14, 44, 22, 27),
                    new Rectangle(345 + 15, 44, 24, 26),
                    new Rectangle(18 + 18, 88, 22, 26),
                    new Rectangle(69 + 15, 88, 22, 29), } },
                // Add more animations as needed
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            PlayerIndex player;

            if (input.IsKeyPressed(Keys.Left, ControllingPlayer, out player) || input.IsKeyPressed(Keys.A, ControllingPlayer, out player))
                animationPlayer.StartClip("WalkLeft");
            else if (input.IsKeyPressed(Keys.Right, ControllingPlayer, out player) || input.IsKeyPressed(Keys.D, ControllingPlayer, out player))
                animationPlayer.StartClip("WalkRight");
            else
                animationPlayer.StartClip("Idle");

            //IsRunning = input.CurrentKeyboardStates[(int)ControllingPlayer].IsKeyDown(Keys.LeftShift);
            IsRunning = input.IsKeyPressed(Keys.LeftShift, ControllingPlayer, out _);

        }

    }
}
