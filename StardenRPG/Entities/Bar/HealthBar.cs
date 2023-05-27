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

        private Texture2D _texture, _healthBox;
        private Vector2 _position, _positionBox;
        private Rectangle _rectangle, _rectangleBG, _rectangleBox;
        private Color _color, _colorBG;


        public HealthBar(GraphicsDevice graphicsDevice, Player _player)
        {
            this._player = _player;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _position = new Vector2(0, 0);
            _positionBox = new Vector2(0, 0);

            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);
            _rectangleBG = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);
            _rectangleBox = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth + 60, 1);

            _color = Color.Red;
            _colorBG = Color.Gray;
            
        }

        public void SetHealthBox(Texture2D texture2) 
        {
            _healthBox = texture2;
        }

        public void Update(GameTime gameTime, Player _player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the health bar's position based on the player's position
            _position = new Vector2(_player.Position.X + xOffset, _player.Position.Y - yOffset);
            _positionBox = new Vector2(_player.Position.X + xOffset, _player.Position.Y - yOffset);

            _rectangle.Location = _position.ToPoint();
            _rectangleBG.Location = _position.ToPoint();
        }

        /*public void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Define the scale for the health bar
            Vector2 scale = new Vector2(0.9f, 0.5f);  // Scale the height by 0.5 (half)

            // Calculate the center of the health bar for rotation
            //Vector2 origin = new Vector2(_rectangle.Width / 2, _rectangle.Height / 2);

            // Calculate the rotation angle (in radians)
            //float rotationAngle = MathHelper.ToRadians(180);  // Rotate 180 degrees

            _rectangleBG.Width = _player.CharacterStats.MaxHealth / 18;
            _rectangle.Width = _player.CharacterStats.CurrentHealth / 18;

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

            // Draw the actual health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangle,
                color: _color,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }*/

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Define the scale for the health bar
            Vector2 scale = new Vector2(0.9f, 0.5f);  // Scale the height by 0.5 (half)

            _rectangleBG.Width = _player.CharacterStats.MaxHealth / 18;
            _rectangle.Width = _player.CharacterStats.CurrentHealth / 18;


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

            // Draw the actual health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangle,
                color: _color,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Draw the health box
            spriteBatch.Draw(
                texture: _healthBox,
                position: _positionBox,
                sourceRectangle: _rectangleBox,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
    }
}
