﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Gaia.Input;
using Gaia.Rendering;
using Gaia.Rendering.RenderViews;
using Gaia.Physics;
using Gaia.Core;

namespace Gaia.SceneGraph.GameEntities
{
    public class Actor : Entity
    {
        protected Vector3 initialPos;

        protected State physicsState;

        protected float forwardAcceleration = 40;
        protected float backwardAcceleration = 8;
        protected float strafeAcceleration = 12;

        protected float speedBonus = 1.0f;
        protected float MAX_SPEED_DURATION = 20;
        protected float speedTime;

        protected static float MAX_HEALTH = 100;

        protected float health = MAX_HEALTH;
        
        protected Vector3 rotation = Vector3.Zero;

        protected ParticleEmitter emitter;
        protected Light emitterLight;

        protected BoundingBox bounds;

        public BoundingBox GetBounds()
        {
            return bounds;
        }

        public int GetTeam()
        {
            return team;
        }

        public bool IsDead()
        {
            return (health <= 0.0f);
        }

        public float GetHealth()
        {
            return health;
        }

        protected int team;

        protected Projectile projectile = null;

        protected static float ATTACK_DELAY_TIME = 0.85f;
        protected float delayTime = 0;
        protected static float MAX_PROJECTILE_TIME = 4.0f;
        protected float explosionMagnitude = 0;

        protected static float RESPAWN_TIME_AI = 15;
        protected static float RESPAWN_TIME_PLAYER = 6;
        protected float respawnTime = -1;

        protected float colorTime = 0;
        protected float maxColorTime = HIT_COLOR_TIME;

        protected Vector3 blendColor;
        protected static Vector3 HIT_COLOR = new Vector3(1.0f, 0.3f, 0.0f);
        protected static Vector3 HEALTH_COLOR = new Vector3(1.0f, 0.1f, 0.95f);

        protected static float HIT_COLOR_TIME = 0.7f;
        protected static float PLAYER_SIZE = 5;
        protected static int DEFAULT_PLAYER_LIVES = 3;
        protected static int DEFAULT_AI_LIVES = 8;
        protected int lives = DEFAULT_AI_LIVES;

        protected virtual void ResetStates()
        {
            respawnTime = -1;
            health = MAX_HEALTH;
            this.Transformation.SetPosition(Vector3.Transform(initialPos, scene.MainTerrain.Transformation.GetTransform()));
            physicsState.position = this.Transformation.GetPosition();
            physicsState.velocity = Vector3.Zero;
            emitter.OnDestroy();
            emitter.OnAdd(this.scene);
            emitter.EmitOnce = false;
            emitter.SetColor(GetTeamColor());
            emitterLight.Color = GetTeamColor();
        }

        public virtual void ApplyDamage(Projectile projectile, Vector3 impulseVector)
        {
            physicsState.velocity += impulseVector;
            health -= projectile.GetDamage();

            if (health <= 0.0)
            {
                OnDeath();
            }

            colorTime = HIT_COLOR_TIME;
            blendColor = HIT_COLOR;
        }

        public virtual void ApplyHealth(float amount)
        {
            if (IsDead())
                return;
            health = Math.Min(health + amount, MAX_HEALTH);

            colorTime = HIT_COLOR_TIME;
            blendColor = HEALTH_COLOR;
        }

        public Vector3 GetTeamColor()
        {
            switch (team)
            {
                case 0: //Blue team
                    return new Vector3(0.13f, 0.86f, 1.26f);
                case 1: //Red team
                    return new Vector3(1.13f, 0.46f, 0.0f);
                case 2: //Green team
                    return new Vector3(0.2f, 1.0f, 0.2f);
                default:
                    return Vector3.One;
            }
        }

        protected void CreateProjectile()
        {
            projectile = new Projectile(this, "TracerParticle", "ExplosionParticle");
            projectile.Transformation.SetPosition(physicsState.position);
            projectile.Transformation.SetRotation(rotation);
            this.scene.Entities.Add(projectile);
            projectile.OnAdd(this.scene);
        }

        protected virtual void OnDeath()
        {
            physicsState.velocity = Vector3.Zero;
            emitter.EmitOnce = true;
            
            emitterLight.Color = Vector3.Zero;
            lives--;
            if (lives >= 0)
            {
                respawnTime = RESPAWN_TIME_AI;
            }
        }

        protected void FireGun(Vector3 forwardVector)
        {
            if (delayTime > 0.0)
                return;

            delayTime = ATTACK_DELAY_TIME + ATTACK_DELAY_TIME * (explosionMagnitude - 1) / Projectile.EXPLOSION_MAX_MAGNITUDE;

            if (projectile == null)
                CreateProjectile();
            projectile.SetVelocity(forwardVector);

            projectile.SetMagnitude(explosionMagnitude);

            projectile = null;
        }

        public Actor(Vector3 initPos)
        {
            initialPos = initPos;
        }

        public override void OnAdd(Scene scene)
        {
            emitter = new ParticleEmitter(Resources.ResourceManager.Inst.GetParticleEffect("PlayerParticles"), 60);
            emitter.SetColor(GetTeamColor());
            emitterLight = new Light(LightType.Point, GetTeamColor(), this.Transformation.GetPosition(), false);
            emitterLight.Parameters = new Vector4(55, 50, 0, 0);
            scene.Entities.Add(emitter);
            scene.Entities.Add(emitterLight);
            scene.Actors.Add(this);

            base.OnAdd(scene);
        }

        public override void OnDestroy()
        {
            scene.Actors.Remove(this);
            scene.Entities.Remove(emitter);
            scene.Entities.Remove(emitterLight);
            emitter.OnDestroy();
            emitterLight.OnDestroy();
            base.OnDestroy();
        }

        public override void OnUpdate()
        {
            
            emitter.Transformation.SetPosition(physicsState.position);
            emitter.Transformation.SetRotation(rotation);
            emitterLight.Transformation.SetPosition(physicsState.position);

            this.Transformation.SetPosition(physicsState.position);
            this.Transformation.SetRotation(rotation);

            bounds.Min = Vector3.Transform(Vector3.One * -PLAYER_SIZE, emitter.Transformation.GetTransform());
            bounds.Max = Vector3.Transform(Vector3.One * PLAYER_SIZE, emitter.Transformation.GetTransform());

            if (delayTime > 0)
            {
                delayTime -= Time.GameTime.ElapsedTime;
            }

            if (speedTime > 0)
            {
                speedTime -= Time.GameTime.ElapsedTime;
                if (speedTime <= 0.0f)
                {
                    speedBonus = 1.0f;
                }
            }

            if (respawnTime > 0)
            {
                respawnTime -= Time.GameTime.ElapsedTime;
                if (respawnTime <= 0.0f)
                {
                    ResetStates();
                }
            }

            if (colorTime > 0.0)
            {
                colorTime -= Time.GameTime.ElapsedTime;
                Vector3 color = Vector3.Lerp(GetTeamColor(), blendColor, colorTime / maxColorTime);
                emitter.SetColor(color);
                emitterLight.Color = color;

                if(colorTime <= 0.0f)
                {
                    emitter.SetColor(GetTeamColor());
                    emitterLight.Color = GetTeamColor();
                }
            }

            if (projectile != null)
            {
                Matrix transform = this.Transformation.GetTransform();
                Vector3 projPos = this.Transformation.GetPosition() + transform.Forward * (8f + explosionMagnitude) - Vector3.Up * 0.15f;
                projectile.SetMagnitude(explosionMagnitude); 
                projectile.Transformation.SetPosition(projPos);
            }

            base.OnUpdate();
        }
    }

    public class Player : Actor
    {
        float hoverMagnitude = 2.5f;
        float hoverAngle = 0;

        MainRenderView renderView;

        float aspectRatio;
        float fieldOfView;

        int numGemsCollected = 0;

        // used to create white flash after being hit
        int MAX_FLASH_TIMER = 8;
        int hitFlashTimer = 0;
        int deathFadeTimer = 0;

        Vector3 cameraPosition = Vector3.Zero;

        public Player(Vector3 spawnPos)
            : base(spawnPos)
        {}

        protected override void ResetStates()
        {
            hitFlashTimer = 0;
            deathFadeTimer = 0;
            base.ResetStates();
        }

        public void OnGemCollected(float speedupPercent)
        {
            speedTime = MAX_SPEED_DURATION;
            speedBonus = speedupPercent;
            numGemsCollected++;
        }

        protected override void OnDeath()
        {
            deathFadeTimer = 1;

            base.OnDeath();
            if (lives >= 0)
            {
                respawnTime = RESPAWN_TIME_PLAYER;
                //Add your death-related code here!
            }
        }

        public override void OnAdd(Scene scene)
        {
            base.OnAdd(scene);

            this.team = 0;
            this.lives = DEFAULT_PLAYER_LIVES;
            renderView = new MainRenderView(scene, Matrix.Identity, Matrix.Identity, Vector3.Zero, 1.0f, 1000);

            ResetStates();
            scene.MainCamera = renderView;
            scene.AddRenderView(renderView);

            fieldOfView = MathHelper.ToRadians(70);
            aspectRatio = GFX.Inst.DisplayRes.X / GFX.Inst.DisplayRes.Y;

            ResetStates();
        }

        public override void OnDestroy()
        {
            scene.RemoveRenderView(renderView);
            base.OnDestroy();
        }

        public override void ApplyDamage(Projectile projectile, Vector3 impulseVector)
        {
            hitFlashTimer = 1;
            base.ApplyDamage(projectile, impulseVector);
        }

        public override void OnRender(RenderView view)
        {
            if (view.GetRenderType() == RenderViewType.MAIN)
            {
                DrawLives();
                DrawHealthBar();
                DrawGemProgressBar();

                if (hitFlashTimer >= MAX_FLASH_TIMER) hitFlashTimer = 0;
                else if (hitFlashTimer > 0)
                {
                    hitFlashTimer++;
                    DrawHitFlash();
                }
                if (deathFadeTimer > 0)
                {
                    deathFadeTimer++;
                    DrawDeathFade();
                }
                if (lives < 0)
                {
                    Vector2 max = new Vector2(1, 1);
                    Vector2 min = new Vector2(-1, -1);
                    Gaia.Resources.TextureResource image = Resources.ResourceManager.Inst.GetTexture("Textures/Details/GameOver.png");
                    GUIElement element = new GUIElement(min, max, image);
                    GFX.Inst.GetGUI().AddElement(element);
                }
                if (numGemsCollected >= scene.NUM_GEMS_Required)
                {
                    Vector2 max = new Vector2(1, 1);
                    Vector2 min = new Vector2(-1, -1);
                    Gaia.Resources.TextureResource image = Resources.ResourceManager.Inst.GetTexture("Textures/Details/Congrats.png");
                    GUIElement element = new GUIElement(min, max, image);
                    GFX.Inst.GetGUI().AddElement(element);
                }
            }
            base.OnRender(view);
        }

        void HandleControls()
        {
            Vector2 centerCrd = GFX.Inst.DisplayRes / 2.0f;
            Vector2 delta = InputManager.Inst.GetMouseDisplacement();
            //delta.Y *= -1;
            rotation.Y += delta.X;
            rotation.X = MathHelper.Clamp(rotation.X + delta.Y, -1.4f, 1.4f);
            if (rotation.Y > MathHelper.TwoPi)
                rotation.Y -= MathHelper.TwoPi;
            if (rotation.Y < 0)
                rotation.Y += MathHelper.TwoPi;
            Mouse.SetPosition((int)(centerCrd.X + GFX.Inst.Origin.X), (int)(centerCrd.Y + GFX.Inst.Origin.Y));

            Matrix transform = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);

            hoverAngle += Time.GameTime.ElapsedTime;
            if (hoverAngle >= MathHelper.TwoPi)
                hoverAngle -= MathHelper.TwoPi;

            Vector3 vel = Vector3.Zero;
            if (InputManager.Inst.IsKeyDown(GameKey.MoveFoward))
                vel += transform.Forward * forwardAcceleration * speedBonus * (Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveFoward) / 3.0f));
            if (InputManager.Inst.IsKeyDown(GameKey.MoveBackward))
                vel -= transform.Forward * backwardAcceleration * speedBonus * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveBackward) / 1.75f);

            if (InputManager.Inst.IsKeyDown(GameKey.MoveRight))
                vel += transform.Right * strafeAcceleration * speedBonus * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveRight) / 1.25f);
            if (InputManager.Inst.IsKeyDown(GameKey.MoveLeft))
                vel -= transform.Right * strafeAcceleration * speedBonus * Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.MoveLeft) / 1.25f);

            physicsState.velocity = Vector3.Lerp(vel, physicsState.velocity, 0.45f) + (float)Math.Sin(hoverAngle) * hoverMagnitude * transform.Up;

            if (delayTime <= 0 && InputManager.Inst.IsKeyDown(GameKey.Fire))
            {
                explosionMagnitude = 1 + Math.Min(1.0f, InputManager.Inst.GetPressTime(GameKey.Fire) / MAX_PROJECTILE_TIME) * Projectile.EXPLOSION_MAX_MAGNITUDE;
                if (projectile == null)
                {
                    CreateProjectile();
                }
            }
            if (InputManager.Inst.IsLeftJustReleased())
            {
                FireGun(transform.Forward);
            }
        }

        public override void OnUpdate()
        {

            if (!this.IsDead())
            {
                HandleControls();
            }

            Matrix transform = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);

            Vector3 acceleration = Vector3.Zero;

            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);

            Vector3 collNormal = Vector3.Zero;
            if (!scene.MainTerrain.IsCollision(newState.position, out collNormal))
            {
                physicsState = newState;
            }
            else
            {
                physicsState.velocity = Vector3.Reflect(physicsState.velocity, collNormal) * 2.5f;
                physicsState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);
            }
            cameraPosition = physicsState.position + transform.Up * 5f - transform.Forward * 0.25f;

            float nearPlane = 0.15f;
            float farPlane = 2000;

            renderView.SetPosition(cameraPosition);
            renderView.SetView(Matrix.CreateLookAt(cameraPosition, cameraPosition + transform.Forward, Vector3.Up));
            renderView.SetProjection(Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane));
            renderView.SetNearPlane(nearPlane);
            renderView.SetFarPlane(farPlane);
            renderView.UpdateRenderViews(); //Update reflections

            base.OnUpdate();
        }

        public void DrawLives()
        {
            for (int i = 0; i < lives; i++)
            {
                Vector2 max = new Vector2(0.99f - 0.08f * i, 0.98f);
                Vector2 min = new Vector2(0.91f - 0.08f * i, 0.83f);
                Gaia.Resources.TextureResource image = Resources.ResourceManager.Inst.GetTexture("Textures/Details/heart.png");
                GUIElement element = new GUIElement(min, max, image);
                GFX.Inst.GetGUI().AddElement(element);
            }
        }

        public void DrawHealthBar()
        {
            float percentHealth = health / MAX_HEALTH;
            float barBottom = -0.98f;
            float barTop = 0.8f;
            float barLeft = 0.93f;
            float barRight = 0.98f;
            float healthBarTop = barBottom + percentHealth * (barTop - barBottom);

            Vector3 color;
            if (percentHealth > 0.5) color = new Vector3(0.0f, 0.8f, 0.2f);
            else if (percentHealth > 0.25) color = new Vector3(0.8f, 0.8f, 0.0f);
            else color = new Vector3(0.8f, 0.0f, 0.0f);

            GUIElement bar = new GUIElement(new Vector2(barLeft, barBottom), new Vector2(barRight, barTop), null, new Vector4(0.0f, 0.0f, 0.0f, 0.5f));
            GUIElement healthBar = new GUIElement(new Vector2(barLeft, barBottom), new Vector2(barRight, healthBarTop), null, new Vector4(color, 0.5f));
            GUIElement healthBarLine = new GUIElement(new Vector2(barLeft, healthBarTop), new Vector2(barRight, healthBarTop + 0.02f), null, new Vector4(color, 1.0f));
            GFX.Inst.GetGUI().AddElement(bar);
            GFX.Inst.GetGUI().AddElement(healthBar);
            GFX.Inst.GetGUI().AddElement(healthBarLine);
        }

        public void DrawGemProgressBar()
        {
            float percentGemsCollected = (float)numGemsCollected / (float)scene.NUM_GEMS_Required;
            float barBottom = -0.98f;
            float barTop = 0.8f;
            float barLeft = -0.98f;
            float barRight = -0.93f;
            float progressBarTop = barBottom + percentGemsCollected * (barTop - barBottom);

            Vector3 color = new Vector3(0.0f, 0.2f, 0.8f);

            GUIElement bar = new GUIElement(new Vector2(barLeft, barBottom), new Vector2(barRight, barTop), null, new Vector4(0.0f, 0.0f, 0.0f, 0.5f));
            GUIElement progressBar = new GUIElement(new Vector2(barLeft, barBottom), new Vector2(barRight, progressBarTop), null, new Vector4(color, 0.5f));
            GUIElement progressBarLine = new GUIElement(new Vector2(barLeft, progressBarTop), new Vector2(barRight, progressBarTop + 0.02f), null, new Vector4(color, 1.0f));
            GUIElement gemIcon = new GUIElement(new Vector2(-1.0f, 0.82f), new Vector2(-0.9f, 0.98f), Resources.ResourceManager.Inst.GetTexture("Textures/Trees/ruby_Color.png"));
            GFX.Inst.GetGUI().AddElement(bar);
            GFX.Inst.GetGUI().AddElement(progressBar);
            GFX.Inst.GetGUI().AddElement(progressBarLine);
            GFX.Inst.GetGUI().AddElement(gemIcon);
        }

        public void DrawHitFlash()
        {
            float alpha = 1.0f - Math.Abs((float)MAX_FLASH_TIMER / 2.0f - (float)hitFlashTimer) / (float)(MAX_FLASH_TIMER / 2.0f);
            GUIElement flash = new GUIElement(new Vector2(-1.0f, -1.0f), new Vector2(1.0f, 1.0f), null, new Vector4(1.0f, 1.0f, 1.0f, alpha));
            GFX.Inst.GetGUI().AddElement(flash);
        }

        public void DrawDeathFade()
        {
            float alpha = Math.Min((float)deathFadeTimer / 20.0f, 1.0f);
            GUIElement fade = new GUIElement(new Vector2(-1.0f, -1.0f), new Vector2(1.0f, 1.0f), null, new Vector4(0.0f, 0.0f, 0.0f, alpha));
            GFX.Inst.GetGUI().AddElement(fade);
        }
    }

    public class Opponent : Actor
    {
        Vector3 aiVelocityVector = Vector3.Zero;

        protected float speed = 50f;

        public enum EnemyState
        {
            Wander = 0,
            ChasePlayer,
            AttackPlayer,
            Dead
        }

        static float DISTANCE_EPSILON = 1.0f;

        static int WANDER_MAX_MOVES = 3;
        static int WANDER_DISTANCE = 160;
        static float WANDER_DELAY_SECONDS = 4.0f;
        static float ATTACK_DELAY_SECONDS = 1.5f;
        static float SIGHT_DISTANCE = 120;
        static float ATTACK_DISTANCE = 60;
        static float MIN_ATTACK_DISTANCE = 30;

        int wanderMovesCount;
        Vector3 wanderPosition;
        Vector3 wanderStartPosition;
        float wanderDelayTime;

        Actor enemy = null;

        EnemyState state;


        public Opponent(Vector3 pos) : base(pos)
        {}

        public override void OnAdd(Scene scene)
        {
            base.OnAdd(scene);
            this.team = 2;
            this.ResetStates();
            /*
            physicsState.position = Vector3.Transform(initialPos, scene.MainTerrain.Transformation.GetTransform());
            this.Transformation.SetPosition(physicsState.position);
            physicsState.velocity = Vector3.Down * speed;
            */
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void ApplyDamage(Projectile projectile, Vector3 impulseVector)
        {
            Actor sender = projectile.GetSender();
            if (sender.GetTeam() != this.GetTeam() && !sender.IsDead())
            {
                if (enemy != null)
                {
                    enemy = (enemy.GetHealth() <= sender.GetHealth()) ? enemy : sender;
                }
                else
                {
                    enemy = sender;
                }
            }
            base.ApplyDamage(projectile, impulseVector);
        }

        private void Wander()
        {
            // Calculate wander vector on X, Z axis
            Vector3 wanderVector = wanderPosition - physicsState.position;
            wanderVector.Y = 0;
            float wanderVectorLength = wanderVector.Length();

            // Reached the destination position
            if (wanderVectorLength < DISTANCE_EPSILON)
            {
                Random rand = new Random();
                // Generate new random position
                if (wanderMovesCount < WANDER_MAX_MOVES)
                {
                    wanderPosition = physicsState.position +
                        WANDER_DISTANCE * (2.0f*new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble())-Vector3.One);

                    wanderMovesCount++;
                }
                // Go back to the start position
                else
                {
                    wanderPosition = wanderStartPosition;
                    wanderMovesCount = 0;
                }

                // Next time wander
                wanderDelayTime = WANDER_DELAY_SECONDS +
                    WANDER_DELAY_SECONDS * (float)rand.NextDouble();

                aiVelocityVector = Vector3.Zero;
            }

            wanderDelayTime -= Time.GameTime.ElapsedTime;

            // Wait for the next action time
            if (wanderDelayTime <= 0.0f)
            {
                Move(Vector3.Normalize(wanderVector));
            }
        }

        void Move(Vector3 moveDir)
        {
            Vector3 forwardVec = this.Transformation.GetTransform().Forward;
            Vector3 strafeVec = this.Transformation.GetTransform().Right;
            float radianAngle = (float)Math.Acos(forwardVec.Y * moveDir.Y);
            if (radianAngle >= 0.075f)
            {
                if (Vector3.Dot(strafeVec, moveDir) < 0)
                    rotation.Y += radianAngle * 0.02f;
                else
                    rotation.Y -= radianAngle * 0.02f;
            }
            aiVelocityVector = moveDir * speed;
        }

        protected override void ResetStates()
        {
            base.ResetStates();
            wanderMovesCount = 0;
            // Unit configurations
            enemy = null;
          
            wanderPosition = physicsState.position;
            wanderStartPosition = physicsState.position;
            state = EnemyState.Wander;
            
        }

        void AcquireEnemy()
        {
            enemy = null;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < scene.Actors.Count; i++)
            {
                Actor currActor = scene.Actors[i];
                if (currActor.GetTeam() != this.GetTeam() && !currActor.IsDead())
                {
                    float dist = Vector3.DistanceSquared(currActor.Transformation.GetPosition(), this.Transformation.GetPosition());
                    if (dist < minDist)
                    {
                        enemy = currActor;
                        minDist = dist;
                    }
                }
            }
        }


        protected override void OnDeath()
        {
            state = EnemyState.Dead;
            aiVelocityVector = Vector3.Zero;
            physicsState.velocity = Vector3.Zero;
            base.OnDeath();
        }

        void PerformBehavior()
        {
            if (this.IsDead())
            {
                return;
            }

            if (enemy == null || enemy.IsDead())
            {
                AcquireEnemy();
            }
            float distanceToTarget = float.PositiveInfinity;
            Vector3 targetVec = Vector3.Forward;

            if (enemy != null)
            {
                
                targetVec = enemy.Transformation.GetPosition() - physicsState.position;
                distanceToTarget = targetVec.Length();
                targetVec *= 1.0f/distanceToTarget; //Normalize the vector
            }

            switch (state)
            {
                case EnemyState.Wander:
                    if (distanceToTarget < SIGHT_DISTANCE)
                        // Change state
                        state = EnemyState.ChasePlayer;
                    else
                        Wander();
                    break;

                case EnemyState.ChasePlayer:
                    if (distanceToTarget <= ATTACK_DISTANCE)
                    {
                        // Change state
                        state = EnemyState.AttackPlayer;
                        wanderDelayTime = 0;
                    }
                    if (distanceToTarget > SIGHT_DISTANCE * 1.05f)
                        state = EnemyState.Wander;
                    else if (distanceToTarget > MIN_ATTACK_DISTANCE)
                    {
                        Move(targetVec);
                    }
                    else
                    {
                        Move(-targetVec);
                    }
                    break;

                case EnemyState.AttackPlayer:
                    if (distanceToTarget > ATTACK_DISTANCE * 1.5f || distanceToTarget < MIN_ATTACK_DISTANCE)
                    {
                        state = EnemyState.ChasePlayer;
                    }
                    else
                    {
                        Move(targetVec);
                        explosionMagnitude = 1.0f + (float)RandomHelper.RandomGen.NextDouble() * Projectile.EXPLOSION_MAX_MAGNITUDE;
                        FireGun(targetVec);
                    }
                    break;

                default:
                    break;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Vector3 acceleration = Vector3.Zero;

            PerformBehavior();

            physicsState.velocity = Vector3.Lerp(aiVelocityVector, physicsState.velocity, 0.8f);
            State newState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);

            Vector3 collNormal = Vector3.Zero;
            if (!scene.MainTerrain.IsCollision(newState.position, out collNormal))
            {
                physicsState = newState;
            }
            else
            {
                physicsState.velocity = -physicsState.velocity;// Vector3.Reflect(physicsState.velocity, collNormal) * 3.5f;
                physicsState = PhysicsHelper.Integrate(physicsState, acceleration, Time.GameTime.ElapsedTime);
            }
        }
    }
}
