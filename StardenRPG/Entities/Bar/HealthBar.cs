using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.Entities.Character;

namespace StardenRPG.Entities.Bar
{
    public class HealthBar
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rectangle;
        private Rectangle _rectangleBG;
        private Color _color;
        private Color _colorBG;

        public HealthBar(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
            _position = new Vector2(40, 40);
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, Player.MaxHealth * 4, 30);
            _rectangleBG = new Rectangle((int)_position.X, (int)_position.Y, Player.MaxHealth * 4, 30);
            _color = Color.Red;
            _colorBG = Color.Gray;
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            _rectangleBG.Width = Player.MaxHealth * 4;
            _rectangle.Width = player.Health * 4;
            spriteBatch.Draw(_texture, _rectangleBG, _colorBG);
            spriteBatch.Draw(_texture, _rectangle, _color);

        }
    }
}
