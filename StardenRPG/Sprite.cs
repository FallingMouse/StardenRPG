using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardenRPG.SpriteManager;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace StardenRPG
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Point CellSize { get; set; }
        public Point Size { get; set; }
        
        // Physics
        public Body Body { get; private set; }
        public World World { get; private set; }

        public int DrawWidth { get; set; }
        public int DrawHeight { get; set; }


        public Texture2D spriteTexture { get; set; }
        protected SpriteSheetAnimationPlayer _animationPlayer;
        protected Dictionary<string, List<Rectangle>> _frameSizes;

        public Vector2 Origin { get; set; }

        public SpriteSheetAnimationPlayer animationPlayer
        {
            get { return _animationPlayer; }
            set
            {
                if (_animationPlayer != value && _animationPlayer != null)
                    _animationPlayer.OnAnimationStopped -= OnAnimationStopped;

                _animationPlayer = value;
                _animationPlayer.OnAnimationStopped += OnAnimationStopped;
            }
        }

        public Color Tint { get; set; }

        protected Rectangle sourceRect
        {
            get
            {
                if (animationPlayer != null && _frameSizes.ContainsKey(animationPlayer.CurrentClip.Name))
                    //return new Rectangle((int)animationPlayer.CurrentCell.X, (int)animationPlayer.CurrentCell.Y, CellSize.X, CellSize.Y);
                    //return _frameSizes[animationPlayer.CurrentClip.Name][(int)animationPlayer.CurrentCell.X];
                    return _frameSizes[animationPlayer.CurrentClip.Name][animationPlayer.CurrentFrameIndex];
                    //return _frameSizes[animationPlayer.CurrentClip.Name][0];
                else
                {
                    if (CellSize == Point.Zero)
                        CellSize = new Point(spriteTexture.Width, spriteTexture.Height);

                    return new Rectangle(0, 0, CellSize.X, CellSize.Y);
                }
            }
        }

        public Sprite(Texture2D spriteSheetAsset, Point size, Point cellSize, World world, Vector2 position)
        {
            spriteTexture = spriteSheetAsset;
            Tint = Color.White;
            Size = size;
            CellSize = cellSize;
            World = world; 
            Position = position;

            DrawWidth = size.X;
            DrawHeight = size.Y;

            //Body = World.CreateBody(Position, 0, BodyType.Dynamic);
            Body = World.CreateRectangle(Size.X, Size.Y, 1, Position);
            Body.BodyType = BodyType.Dynamic;
            Body.FixedRotation = true;
            Body.OnCollision += OnCollision;
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // You can add custom collision handling logic here.
            return true;
        }

        protected virtual void OnAnimationStopped(SpriteSheetAnimationClip clip)
        {
            return;
        }

        public void SetOriginForAnimation(string animationName)
        {
            /*if (animationName == "Idle")
            {
                Origin = new Vector2(0, 0); // Adjust these values as needed
            }*/
            if (animationName == "WalkLeft")
            {
                Origin = new Vector2(0, 0); // Adjust these values as needed
            }
            // Add more cases for other animations as needed
        }

        public virtual void StartAnimation(string animation)
        {
            if (animationPlayer != null)
            {
                animationPlayer.StartClip(animation);
                SetOriginForAnimation(animation);
            }
        }

        public virtual void StopAnimation()
        {
            if (animationPlayer != null)
                animationPlayer.StopClip();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animationPlayer != null)
                animationPlayer.Update(gameTime.ElapsedGameTime);

            Position = Body.Position;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Old Code don't delete yet
            //spriteBatch.Draw(spriteTexture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), sourceRect, Tint);

            // Add these lines to determine the SpriteEffects based on the current animation.
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (animationPlayer != null && animationPlayer.CurrentClip != null && animationPlayer.CurrentClip.Name == "WalkLeft")
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            spriteBatch.Draw(
                texture: spriteTexture,
                destinationRectangle: new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), DrawWidth, DrawHeight),
                sourceRectangle: sourceRect,
                color: Tint,
                rotation: 0,
                origin: Vector2.Zero,
                effects: spriteEffects,
                layerDepth: 0
            );
        }
    }
}
