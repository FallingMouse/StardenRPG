using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.Entities.Character;

namespace StardenRPG.Entities.Bar
{
    public class HealthBar
    {
        protected Player _player;

        // Variables for the offset of the health bar relative to the player
        private float xOffset = -2.6f; 
        private float yOffset = -3f;

        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rectangle, _rectangleBG;
        private Color _colorBG;


        public HealthBar(GraphicsDevice graphicsDevice, Player _player)
        {
            this._player = _player;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _position = new Vector2(0, 0);

            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);
            _rectangleBG = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);

            _colorBG = Color.Gray;
            
        }

        public void Update(GameTime gameTime, Player _player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the health bar's position based on the player's position
            _position = new Vector2(_player.Position.X + xOffset, _player.Position.Y - yOffset);

            _rectangle.Location = _position.ToPoint();
            _rectangleBG.Location = _position.ToPoint();
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Define the scale for the health bar
            Vector2 scale = new Vector2(0.9f, 0.5f);  // Scale the height by 0.5 (half)

            _rectangleBG.Width = _player.CharacterStats.MaxHealth / 18;
            _rectangle.Width = _player.CharacterStats.CurrentHealth / 18;

            // Calculate the gradient colors
            Color startColor = new Color(31, 22, 28);
            Color endColor = new Color(243, 63, 72);

            // Calculate the color interpolation value based on the player's health percentage
            float healthPercentage = _player.CharacterStats.CurrentHealth / (float)_player.CharacterStats.MaxHealth;
            Color interpolatedColor = Color.Lerp(startColor, endColor, healthPercentage);

            // Draw the background of the health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangleBG,
                color: _colorBG,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Draw the actual health bar with gradient color
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangle,
                color: interpolatedColor, // Use the interpolated color
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
    }
}
