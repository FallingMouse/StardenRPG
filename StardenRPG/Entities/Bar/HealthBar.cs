using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.Entities.Character;
using StardenRPG.Entities.Monster;

namespace StardenRPG.Entities.Bar
{
    public class HealthBar
    {
        protected Player _player;
        protected Slime _slime; 

        // Variables for the offset of the health bar relative to the player
        private float xOffset = -2.6f; 
        private float yOffset = -3f;

        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rectangle, _rectangleBG;
        private Color _colorBG /*, _color*/;

        //for slime
        private Rectangle _rectangleSlime, _rectangleBGSlime, _rectangleBoxSlime;
        private Vector2 _positionSlime, _positionBoxSlime;


        public HealthBar(GraphicsDevice graphicsDevice, Player _player, Slime _slime)
        {
            this._player = _player;
            this._slime = _slime;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _position = new Vector2(0, 0);

            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);
            _rectangleBG = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.MaxHealth, 1);

            //slime 
            _positionSlime = new Vector2(0, 0);
            _positionBoxSlime = new Vector2(0, 0);

            _rectangleSlime = new Rectangle((int)_positionSlime.X, (int)_positionSlime.Y, _slime.CharacterStats.MaxHealth, 1);
            _rectangleBGSlime = new Rectangle((int)_positionSlime.X, (int)_positionSlime.Y, _slime.CharacterStats.MaxHealth, 1);
            _rectangleBoxSlime = new Rectangle((int)_positionSlime.X, (int)_positionSlime.Y, _slime.CharacterStats.MaxHealth + 60, 1);

            //_color = Color.Red;
            _colorBG = Color.Gray;
            
        }

        public void SetHealthBox(Texture2D texture2) 
        {
            //_healthBox = texture2;
        }

        public void Update(GameTime gameTime, Player _player, Slime _slime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the health bar's position based on the player's position
            _position = new Vector2(_player.Position.X + xOffset, _player.Position.Y - yOffset);

            // Update the health bar's position based on the slime's position
            _positionSlime = new Vector2(_slime.Position.X + xOffset, _slime.Position.Y - yOffset);
            _positionBoxSlime = new Vector2(_slime.Position.X + xOffset, _slime.Position.Y - yOffset);

            _rectangle.Location = _position.ToPoint();
            _rectangleBG.Location = _position.ToPoint();

            //slime 
            _rectangleSlime.Location = _position.ToPoint();
            _rectangleBGSlime.Location = _position.ToPoint();
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Define the scale for the health bar
            Vector2 scale = new Vector2(0.9f, 0.5f);  // Scale the height by 0.5 (half)

            _rectangleBG.Width = _player.CharacterStats.MaxHealth / 18;
            _rectangle.Width = _player.CharacterStats.CurrentHealth / 18;

            //slime
            _rectangleBGSlime.Width = _slime.CharacterStats.MaxHealth / 18;
            _rectangleSlime.Width = _slime.CharacterStats.CurrentHealth / 18;

            // Calculate the gradient colors
            Color startColor = new Color(31, 22, 28);
            Color endColor = new Color(243, 63, 72);

            // Calculate the color interpolation value based on the player's health percentage
            float healthPercentage = _player.CharacterStats.CurrentHealth / (float)_player.CharacterStats.MaxHealth;
            Color interpolatedColor = Color.Lerp(startColor, endColor, healthPercentage);

            // Calculate the color interpolation value based on the slime's health percentage
            float healthSlimePercentage = _slime.CharacterStats.CurrentHealth / (float)_slime.CharacterStats.MaxHealth;
            Color interpolatedColorSlime = Color.Lerp(startColor, endColor, healthSlimePercentage);

            // Draw the actual health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangle,
                color: _colorBG,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

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

            //slime
            // Draw the actual health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _positionSlime,
                sourceRectangle: _rectangleSlime,
                color: _colorBG,
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Draw the background of the health bar
            spriteBatch.Draw(
                texture: _texture,
                position: _positionSlime,
                sourceRectangle: _rectangleBGSlime,
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
                position: _positionSlime,
                sourceRectangle: _rectangleSlime,
                color: interpolatedColorSlime, // Use the interpolated color
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }               
    }
}
