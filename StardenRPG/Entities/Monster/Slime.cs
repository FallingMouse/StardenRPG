using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.Entities.Character;
using StardenRPG.Entities.RPGsystem;
using StardenRPG.Entities.ItemDrop;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using StardenRPG.Screens;

namespace StardenRPG.Entities.Monster
{
    public class Slime : Sprite
    {
        private TestScreen _testScreen;

        World world;

        // Slime RPG Stats
        public RPGCharacter CharacterStats { get; set; }
        public bool IsDeath { get; set; }

        Player _player;

        Coin droppedCoin;
        Texture2D _coinTexture;

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
            this.world = world;
            IsDeath = false;

            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("SlimeIdle");

            this._player = _player;

            //this._coinTexture = _coinTexture;

            // Create Slime RPG Stats
            CharacterStats = new RPGCharacter("Slime", 100, 10, 10, Element.Plant);

            moveSpeed = 10f;
            attackRange = 1f;
            attackCooldown = 1f;
            timeSinceLastAttack = 0f;
        }
        public void Update(GameTime gameTime, Player _player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float distanceToPlayer = Vector2.Distance(Body.Position, _player.Position);

            // Check if the slime's HP is zero or less
            if (CharacterStats.CurrentHealth <= 0 && !IsDeath)
            {
                // Drop money
                DropMoney();

                // Destroy this Slime instance
                IsDeath = true;
                Destroy();
            }

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

        public void setGameplayScreen(TestScreen _testScreen)
        {
            this._testScreen = _testScreen;
        }

        public void setCoinTexture(Texture2D _coinTexture)
        {
            this._coinTexture = _coinTexture;
        }

        public override bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // Check if this Slime is colliding with a Player
            //if (other.Body.Tag.Equals("Player"))
            if (other.Body.Tag == _player)
            {
                _player.CharacterStats.TakeDamage(CharacterStats.ATK);
                //CharacterStats.TakeDamage(50);
                //Body.Position -= new Vector2(1);
            }
            //if(other.Body.Tag.Equals(_player.WeaponBodyLeftSide.Tag) || other.Body.Tag.Equals(_player.WeaponBodyRightSide.Tag))
            if (other.Body.Tag == _player.WeaponBodyLeftSide.Tag)
            {
                if (_player.WeaponBodyLeftSide.Enabled == true)
                {
                    //check element
                    if(_player.CharacterStats.ElementalType == Element.Fire)
                    {
                        CharacterStats.TakeDamage(_player.CharacterStats.ATK * 3);
                    }
                    else if(_player.CharacterStats.ElementalType == CharacterStats.ElementalType)
                    {
                        CharacterStats.TakeDamage(0); //if player and monster have same element, it's not make damage (triggered)
                    }
                    else
                    {
                        CharacterStats.TakeDamage(_player.CharacterStats.ATK);
                    }
                    
                }
            }
            if (other.Body.Tag == _player.WeaponBodyRightSide.Tag)
            {
                //waiting for elemant check
                if (_player.WeaponBodyRightSide.Enabled == true)
                {
                    //check element
                    if (_player.CharacterStats.ElementalType == Element.Water)
                    {
                        CharacterStats.TakeDamage(_player.CharacterStats.ATK * 2);
                    }
                    else if (_player.CharacterStats.ElementalType == CharacterStats.ElementalType)
                    {
                        CharacterStats.TakeDamage(0); //if player and monster have same element, it's not make damage (triggered)
                    }
                    else
                    {
                        CharacterStats.TakeDamage(_player.CharacterStats.ATK);
                    }
                }
            }


            /*if (other.Body.Tag is Player player)
            {
                // If so, deal damage to the player
                _player.CharacterStats.TakeDamage(10);
            }*/
            return true;
        }

        public void DropMoney()
        {
            int coinToDrop = 10;

            droppedCoin = new Coin(_coinTexture, new Point(16 / 16, 16 / 16), new Point(16, 16), World, Position);
            droppedCoin.Amount = coinToDrop;

            _testScreen.Coins.Add(droppedCoin);
        }

        public void Destroy()
        {
            // You can call a method to remove this slime from the game world. 
            // The implementation of this method depends on how you're managing game entities in your game world.
            if (Body != null && Body.FixtureList.Count > 0)
                Body.Remove(Body.FixtureList[0]);
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