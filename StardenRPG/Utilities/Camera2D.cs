using Microsoft.Xna.Framework;

namespace StardenRPG.Utilities
{
    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _rotation;

        public Camera2D()
        {
            _position = Vector2.Zero;
            _zoom = 1.0f;
            _rotation = 0.0f;
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
    }

}
