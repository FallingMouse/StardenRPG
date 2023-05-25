using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace StardenRPG.Entities.Character
{
    public class Ground
    {
        private Texture2D _groundTexture;
        public Body _groundBody; // should be private but I want to test it.
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
            _groundBody = world.CreateBody(_groundPosition, 0, BodyType.Static);
            var groundFixture = _groundBody.CreateRectangle(_groundWidth, _groundHeight, 1f, Vector2.Zero);
            groundFixture.Friction = 0.5f;

            _groundBody.OnCollision += OnCollision;
            _groundBody.Tag = "Ground";
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // You can add custom collision handling logic here.
            return true;
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
