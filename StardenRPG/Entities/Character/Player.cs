using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardenRPG.SpriteManager;
using StardenRPG.StateManagement;
using StardenRPG.Entities.Weapons;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Common.TextureTools;
using StardenRPG.Entities.Bar;

namespace StardenRPG.Entities.Character
{
    public class Player : Sprite
    {
        // HealthBar
        public int Health { get; set; }
        public const int MaxHealth = 100;
        private HealthBar healthBar;

        public enum PlayerState
        {
            Idle,
            Walking,
            Attacking
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

        SpriteEffects _spriteEffects;

        // Check if the player is running
        public bool IsRunning { get; set; }

        public Player(Texture2D spriteSheet, Point size, Point origin, World world, Vector2 startPosition, Dictionary<string, SpriteSheetAnimationClip> spriteAnimationClips)
            : base(spriteSheet, size, origin, world, startPosition, new Vector2(2, 3), new Vector2(0.3f, 0.5f))
        {
            // HealthBar
            Health = MaxHealth;

            Body.Tag = "Player";

            animationPlayer = new SpriteSheetAnimationPlayer(spriteAnimationClips);
            StartAnimation("PlayerIdle");

            SizeExpand = 1; // Old is 3

            // Create the actual size of the character and the offset

            // Create weapon for characterr
            CurrentWeapon = new Sword();

            // Initialize the weapon body
            WeaponBody = World.CreateBody(Position, 0, BodyType.Dynamic);
            WeaponBody.FixedRotation = true;
            WeaponBody.OnCollision += OnWeaponCollision; // Implement this method to handle weapon collisions
            WeaponBody.Enabled = false; // Initially disable the weapon body, we'll enable it when attacking
            //WeaponBody.Enabled = true;
            //CurrentWeapon.findSwordVertices(WeaponBody);
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
            }
            
            // Update the weapon hitbox size based on the current animation frame
            if (CurrentPlayerState == PlayerState.Attacking)
            {
                // Enable the weapon body
                //WeaponBody.Enabled = true;

                Rectangle currentWeaponHitbox = CurrentWeapon.GetCurrentHitbox("PlayerAttack", animationPlayer.CurrentFrameIndex);

                //find vertices of sword in current position, but seem like it's cause delay to the game
                //CurrentWeapon.findSwordVertices(WeaponBody, new Vector2(Position.X + currentWeaponHitbox.X, Position.Y + currentWeaponHitbox.Y));
                CurrentWeapon.findSwordVertices(WeaponBody);

                // Update the weapon body position and size
                //WeaponBody.Position = new Vector2(Position.X + currentWeaponHitbox.X, Position.Y + currentWeaponHitbox.Y);

                // Assume UpdateWeaponFixtureSize works similarly to UpdateFixtureSize
                //UpdateWeaponFixtureSize(currentWeaponHitbox.Width, currentWeaponHitbox.Height);

                // Enable the weapon body
                WeaponBody.Enabled = true;

            }
            else
            {
                // Disable the weapon body when not attacking
                WeaponBody.Enabled = false;
            }
        }
        /*public void UpdateWeaponFixtureSize(float width, float height)
        {
            // Convert from pixels to world units if necessary
            float widthInWorldUnits = ConvertPixelsToWorldUnits(width);
            float heightInWorldUnits = ConvertPixelsToWorldUnits(height);

            // Remove the old fixture from the weapon body
            if (WeaponBody.FixtureList.Count > 0)
            {
                WeaponBody.DestroyFixture(WeaponBody.FixtureList[0]);
            }

            // Create and attach a new fixture with the updated size
            PolygonShape shape = new PolygonShape(1f);
            shape.SetAsBox(widthInWorldUnits / 2, heightInWorldUnits / 2);
            Fixture fixture = WeaponBody.CreateFixture(shape);
            fixture.IsSensor = true; // Set to true if you want the weapon to only detect collisions without physically reacting
        }*/

        // This method will handle the collisions of the weapon body
        private bool OnWeaponCollision(Fixture sender, Fixture other, Contact contact)
        {
            // Handle weapon collisions...

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
            else if (CurrentPlayerState != PlayerState.Attacking || animationPlayer.IsAnimationComplete("PlayerAttack"))
            {
                animationPlayer.StartClip("PlayerIdle");
            }

            if (input.IsNewKeyPress(Keys.P, ControllingPlayer, out player))
            {
                animationPlayer.StartClip("PlayerAttack");
                CurrentPlayerState = PlayerState.Attacking;
            }

            // Test HealthBar
            if (input.IsKeyPressed(Keys.H, ControllingPlayer, out player) && input.IsKeyUp(Keys.H, ControllingPlayer, out player))
            {
                Health -= 10;
            }

            IsRunning = input.IsKeyPressed(Keys.LeftShift, ControllingPlayer, out _);

            
            Vector2 movementDirection = Vector2.Zero;

            float baseSpeed = 25.5f; // default = 250, 21.5
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
                case "PlayerIdle":
                    break;
                case "PlayerAttack":
                    movementDirection = new Vector2(0, 0);
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

            //base.Draw(gameTime, spriteBatch, _spriteEffects);

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
            /*spriteBatch.Draw(spriteTexture, Position, sourceRect, Color.White, 0, 
                Vector2.Zero, new Vector2(72f, 32f) * 1, SpriteEffects.FlipVertically, 0f);*/
        }
    }
}
