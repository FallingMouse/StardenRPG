using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace StardenRPG.Entities.Monster
{
    public class Slime : Sprite
    {
        private float moveSpeed;
        private float attackRange;
        private float attackCooldown;
        private float timeSinceLastAttack;
        
        private float moveRange = 600f; // You can adjust this value based on your game's requirements

        public Slime(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition)
        {
            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("SlimeIdle");

            moveSpeed = 40f;
            attackRange = 60f;
            attackCooldown = 1f;
            timeSinceLastAttack = 0f;

            DrawWidth = 64; // 16 * 4
            DrawHeight = 48; // 12 * 4

            _frameSizes = new Dictionary<string, List<Rectangle>> 
            {
                { "SlimeIdle", new List<Rectangle> {
                        new Rectangle(0 + 25, 0 + 29, 16, 12),
                        new Rectangle(0 + 25, 0 + 30, 18, 11),
                        new Rectangle(0 + 23, 0 + 31, 20, 10),
                        new Rectangle(0 + 24, 0 + 30, 18, 11), 
                } }, 
            };
        }
        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float distanceToPlayer = Vector2.Distance(Body.Position, playerPosition);

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
            else if (distanceToPlayer <= moveRange)
            {
                // Move the slime towards the player if the player is within a certain range
                Vector2 direction = Vector2.Normalize(playerPosition - Body.Position);
                Body.Position += direction * moveSpeed * deltaTime;
            }

            // Update the slime's animation
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            SpriteEffects _spriteEffects = spriteEffects;
            if (animationPlayer != null && animationPlayer.CurrentClip != null)
            {
                if (animationPlayer.CurrentClip.Name == "SlimeIdle")
                {
                    DrawWidth = 64; // 16 * 4
                    DrawHeight = 48; // 12 * 4
                    UpdateFixtureSize(DrawWidth, DrawHeight);
                }
                /*else if (animationPlayer.CurrentClip.Name == "PlayerWalkRight")
                {
                    DrawWidth = 112; // 28 * 4
                    DrawHeight = 124; // 31 * 4
                    //UpdateFixtureSize(DrawWidth, DrawHeight);
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerWalkLeft")
                {
                    _spriteEffects = SpriteEffects.FlipHorizontally;
                    DrawWidth = 112; // 28 * 4
                    DrawHeight = 124; // 31 * 4
                }
                else if (animationPlayer.CurrentClip.Name == "PlayerAttack")
                {
                    DrawWidth = 192; // 48 * 4;
                    DrawHeight = 160; // 40 * 4;
                }*/
            }

            base.Draw(gameTime, spriteBatch, _spriteEffects);
        }
    }
}