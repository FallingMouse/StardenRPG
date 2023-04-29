using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities.Character
{
    public class Player : Sprite
    {
        public enum PlayerState
        {
            Idle,
            Walking,
            Attacking
        }

        public enum FacingDirection
        {
            Left,
            Right
        }

        public PlayerState CurrentPlayerState { get; set; }
        public FacingDirection CurrentFacingDirection { get; set; } = FacingDirection.Right;
        public PlayerIndex ControllingPlayer { get; set; }

        SpriteEffects _spriteEffects = SpriteEffects.None;

        // Check if the player is running
        public bool IsRunning { get; set; }

        public Player(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition)
        {
            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("PlayerIdle");

            DrawWidth = 72; // 18 * 4
            DrawHeight = 132; // 33 * 4

            _frameSizes = new Dictionary<string, List<Rectangle>>
            {
                { "PlayerIdle", new List<Rectangle> { 
                    new Rectangle(0 + 18, 0 + 10, 18, 33),
                    new Rectangle(69 + 18, 0 + 10, 18, 33),
                    new Rectangle(138 + 17, 0 + 11, 20, 32),
                    new Rectangle(207 + 17, 0 + 12, 20, 31),
                    new Rectangle(276 + 18, 0 + 12, 18, 31),
                    new Rectangle(345 + 18, 0 + 11, 18, 32), } },
                { "PlayerWalkLeft", new List<Rectangle> {
                    new Rectangle(0 + 12, 44 + 14, 25, 29),
                    new Rectangle(69 + 13, 44 + 14, 24, 26),
                    new Rectangle(138 + 12, 44 + 15, 29, 27),
                    new Rectangle(207 + 11, 44 + 16, 26, 27),
                    new Rectangle(276 + 12, 44 + 15, 29, 28),
                    new Rectangle(345 + 13, 44 + 12, 28, 31),
                    new Rectangle(414 + 11, 44 + 13, 29, 28),
                    new Rectangle(483 + 13, 44 + 15, 24, 28), } },
                { "PlayerWalkRight", new List<Rectangle> {
                    new Rectangle(0 + 12, 44 + 14, 25, 29),
                    new Rectangle(69 + 13, 44 + 14, 24, 26),
                    new Rectangle(138 + 12, 44 + 15, 29, 27),
                    new Rectangle(207 + 11, 44 + 16, 26, 27),
                    new Rectangle(276 + 12, 44 + 15, 29, 28),
                    new Rectangle(345 + 13, 44 + 12, 28, 31),
                    new Rectangle(414 + 11, 44 + 13, 29, 28),
                    new Rectangle(483 + 13, 44 + 15, 24, 28), } },
                { "PlayerAttack", new List<Rectangle> {
                    new Rectangle(0 + 13, 88 + 3, 48, 40),
                    new Rectangle(69 + 14, 88 + 4, 50, 39),
                } },
                // Add more animations as needed
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                string currentClipName = animationPlayer.CurrentClip.Name;
                if (_frameSizes.ContainsKey(currentClipName))
                {
                    Rectangle currentFrame = _frameSizes[currentClipName][animationPlayer.CurrentFrameIndex];
                    DrawWidth = currentFrame.Width * 4;
                    DrawHeight = currentFrame.Height * 4;
                    UpdateFixtureSize(DrawWidth, DrawHeight);
                }
            }

            base.Update(gameTime);

            // Update the player state based on the current animation clip
            switch (animationPlayer.CurrentClip.Name)
            {
                case "PlayerIdle":
                    CurrentPlayerState = PlayerState.Idle;
                    break;
                case "PlayerWalkLeft":
                    CurrentPlayerState = PlayerState.Walking;
                    CurrentFacingDirection = FacingDirection.Left;
                    break;
                case "PlayerWalkRight":
                    CurrentPlayerState = PlayerState.Walking;
                    CurrentFacingDirection = FacingDirection.Right;
                    break;
                case "PlayerAttack":
                    CurrentPlayerState = PlayerState.Attacking;
                    break;
            }
        }

        public void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            PlayerIndex player;

            if (input.IsKeyPressed(Keys.Left, ControllingPlayer, out player) || input.IsKeyPressed(Keys.A, ControllingPlayer, out player))
            {
                CurrentFacingDirection = FacingDirection.Left;
                if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
                {
                    animationPlayer.StartClip("PlayerWalkLeft");
                }
            }
            else if (input.IsKeyPressed(Keys.Right, ControllingPlayer, out player) || input.IsKeyPressed(Keys.D, ControllingPlayer, out player))
            {
                CurrentFacingDirection = FacingDirection.Right;
                if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
                {
                    animationPlayer.StartClip("PlayerWalkRight");
                }
            }
            else if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
            {
                animationPlayer.StartClip("PlayerIdle");
            }

            if (input.IsNewKeyPress(Keys.P, ControllingPlayer, out player))
            {
                animationPlayer.StartClip("PlayerAttack");
                CurrentPlayerState = PlayerState.Attacking;
            }

            IsRunning = input.IsKeyPressed(Keys.LeftShift, ControllingPlayer, out _);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            _spriteEffects = spriteEffects;

            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                if (CurrentFacingDirection == FacingDirection.Left)
                {
                    _spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    _spriteEffects = SpriteEffects.None;
                }

                if (animationPlayer.CurrentClip.Name == "PlayerIdle")
                {
                    _yOffset = Vector2.Zero;
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerWalkRight" || animationPlayer.CurrentClip.Name == "PlayerWalkLeft")
                {
                    _yOffset = new Vector2(0, (33 - 29) * 4); // Difference in heights between Idle and Walk animations
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerAttack")
                {
                    _yOffset = new Vector2(0, (33 - 39) * 4); // Difference in heights between Idle and Attack animations
                }
                // Add more cases for other animations as needed
            }

            base.Draw(gameTime, spriteBatch, _spriteEffects);
        }


    }
}
