using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities
{
    public class Ground
    {
        private Texture2D _groundTexture;
        private Body _groundBody;
        private Vector2 _groundPosition;
        private float _groundWidth, _groundHeight;

        public Ground(Texture2D groundTexture, float groundWidth, float groundHeight, Vector2 groundPosition, World world)
        {
            _groundTexture = groundTexture;
            _groundWidth = groundWidth;
            _groundHeight = groundHeight;
            _groundPosition = groundPosition;

            CreateGround(world);
        }

        private void CreateGround(World world)
        {
            _groundBody = world.CreateRectangle(_groundWidth, _groundHeight, 1, _groundPosition);
            _groundBody.BodyType = BodyType.Static;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _groundTexture,
                new Rectangle((int)_groundBody.Position.X, (int)_groundBody.Position.Y, (int)_groundWidth, (int)_groundHeight),
                Color.White
            );
        }
    }
}
