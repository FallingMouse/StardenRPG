using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardenRPG.Entities.Character;
using StardenRPG.SpriteManager;
using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace StardenRPG.Entities.ItemDrop
{
    public class Coin : Sprite
    {
        World world;
        
        Player _player;

        public float Amount { get; set; } = 0.0f;

        public Coin(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition) 
            : base(spriteSheet, size, origin, world, startPosition)
        {
            Body.Tag = this;
            this.world = world;

            Offset = new Vector2(1, 1);
        }

        public void setPlayer(Player _player)
        {
            this._player = _player;
        }

        public override bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            if (other.Body.Tag == _player)
            {
                //_player.playerCoin.Amount += this.Amount;
            }
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(
                texture: spriteTexture,
                destinationRectangle: new Rectangle((int)(Position.X + Offset.X), (int)(Position.Y + Offset.Y), (int)Size.X, (int)Size.Y),
                sourceRectangle: sourceRect,
                color: Tint,
                rotation: (float)Math.PI,
                origin: Vector2.Zero,
                effects: spriteEffects,
                layerDepth: 0
            );
            //base.Draw(gameTime, spriteBatch, spriteEffects);
        }
    }
}
