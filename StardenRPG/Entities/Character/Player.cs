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
            Body.Tag = "Player";

            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("PlayerIdle");

            SizeExpand = 3;

            // Create the actual size of the character and the offset
            CreateActualCharSize();
            CalculateOffsetActualSizes();
        }

        public override void Update(GameTime gameTime)
        {
            // Update Frames size and Hitbox size
            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                string currentClipName = animationPlayer.CurrentClip.Name;
                if (animationPlayer != null)
                {
                    // Update Hitbox size based on the current animation frame
                    Rectangle currentActualFrame = _actualSizes[currentClipName][animationPlayer.CurrentFrameIndex];
                    DrawActualWidth = currentActualFrame.Width * SizeExpand;
                    DrawActualHeight = currentActualFrame.Height * SizeExpand;
                    UpdateFixtureSize(DrawActualWidth, DrawActualHeight, OffsetActualSizes[currentClipName][animationPlayer.CurrentFrameIndex]);
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

        public void CalculateOffsetActualSizes()
        {
            OffsetActualSizes = new Dictionary<string, List<Vector2>>();
            int j = 0;

            foreach (var key in _actualSizes.Keys)
            {
                List<Vector2> offsets = new List<Vector2>();
                
                if (key == "PlayerWalkRight") j -= 1;

                for (int i = 0; i < _actualSizes[key].Count; i++)
                {
                    // Calculate the source rectangle for the current frame
                    int frameX = i * CellSize.X;
                    int frameY = j * CellSize.Y;
                    Rectangle frameRect = new Rectangle(frameX, frameY, CellSize.X, CellSize.Y);

                    // Calculate the offset
                    float offsetX = (_actualSizes[key][i].X - frameRect.X) * SizeExpand;
                    float offsetY = (_actualSizes[key][i].Y - frameRect.Y) * SizeExpand;
                    offsets.Add(new Vector2(offsetX, offsetY));
                    
                    if (i == _actualSizes[key].Count - 1) j++;
                }

                OffsetActualSizes.Add(key, offsets);
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
            }

            base.Draw(gameTime, spriteBatch, _spriteEffects);
        }

        public void CreateActualCharSize()
        {
            _actualSizes = new Dictionary<string, List<Rectangle>>
            {
                { "PlayerIdle", new List<Rectangle> {
                    new Rectangle(0 + 133, 0 + 83, 25, 44),
                    new Rectangle(288 + 133, 0 + 83, 25, 44),
                    new Rectangle(576+ 133, 0 + 83, 25, 44),
                    new Rectangle(864 + 133, 0 + 83, 25, 44),
                    new Rectangle(1152 + 133, 0 + 83, 25, 44),
                    new Rectangle(1440 + 133, 0 + 84, 25, 43),
                    new Rectangle(1728 + 133, 0 + 84, 25, 43),
                    new Rectangle(2016 + 133, 0 + 84, 25, 43), } },
                { "PlayerWalkLeft", new List<Rectangle> {
                    new Rectangle(0 + 132, 128 + 83, 27, 44),
                    new Rectangle(288 + 127, 128 + 82, 32, 43),
                    new Rectangle(576+ 131, 128 + 83, 28, 44),
                    new Rectangle(864 + 136, 128 + 84, 21, 43),
                    new Rectangle(1152 + 136, 128 + 83, 21, 44),
                    new Rectangle(1440 + 134, 128 + 82, 22, 43),
                    new Rectangle(1728 + 135, 128 + 83, 21, 44),
                    new Rectangle(2016 + 136, 128 + 84, 20, 43), } },
                { "PlayerWalkRight", new List<Rectangle> {
                    new Rectangle(0 + 132, 128 + 83, 27, 44),
                    new Rectangle(288 + 127, 128 + 82, 32, 43),
                    new Rectangle(576+ 131, 128 + 83, 28, 44),
                    new Rectangle(864 + 136, 128 + 84, 21, 43),
                    new Rectangle(1152 + 136, 128 + 83, 21, 44),
                    new Rectangle(1440 + 134, 128 + 82, 22, 43),
                    new Rectangle(1728 + 135, 128 + 83, 21, 44),
                    new Rectangle(2016 + 136, 128 + 84, 20, 43), } },
                { "PlayerAttack", new List<Rectangle> {
                    new Rectangle(0 + 131, 256 + 86, 26, 41),
                    new Rectangle(288 + 125, 256 + 84, 26, 43),
                    new Rectangle(576+ 135, 256 + 79, 23, 48),
                    new Rectangle(864 + 140, 256 + 79, 23, 48),
                    new Rectangle(1152 + 143, 256 + 88, 23, 39),
                    new Rectangle(1440 + 141, 256 + 89, 26, 38),
                    new Rectangle(1728 + 141, 256 + 89, 25, 38),
                    new Rectangle(2016 + 142, 256 + 89, 26, 38),
                    new Rectangle(2304 + 135, 256 + 83, 21, 44),
                    new Rectangle(2592 + 135, 256 + 83, 21, 44),
                } },
                // Add more animations as needed
            };
        }
    }
}
