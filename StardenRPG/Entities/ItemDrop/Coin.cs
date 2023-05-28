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
        
        public bool IsKeep { get; set; } = false;

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
            if (other.Body.Tag == _player && !IsKeep)
            {
                _player.playerCoin.Amount += Amount;
                IsKeep = true;
            }
            return true;
        }

        public void Destroy()
        {
            // You can call a method to remove this slime from the game world. 
            // The implementation of this method depends on how you're managing game entities in your game world.
            if (Body != null && Body.FixtureList.Count > 0)
                Body.Remove(Body.FixtureList[0]);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsKeep)
            {
                Destroy();
            }
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
