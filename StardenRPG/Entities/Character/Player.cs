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

            SizeExpand = 3;

            /*DrawWidth = 100; // 25 * 4
            DrawHeight = 176; // 44 * 4*/

            // Create _frameSizes(cellsize) and _actualSizes(actual character size)
            /*CreateFrameSizes();*/
            CreateActualCharSize();
            /*CalculateOffsetFrameSizes();*/
            CalculateOffsetActualSizes();
        }

        public override void Update(GameTime gameTime)
        {
            // Update Frames size and Hitbox size
            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                string currentClipName = animationPlayer.CurrentClip.Name;
                //if (_frameSizes.ContainsKey(currentClipName))
                if (animationPlayer != null)
                {
                    // Update Frames size based on the current animation frame
                    /*Rectangle currentFrame = _frameSizes[currentClipName][animationPlayer.CurrentFrameIndex];
                    DrawWidth = currentFrame.Width * 4;
                    DrawHeight = currentFrame.Height * 4;*/

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
        /*public void CalculateOffsetFrameSizes()
        {
            OffsetFrameSizes = new Dictionary<string, List<Vector2>>();

            foreach (var key in _frameSizes.Keys)
            {
                List<Vector2> offsets = new List<Vector2>();

                for (int i = 0; i < _frameSizes[key].Count; i++)
                {
                    float offsetX = _frameSizes[key][i].X - _actualSizes[key][i].X;
                    float offsetY = _frameSizes[key][i].Y - _actualSizes[key][i].Y;
                    offsets.Add(new Vector2(offsetX, offsetY));
                }

                OffsetFrameSizes.Add(key, offsets);
            }
        }*/

        public void CalculateOffsetActualSizes()
        {
            OffsetActualSizes = new Dictionary<string, List<Vector2>>();

            /*foreach (var key in _actualSizes.Keys)
            {
                List<Vector2> offsets = new List<Vector2>();

                for (int i = 0; i < _actualSizes[key].Count; i++)
                {
                    float offsetX = (_actualSizes[key][i].X - _frameSizes[key][i].X) * SizeExpand;
                    float offsetY = (_actualSizes[key][i].Y - _frameSizes[key][i].Y) * SizeExpand;
                    offsets.Add(new Vector2(offsetX, offsetY));
                }

                OffsetActualSizes.Add(key, offsets);
            }*/

            /*foreach (var key in _actualSizes.Keys)
            {
                List<Vector2> offsets = new List<Vector2>();
                var currentClip = animationPlayer.Clips[key];

                for (int i = 0; i < _actualSizes[key].Count; i++)
                {
                    // Calculate the source rectangle for the current frame
                    int frameX = (i % currentClip.FrameCount.X) * CellSize.X;
                    int frameY = (i / currentClip.FrameCount.X) * CellSize.Y;
                    Rectangle frameRect = new Rectangle(frameX, frameY, CellSize.X, CellSize.Y);

                    // Calculate the offset
                    float offsetX = (_actualSizes[key][i].X - frameRect.X) * SizeExpand;
                    float offsetY = (_actualSizes[key][i].Y - frameRect.Y) * SizeExpand;
                    offsets.Add(new Vector2(offsetX, offsetY));
                }

                OffsetActualSizes.Add(key, offsets);
            }*/

            foreach (var key in _actualSizes.Keys)
            {
                List<Vector2> offsets = new List<Vector2>();
                var currentClip = animationPlayer.Clips[key];

                int framesPerRow = spriteTexture.Width / CellSize.X;

                for (int i = 0; i < _actualSizes[key].Count; i++)
                {
                    // Calculate the source rectangle for the current frame
                    int frameX = (i % framesPerRow) * CellSize.X;
                    int frameY = (i / framesPerRow) * CellSize.Y;
                    Rectangle frameRect = new Rectangle(frameX, frameY, CellSize.X, CellSize.Y);

                    // Calculate the offset
                    float offsetX = (_actualSizes[key][i].X - frameRect.X) * SizeExpand;
                    float offsetY = (_actualSizes[key][i].Y - frameRect.Y) * SizeExpand;
                    offsets.Add(new Vector2(offsetX, offsetY));
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

                /* Old Code 1 */
                /*string currentClipName = animationPlayer.CurrentClip.Name;
                int currentFrameIndex = animationPlayer.CurrentFrameIndex;

                if (OffsetFrameSizes.ContainsKey(currentClipName))
                {
                    Offset = OffsetFrameSizes[currentClipName][currentFrameIndex];
                }
                else
                {
                    Offset = Vector2.Zero;
                }*/

                /* Old Code 2 */
                /*if (animationPlayer.CurrentClip.Name == "PlayerIdle")
                {
                    OffsetFrameSizes = Vector2.Zero;
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerWalkRight" || animationPlayer.CurrentClip.Name == "PlayerWalkLeft")
                {
                    OffsetFrameSizes = new Vector2(0, (33 - 29) * SizeExpand); // Difference in heights between Idle and Walk animations
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerAttack")
                {
                    OffsetFrameSizes = CurrentFacingDirection == FacingDirection.Left ?
                        new Vector2(-(50 - 27) * SizeExpand, (33 - 39) * SizeExpand) :
                        new Vector2(0, (33 - 39) * SizeExpand); // Difference in widths and heights between Attack and Idle/Walk animations when facing left
                }*/
                // Add more cases for other animations as needed
            }

            base.Draw(gameTime, spriteBatch, _spriteEffects);
        }

        /*public void CreateFrameSizes()
        {
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
        }*/

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
                    new Rectangle(2880 + 131, 256 + 84, 24, 43),
                } },
                // Add more animations as needed
            };
        }
    }
}
