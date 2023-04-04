using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StardenRPG.SpriteManager
{
    public class SpriteAnimationClipGenerator
    {
        private SpriteSheetKeyFrame[,] masterFrames;

        protected Vector2 SpriteSheetDimensions { get; set; }

        protected Vector2 Slices { get; set; }

        public SpriteAnimationClipGenerator(Vector2 spriteSheetDimensions, Vector2 slices)
        {
            SpriteSheetDimensions = spriteSheetDimensions;
            Slices = slices;
        }

        protected void GenerateMasterFrames()
        {
            masterFrames = new SpriteSheetKeyFrame[(int)Slices.X, (int)Slices.Y];
            for (int i = 0; (float)i < Slices.Y; i++)
            {
                for (int j = 0; (float)j < Slices.X; j++)
                {
                    masterFrames[j, i] = new SpriteSheetKeyFrame();
                }
            }
        }

        public SpriteSheetAnimationClip Generate(string name, Vector2 start, Vector2 end, TimeSpan duration, bool looped)
        {
            List<SpriteSheetKeyFrame> frame = new List<SpriteSheetKeyFrame>();
            SpriteSheetAnimationClip spriteSheetAnimationClip = new SpriteSheetAnimationClip(name, duration, null, looped);
            int xDirection = 1;
            int yDirection = 1;
            float xFrameCount = 0f;
            float yFrameCount = 0f;
            if (start.X > end.X)
            {
                xDirection = -1;
                xFrameCount = start.X - end.X + 1f;
            }
            else
            {
                xFrameCount = end.X - start.X + 1f;
            }

            if (start.Y > end.Y)
            {
                yDirection = -1;
                yFrameCount = start.Y - end.Y + 1f;
            }
            else
            {
                yFrameCount = end.Y - start.Y + 1f;
            }

            // Time of each frame
            TimeSpan timeSpan = new TimeSpan(duration.Ticks / (long)(xFrameCount * yFrameCount));
            
            // Sprite sheet dimensions divided by slices (number of frames)
            Vector2 cellSize = SpriteSheetDimensions / Slices;

            // Is this just one line off the sheet?
            // If both start and end Y are the same, then it is a horizontal
            // line of keyframes.
            int frameCount = 0; // or keyframeIndex
            if (start.Y == end.Y)
            {
                int startY = (int)start.Y;
                int startX = (int)start.X;
                while (xFrameCount > 0f)
                {
                    SpriteSheetKeyFrame item = new SpriteSheetKeyFrame(new Vector2((float)startX * cellSize.X, (float)startY * cellSize.Y), new TimeSpan(timeSpan.Ticks * frameCount++));
                    frame.Add(item);
                    startX += xDirection;
                    xFrameCount -= 1f;
                }
            }
            else if (start.X == end.X) // If both start and end X are the same, it's a vertical slice.
            {
                int fixedStartX = (int)start.X;
                int fixedStartY = (int)start.Y;
                while (yFrameCount > 0f)
                {
                    SpriteSheetKeyFrame item2 = new SpriteSheetKeyFrame(new Vector2((float)fixedStartX * cellSize.X, (float)fixedStartY * cellSize.Y), new TimeSpan(timeSpan.Ticks * frameCount++));
                    frame.Add(item2);
                    fixedStartY += yDirection;
                    yFrameCount -= 1f;
                }
            }
            else // If neither start or end X or Y are the same, then it's a block of frames.
            {
                int currentY = (int)start.Y;
                while (yFrameCount > 0f)
                {
                    float remainingXFrames = xFrameCount;
                    int currentX = (int)start.X;
                    while (remainingXFrames > 0f)
                    {
                        SpriteSheetKeyFrame item3 = new SpriteSheetKeyFrame(new Vector2((float)currentX * cellSize.X, (float)currentY * cellSize.Y), new TimeSpan(timeSpan.Ticks * frameCount++));
                        frame.Add(item3);
                        currentX += xDirection;
                        remainingXFrames -= 1f;
                    }

                    currentY += yDirection;
                    yFrameCount -= 1f;
                }
            }

            spriteSheetAnimationClip.Keyframes = frame;
            return spriteSheetAnimationClip;
        }
    }
}
