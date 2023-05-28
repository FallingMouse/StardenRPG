using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardenRPG.Entities.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardenRPG.Entities.Bar
{
    public class AttackBar
    {
        protected Player _player;
        private float xOffset = -2.6f;
        private float yOffset = -3f;

        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rectangle, _rectangleBG;
        private Color _colorBG;

        public AttackBar(GraphicsDevice graphicsDevice, Player _player)
        {
            this._player = _player;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _position = new Vector2(0, 0);

            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.ATK, 1);
            _rectangleBG = new Rectangle((int)_position.X, (int)_position.Y, _player.CharacterStats.ATK, 1);

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
            Vector2 scale = new Vector2(0.9f, 0.5f);

            _rectangleBG.Width = _player.CharacterStats.ATK / 18;
            _rectangle.Width = _player.CharacterStats.ATK / 18;

            // Draw the actual ATK bar
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

            // Draw the background of the ATK bar
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

            // Draw the actual ATK bar with a solid color
            spriteBatch.Draw(
                texture: _texture,
                position: _position,
                sourceRectangle: _rectangle,
                color: Color.Blue, // Use a solid color for ATK bar
                rotation: 0,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
    }

}
