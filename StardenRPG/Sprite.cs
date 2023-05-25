using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardenRPG.SpriteManager;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Common;
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
        public int DrawActualWidth { get; set; }
        public int DrawActualHeight { get; set; }

        public Texture2D spriteTexture { get; set; }
        protected SpriteSheetAnimationPlayer _animationPlayer;
        protected Dictionary<string, List<Rectangle>> _actualSizes;

        /* From Sprite Aether Sample */
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Dictionary<string, List<Vector2>> OffsetActualSizes { get; set; }

        public readonly Texture2D TextureForSprite;
        public readonly Vector2 SizeVT;
        public readonly Vector2 TexelSize;
        public Vector2 Origin;

        public Sprite(Texture2D texture, Vector2 origin)
        {
            TextureForSprite = texture;
            SizeVT = new Vector2(TextureForSprite.Width, TextureForSprite.Height);
            TexelSize = Vector2.One / SizeVT;
            Origin = origin;
        }

        public Sprite(Texture2D texture)
        {
            TextureForSprite = texture;
            SizeVT = new Vector2(TextureForSprite.Width, TextureForSprite.Height);
            TexelSize = Vector2.One / SizeVT;
            Origin = SizeVT / 2f;
        }
        public static Vector2 CalculateOrigin(Body b, float pixelsPerMeter)
        {
            Vector2 lBound = new Vector2(float.MaxValue);
            Transform trans = b.GetTransform();

            foreach (Fixture fixture in b.FixtureList)
            {
                for (int j = 0; j < fixture.Shape.ChildCount; ++j)
                {
                    AABB bounds;
                    fixture.Shape.ComputeAABB(out bounds, ref trans, j);
                    Vector2.Min(ref lBound, ref bounds.LowerBound, out lBound);
                }
            }

            // calculate body offset from its center and add a 1 pixel border
            // because we generate the textures a little bigger than the actual body's fixtures
            return pixelsPerMeter * (b.Position - lBound) + new Vector2(1f);
        }
        /* End From Sprite Aether Sample */

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

            Body = World.CreateBody(Position, 0, BodyType.Dynamic);
            var fixture = Body.CreateRectangle(Size.X, Size.Y, 1f, new Vector2(-Size.X / 2, -Size.Y / 2));
            fixture.Tag = this;
            //Body = World.CreateRectangle(Size.X, Size.Y, 1, Position);
            //Body.BodyType = BodyType.Dynamic;
            Body.FixedRotation = true;
            Body.OnCollision += OnCollision;
        }

        public Sprite(Texture2D spriteSheetAsset, Point size, Point cellSize, World world, Vector2 position, Vector2 widthNheight, Vector2 _offset)
        {
            spriteTexture = spriteSheetAsset;
            Tint = Color.White;
            Size = size;
            CellSize = cellSize;
            World = world;
            Position = position;

            Body = World.CreateBody(Position, 0, BodyType.Dynamic);
            var fixture = Body.CreateRectangle(widthNheight.X, widthNheight.Y, 1f, new Vector2(-_offset.X, -_offset.Y));
            fixture.Tag = this;
            //Body = World.CreateRectangle(Size.X, Size.Y, 1, Position);
            //Body.BodyType = BodyType.Dynamic;
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
                newFixture.Tag = this;
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
