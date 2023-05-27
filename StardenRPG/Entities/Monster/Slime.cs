using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.Entities.Character;
using StardenRPG.Entities.RPGsystem;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace StardenRPG.Entities.Monster
{
    public class Slime : Sprite
    {
        // Slime RPG Stats
        public RPGCharacter CharacterStats { get; set; }

        Player _player;

        private float moveSpeed;
        private float attackRange;
        private float attackCooldown;
        private float timeSinceLastAttack;
        
        private float moveRange = 10f; // You can adjust this value based on your game's requirements

        public enum FacingDirection
        {
            Left,
            Right
        }

        public FacingDirection CurrentFacingDirection { get; set; } = FacingDirection.Right;

        SpriteEffects _spriteEffects;

        public Slime(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition, new Vector2(1, 1), new Vector2(0, -0.5f))
        {
            Body.Tag = this;

            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("SlimeIdle");

            this._player = _player;

            // Create Slime RPG Stats
            CharacterStats = new RPGCharacter("Slime", 100, 10, Element.Fire);

            moveSpeed = 10f;
            attackRange = 1f;
            attackCooldown = 1f;
            timeSinceLastAttack = 0f;
        }
        public void Update(GameTime gameTime, Player _player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float distanceToPlayer = Vector2.Distance(Body.Position, _player.Position);

            // Check if the player is within the slime's attack range
            if (distanceToPlayer <= attackRange)
            {
                // Check if the slime can attack
                if (timeSinceLastAttack >= attackCooldown)
                {
                    // Trigger the attack animation and apply damage to the player

                    // Reset the attack timer
                    timeSinceLastAttack = 0f;
                }
                else
                {
                    // Increment the attack timer
                    timeSinceLastAttack += deltaTime;
                }
            }
            else if (distanceToPlayer <= moveRange && _player.CharacterStats.CurrentHealth > 0)
            {
                // Move the slime towards the player if the player is within a certain range
                Vector2 direction = Vector2.Normalize(_player.Position - Body.Position);
                Body.Position += direction * moveSpeed * deltaTime;
            }

            // Update the slime's animation
            base.Update(gameTime);
        }

        public void setPlayer(Player _player)
        {
            this._player = _player;
        }

        public override bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // Check if this Slime is colliding with a Player
            //if (other.Body.Tag.Equals("Player"))
            if (other.Body.Tag == _player)
                _player.CharacterStats.TakeDamage(50);
            /*if (other.Body.Tag is Player player)
            {
                // If so, deal damage to the player
                _player.CharacterStats.TakeDamage(10);
            }*/
            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            _spriteEffects = spriteEffects;

            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                if (CurrentFacingDirection == FacingDirection.Right)
                {
                    _spriteEffects = SpriteEffects.FlipHorizontally;
                }
            }

            spriteBatch.Draw(
                texture: spriteTexture,
                destinationRectangle: new Rectangle((int)(Position.X + Offset.X), (int)(Position.Y + Offset.Y), (int)Size.X, (int)Size.Y),
                sourceRectangle: sourceRect,
                color: Tint,
                rotation: (float)Math.PI,
                origin: /*Vector2.Zero*/new Vector2(64 / 2, 41 - 16),
                effects: _spriteEffects,
                layerDepth: 0
            );
        }
    }
}