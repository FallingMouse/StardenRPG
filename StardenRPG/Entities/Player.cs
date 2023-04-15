using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
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
