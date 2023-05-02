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
                    new Rectangle(0 + 100, 0 + 83, 60, 44),
                    new Rectangle(288 + 100, 0 + 83, 60, 44),
                    new Rectangle(576+ 100, 0 + 83, 60, 44),
                    new Rectangle(864 + 100, 0 + 83, 60, 44),
                    new Rectangle(1152 + 100, 0 + 83, 60, 44),
                    new Rectangle(1440 + 100, 0 + 84, 60, 43),
                    new Rectangle(1728 + 100, 0 + 84, 60, 43),
                    new Rectangle(2016 + 100, 0 + 84, 60, 43), } },
                { "PlayerWalkLeft", new List<Rectangle> {
                    new Rectangle(0 + 101, 128 + 83, 58, 44),
                    new Rectangle(288 + 102, 128 + 82, 62, 43),
                    new Rectangle(576+ 103, 128 + 83, 60, 44),
                    new Rectangle(864 + 102, 128 + 84, 57, 43),
                    new Rectangle(1152 + 101, 128 + 83, 58, 44),
                    new Rectangle(1440 + 101, 128 + 82, 58, 43),
                    new Rectangle(1728 + 101, 128 + 83, 58, 44),
                    new Rectangle(2016 + 101, 128 + 84, 58, 43), } },
                { "PlayerWalkRight", new List<Rectangle> {
                    new Rectangle(0 + 101, 128 + 83, 58, 44),
                    new Rectangle(288 + 102, 128 + 82, 62, 43),
                    new Rectangle(576+ 103, 128 + 83, 60, 44),
                    new Rectangle(864 + 102, 128 + 84, 57, 43),
                    new Rectangle(1152 + 101, 128 + 83, 58, 44),
                    new Rectangle(1440 + 101, 128 + 82, 58, 43),
                    new Rectangle(1728 + 101, 128 + 83, 58, 44),
                    new Rectangle(2016 + 101, 128 + 84, 58, 43), } },
                { "PlayerAttack", new List<Rectangle> {
                    new Rectangle(0 + 109, 256 + 86, 51, 41),
                    new Rectangle(288 + 111, 256 + 84, 45, 43),
                    new Rectangle(576+ 119, 256 + 79, 41, 48),
                    new Rectangle(864 + 118, 256 + 67, 48, 60),
                    new Rectangle(1152 + 118, 256 + 44, 94, 83),
                    new Rectangle(1440 + 138, 256 + 46, 73, 81),
                    new Rectangle(1728 + 138, 256 + 54, 72, 73),
                    new Rectangle(2016 + 138, 256 + 89, 70, 38),
                    new Rectangle(2304 + 127, 256 + 83, 33, 44),
                    new Rectangle(2592 + 100, 256 + 83, 60, 44),
                    new Rectangle(2880 + 130, 256 + 84, 57, 43),
                } },
                // Add more animations as needed
            };
        }

        public override void Update(GameTime gameTime)
        {
            // Update Hitbox size based on the current animation frame
            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                string currentClipName = animationPlayer.CurrentClip.Name;
                if (_frameSizes.ContainsKey(currentClipName))
                {
                    Rectangle currentFrame = _frameSizes[currentClipName][animationPlayer.CurrentFrameIndex];
                    DrawWidth = currentFrame.Width * 4;
                    DrawHeight = currentFrame.Height * 4;
                    UpdateFixtureSize(DrawWidth, DrawHeight, Offset);
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
                    Offset = Vector2.Zero;
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerWalkRight" || animationPlayer.CurrentClip.Name == "PlayerWalkLeft")
                {
                    Offset = new Vector2(0, (33 - 29) * 4); // Difference in heights between Idle and Walk animations
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerAttack")
                {
                    Offset = CurrentFacingDirection == FacingDirection.Left ? 
                        new Vector2(-(50 - 27) * 4, (33 - 39) * 4) : 
                        new Vector2(0, (33 - 39) * 4); // Difference in widths and heights between Attack and Idle/Walk animations when facing left
                }
                // Add more cases for other animations as needed
            }

            base.Draw(gameTime, spriteBatch, _spriteEffects);
        }


    }
}
