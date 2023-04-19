using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StardenRPG.SpriteManager
{
    public class SpriteSheetAnimationPlayer
    {
        protected bool _IsPlaying = false;

        protected SpriteSheetAnimationClip currentClip;

        private TimeSpan currentTime;

        public TimeSpan AnimationOffSet { get; set; }

        public bool IsPlaying => _IsPlaying;

        public Vector2 CurrentCell { get; set; }

        public int CurrentKeyframe { get; set; }

        public int CurrentFrameIndex { get; private set; }


        public float ClipLerpValue
        {
            get
            {
                if (currentClip != null)
                {
                    return (float)CurrentKeyframe / (float)currentClip.Keyframes.Count;
                }

                return 0f;
            }
        }

        public SpriteSheetAnimationClip CurrentClip => currentClip;

        public TimeSpan CurrentTime => currentTime;

        public Dictionary<string, SpriteSheetAnimationClip> Clips { get; set; }

        public delegate void AnimationStopped(SpriteSheetAnimationClip clip);

        public event AnimationStopped OnAnimationStopped;

        public SpriteSheetAnimationPlayer(Dictionary<string, SpriteSheetAnimationClip> clips = null, TimeSpan animationOffSet = default(TimeSpan))
        {
            AnimationOffSet = animationOffSet;
            Clips = clips;
        }

        public void StartClip(string name, int frame = 0)
        {
            StartClip(Clips[name]);
        }

        public void StartClip(SpriteSheetAnimationClip clip, int frame = 0)
        {
            if (clip != null && clip != currentClip)
            {
                currentTime = TimeSpan.Zero + AnimationOffSet;
                CurrentKeyframe = frame;
                currentClip = clip;
                _IsPlaying = true;

                // Reset the CurrentFrameIndex when starting a new clip
                CurrentFrameIndex = 0;
            }
        }

        public void StopClip()
        {
            if (currentClip != null && IsPlaying)
            {
                _IsPlaying = false;
                if (this.OnAnimationStopped != null)
                {
                    this.OnAnimationStopped(currentClip);
                }
            }
        }

        public void Update(TimeSpan time)
        {
            if (currentClip != null)
            {
                GetCurrentCell(time);
            }
        }

        public void Update(float lerp)
        {
            if (currentClip != null)
            {
                GetCurrentCell(lerp);
            }
        }

        protected void GetCurrentCell(float lerp)
        {
            CurrentKeyframe = (int)MathHelper.Lerp(0f, currentClip.Keyframes.Count - 1, lerp);
            CurrentCell = currentClip.Keyframes[CurrentKeyframe].Cell;
        }

        protected void GetCurrentCell(TimeSpan time)
        {
            time += currentTime;

            // If we reached the end, loop back to the start.
            while (time >= currentClip.Duration)
            {
                time -= currentClip.Duration;
            }

            if (time < TimeSpan.Zero || time >= currentClip.Duration)
            {
                throw new ArgumentOutOfRangeException("time");
            }

            if (time < currentTime)
            {
                if (currentClip.Looped)
                {
                    CurrentKeyframe = 0;
                }
                else
                {
                    CurrentKeyframe = currentClip.Keyframes.Count - 1;
                    StopClip();
                }
            }

            currentTime = time;

            // Read keyframe matrices.
            IList<SpriteSheetKeyFrame> keyframes = currentClip.Keyframes;

            while (CurrentKeyframe < keyframes.Count)
            {
                SpriteSheetKeyFrame spriteSheetKeyFrame = keyframes[CurrentKeyframe];

                // Stop when we've read up to the current time position.
                if (spriteSheetKeyFrame.Time > currentTime)
                {
                    break;
                }

                // Use this keyframe.
                CurrentCell = spriteSheetKeyFrame.Cell;

                // Update the CurrentFrameIndex
                CurrentFrameIndex = CurrentKeyframe;

                CurrentKeyframe++;
            }
        }
    }
}
