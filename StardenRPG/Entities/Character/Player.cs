using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Entities.Weapons;
using StardenRPG.Entities.RPGsystem;
using StardenRPG.Entities.ItemDrop;
using StardenRPG.Entities.Bar;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Common.TextureTools;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using Microsoft.VisualBasic;
using tainicom.Aether.Physics2D.Collision;
using StardenRPG.Entities.Monster;
using StardenRPG.Screens;

namespace StardenRPG.Entities.Character
{
    public class Player : Sprite
    {
        // Player RPG Stats
        public RPGCharacter CharacterStats { get; set; }
        public Coin playerCoin { get; set; }
        Texture2D _coinTexture;

        private bool isDead = false;

        public enum PlayerState
        {
            Idle,
            Walking,
            Attacking,
            TakingHit,
            Death
        }

        public enum FacingDirection
        {
            Left,
            Right
        }

        public PlayerState CurrentPlayerState { get; set; }
        public FacingDirection CurrentFacingDirection { get; set; } = FacingDirection.Right;
        public PlayerIndex ControllingPlayer { get; set; }

        // Weapon
        public Weapon CurrentWeapon { get; set; }
        public Body WeaponBody { get; set; }
        public Body WeaponBodyLeftSide { get; set; }
        public Body WeaponBodyRightSide { get; set; }

        //joint sword and player
        private WheelJoint _swordJoint;

        //All identifiers, contain values related to sword
        public Vector2 _swordBodyPosition;
        public Vector2 _swordLeftSide, _swordRightSide;
        public Vertices _swordLeftVertices, _swordRightVertices;
        public PolygonShape chassis;
       
        SpriteEffects _spriteEffects;
        
        // Check if the player is running
        public bool IsRunning { get; set; }

        public Player(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition, new Vector2(2, 3), new Vector2(0.3f, 0.5f))
        {
            Body.Tag = this;

            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("PlayerIdle");

            playerCoin = new Coin(_coinTexture, new Point(16 / 16, 16 / 16), new Point(16, 16), World, Position);

            // Create Character RPG Stats
            CharacterStats = new RPGCharacter("Player", 100, 20, 25, Element.Fire);
            //playerCoin = new Coin(100);

            // Create weapon for characterr
            CurrentWeapon = new Sword();

            //create weapon body left side
            WeaponBodyLeftSide = World.CreateBody(Position, 0, BodyType.Dynamic);
            WeaponBodyLeftSide.FixedRotation = true;
            WeaponBodyLeftSide.OnCollision += OnWeaponCollision; // Implement this method to handle weapon collisions
            WeaponBodyLeftSide.Enabled = false;
            WeaponBodyLeftSide.Tag = new Tag("weaponHitLeft");

            //fix position
            _swordLeftSide = new Vector2(-0.8f , -1.0f);
            
            //When player attack at the left side, sword hitbox will be created at the same side
            //Left Side
            _swordLeftVertices = CurrentWeapon.findSwordVertices(_swordLeftSide); //find vertices to create sword hitbox based on the sword body position
            chassis = new PolygonShape(_swordLeftVertices, 2);
            WeaponBodyLeftSide.CreateFixture(chassis);
            WeaponBodyLeftSide.BodyType = BodyType.Dynamic;

            //joint the sword hitbox to the player body, seem like it doesn't work but have it made the code work better (maybe)
            _swordJoint = new WheelJoint(Body, WeaponBodyLeftSide, new Vector2(WeaponBodyLeftSide.Position.X, WeaponBodyLeftSide.Position.Y), new Vector2(Body.Position.X, Body.Position.Y), true);

            //create weapon body right side
            WeaponBodyRightSide = World.CreateBody(Position, 0, BodyType.Dynamic);
            WeaponBodyRightSide.FixedRotation = true;
            WeaponBodyRightSide.OnCollision += OnWeaponCollision; // Implement this method to handle weapon collisions
            WeaponBodyRightSide.Enabled = false;
            WeaponBodyRightSide.Tag = new Tag("weaponHitRight");

            //fix position
            _swordRightSide = new Vector2(4.4f, -1.0f);
            //When player attack at the right side, sword hitbox will be created at the same side
            //Right Side
            _swordRightVertices = CurrentWeapon.findSwordVertices(_swordRightSide);
            chassis = new PolygonShape(_swordRightVertices, 2);
            WeaponBodyRightSide.CreateFixture(chassis);
            WeaponBodyRightSide.BodyType = BodyType.Dynamic;

            _swordJoint = new WheelJoint(Body, WeaponBodyRightSide, new Vector2(WeaponBodyRightSide.Position.X, WeaponBodyRightSide.Position.Y), new Vector2(_swordBodyPosition.X, _swordBodyPosition.Y), true);
            
        }

        public Element SwitchElement(Element currentElement, Element changedElement)
        {
            Element prevElement;
            //check if element, player want to change is not the same
            if (!currentElement.Equals(changedElement))
            {
                //prevElement = currentElement;
                currentElement = changedElement;
                //changedElement = currentElement;
            }

            return currentElement;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the player state based on the current animation clip
            switch (animationPlayer.CurrentClip.Name)
            {
                case "PlayerIdle":
                    CurrentPlayerState = PlayerState.Idle;
                    break;
                case "PlayerWalkLeft":
                    CurrentPlayerState = PlayerState.Walking;
                    CurrentFacingDirection = FacingDirection.Left;
                    break;
                case "PlayerWalkRight":
                    CurrentPlayerState = PlayerState.Walking;
                    CurrentFacingDirection = FacingDirection.Right;
                    break;
                case "PlayerAttack":
                    CurrentPlayerState = PlayerState.Attacking;
                    break;
                case "PlayerTakeHit":
                    CurrentPlayerState = PlayerState.TakingHit;
                    break;
                case "PlayerDeath":
                    CurrentPlayerState = PlayerState.Death;
                    break;
            }
            
            // Update the weapon hitbox size based on the current animation frame
            if (CurrentPlayerState == PlayerState.Attacking)
            {
                Rectangle currentWeaponHitbox = CurrentWeapon.GetCurrentHitbox("PlayerAttack", animationPlayer.CurrentFrameIndex);

                // Update the weapon body position and size
                WeaponBodyRightSide.Position = new Vector2(Body.Position.X, Body.Position.Y);
                WeaponBodyLeftSide.Position = new Vector2(Body.Position.X, Body.Position.Y);

                if (CurrentFacingDirection == FacingDirection.Left)
                {
                    WeaponBodyLeftSide.Enabled = true;
                }

                //When player attack at the right side, sword hitbox will be created at the same side
                if (CurrentFacingDirection == FacingDirection.Right)
                {
                    WeaponBodyRightSide.Enabled = true;
                }
            }
            else
            {
                // Disable the weapon body when not attacking
                WeaponBodyLeftSide.Enabled = false;
                WeaponBodyRightSide.Enabled = false;
            }

            if (CharacterStats.CurrentHealth <= 0 && !isDead)
            {
                CurrentPlayerState = PlayerState.Death;
                isDead = true;

                // ดำเนินการอื่น ๆ เมื่อผู้เล่นตาย
                // เช่น หยุดการเคลื่อนที่, ปิดการรับค่าอินพุต, แสดงหน้าจอ Game Over เป็นต้น
            }

        }

        public void setCoinTexture(Texture2D _coinTexture)
        {
            this._coinTexture = _coinTexture;
        }

        // This method will handle the collisions of the weapon body
        private bool OnWeaponCollision(Fixture sender, Fixture other, Contact contact)
        {
            return true;
        }

        public void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            PlayerIndex player;

            if (input.IsKeyPressed(Keys.A, ControllingPlayer, out player))
            {
                CurrentFacingDirection = FacingDirection.Left;
                if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
                {
                    animationPlayer.StartClip("PlayerWalkLeft");
                }
            }
            else if (input.IsKeyPressed(Keys.D, ControllingPlayer, out player))
            {
                CurrentFacingDirection = FacingDirection.Right;
                if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
                {
                    animationPlayer.StartClip("PlayerWalkRight");
                }
            }

            else if (CurrentPlayerState != PlayerState.Attacking
                && CurrentPlayerState != PlayerState.Death)
            {
                animationPlayer.StartClip("PlayerIdle");
            }

            else if (CurrentPlayerState == PlayerState.Death)
            {
                animationPlayer.StartClip("PlayerDeath");
            }

            if (input.IsNewKeyPress(Keys.P, ControllingPlayer, out player))
            {
                animationPlayer.StartClip("PlayerAttack");
                CurrentPlayerState = PlayerState.Attacking;
            }

            //test change element
            if(input.IsNewKeyPress(Keys.Q, ControllingPlayer,out player))
            {
                //Element newEle = Element.Water;
                Element currentEle = CharacterStats.ElementalType;

                if(currentEle.Equals(Element.Fire))
                {
                    CharacterStats.ElementalType = SwitchElement(currentEle, Element.Water);
                }
                else if (currentEle.Equals(Element.Water))
                {
                    CharacterStats.ElementalType = SwitchElement(currentEle, Element.Fire);
                }
            }

            // Test HealthBar
            if (input.IsKeyPressed(Keys.H, ControllingPlayer, out player) && input.IsKeyUp(Keys.H, ControllingPlayer, out player))
            {
                CharacterStats.CurrentHealth -= 10;
            }

            IsRunning = input.IsKeyPressed(Keys.LeftShift, ControllingPlayer, out _);

            
            Vector2 movementDirection = Vector2.Zero;

            float baseSpeed = 25.5f; // default = 250, 25.5
            float runningMultiplier = baseSpeed * 2f;
            float moveSpeed = IsRunning ? baseSpeed * runningMultiplier : baseSpeed * runningMultiplier;

            switch (animationPlayer.CurrentClip.Name)
            {
                case "PlayerWalkLeft":
                    movementDirection = new Vector2(-1, 0);
                    //Body.ApplyTorque(100);
                    break;
                case "PlayerWalkRight":
                    movementDirection = new Vector2(1, 0);
                    //Body.ApplyTorque(-100);
                    break;
            }

            //Body.LinearVelocity = movementDirection * moveSpeed;
            Body.LinearVelocity = movementDirection * moveSpeed * (float)(2 * gameTime.ElapsedGameTime.TotalSeconds);
            //Body.ApplyForce(movementDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            //player.Body.ApplyLinearImpulse(movementDirection * moveSpeed);
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
                rotation: (float)Math.PI/*0*/,
                origin: /*Vector2.Zero*/new Vector2(288 / 2, 128 - 16),
                effects: _spriteEffects,
                layerDepth: 0
            );
        }
    }
}
