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

        public int SizeExpand { get; set; } = 1;
        public int DrawWidth { get; set; }
        public int DrawHeight { get; set; }
        public int DrawActualWidth { get; set; }
        public int DrawActualHeight { get; set; }


        public Texture2D spriteTexture { get; set; }
        protected SpriteSheetAnimationPlayer _animationPlayer;
        protected Dictionary<string, List<Rectangle>> _frameSizes;
        protected Dictionary<string, List<Rectangle>> _actualSizes;

        public Vector2 OffsetFrameSizes { get; set; } = Vector2.Zero;
        //public Vector2 OffsetActualSizes { get; set; } = Vector2.Zero;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        //public Dictionary<string, List<Vector2>> OffsetFrameSizes { get; set; }

        public Dictionary<string, List<Vector2>> OffsetActualSizes { get; set; }


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
                if (animationPlayer != null)
                //if (animationPlayer != null && _frameSizes.ContainsKey(animationPlayer.CurrentClip.Name))
                    return new Rectangle((int)animationPlayer.CurrentCell.X, (int)animationPlayer.CurrentCell.Y, CellSize.X, CellSize.Y);
                    //return _frameSizes[animationPlayer.CurrentClip.Name][animationPlayer.CurrentFrameIndex];
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

        public virtual void StartAnimation(string animation)
        {
            if (animationPlayer != null)
            {
                animationPlayer.StartClip(animation);
            }
        }

        public void UpdateFixtureSize(int width, int height, Vector2 offset = default)
        {
            if (Body != null && Body.FixtureList.Count > 0)
            {
                var oldFixture = Body.FixtureList[0];
                Body.Remove(oldFixture);
                var newFixture = Body.CreateRectangle(width, height, 1f, offset);
                newFixture.Restitution = 0.3f;
                newFixture.Friction = 0.5f;
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

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            // Old Code don't delete yet
            //spriteBatch.Draw(spriteTexture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), sourceRect, Tint);
            SpriteEffects _spriteEffects = spriteEffects;

            spriteBatch.Draw(
                texture: spriteTexture,
                destinationRectangle: new Rectangle((int)(Position.X + Offset.X), (int)(Position.Y + Offset.Y), (int)Size.X, (int)Size.Y),
                sourceRectangle: sourceRect,
                color: Tint,
                rotation: 0,
                origin: Vector2.Zero,
                effects: _spriteEffects,
                layerDepth: 0
            );
        }
    }
}
