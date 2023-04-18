using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities
{
    public class StaminaBar 
    {

        private Texture2D texture;
        private Rectangle staminaBarRect, bgRect;
        private Vector2 pos;
        private Color staminaBarColor, bgColor;


        public StaminaBar(GraphicsDevice graphicsDevice) 
        {
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            pos= new Vector2(20, 50);
            staminaBarRect = new Rectangle((int)pos.X, (int)pos.Y, (int)Player.MAX_stamina , 20);
            bgRect = new Rectangle((int)pos.X, (int)pos.Y, (int)Player.MAX_stamina, 20);
            staminaBarColor = Color.Yellow;
            bgColor = Color.Gray;
        }

        public void Draw(SpriteBatch _spriteBatch, Player player) 
        {
            staminaBarRect.Width = (int)player.currentStamina;
            bgRect.Width = (int)Player.MAX_stamina;
            _spriteBatch.Draw(texture, bgRect, bgColor);
            _spriteBatch.Draw(texture, staminaBarRect, staminaBarColor);
        
        }

    }
}
