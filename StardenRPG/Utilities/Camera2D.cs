using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.Entities;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Utilities
{
    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _rotation;

        private Player _following;
        private Viewport _viewport;

        public Rectangle CameraBounds { get; set; }
        public Vector2 CharacterOffset { get; set; }


        public Camera2D(GraphicsDevice graphicsDevice)
        {
            _position = Vector2.Zero;
            _zoom = 1.0f;
            _rotation = 0.0f;

            _viewport = graphicsDevice.Viewport;
        }

        public void Follow(Player target)
        {
            _following = target;
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-_position, 0.0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_zoom, _zoom, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            if (_following != null)
            {
                float targetX = _following.Position.X - _viewport.Width / 2 + CharacterOffset.X;
                float targetY = _following.Position.Y - _viewport.Height / 2 + CharacterOffset.Y;

                _position.X = MathHelper.Clamp(targetX, CameraBounds.Left, CameraBounds.Right - _viewport.Width);
                _position.Y = MathHelper.Clamp(targetY, CameraBounds.Top, CameraBounds.Bottom - _viewport.Height);
            }
        }
    }

}
