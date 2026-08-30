using BreadLibrary.Core.Graphics;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using FargowiltasSouls.Content.UI;
 
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{

    public abstract class BaseBroadswordProjectile : ModProjectile, IDrawPixelated
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual SoundStyle Swing { get; set; } = DTAssetLib.SwordSounds.Woosh;
        public Asset<Texture2D> Glowmask = null;

        public static int HitCooldownGlobal = 10;
        private int HitCooldown = 0;

        /// <summary>
        /// Use this in place of SetStaticDefaults.
        /// </summary>
        public virtual void SetStaticDefaultsExtra()
        {

        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            ProjectileID.Sets.CanDistortWater[Type] = false;
            SetStaticDefaultsExtra();
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
        }

        public virtual void OnSpawnExtras(IEntitySource source)
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            OnSpawnExtras(source);

            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;

            targetAngle = (Main.MouseWorld - Owner.MountedCenter);
            if (targetAngle == Vector2.Zero)
                targetAngle = Vector2.UnitX * Projectile.spriteDirection;

            LastSwing = Owner.direction == 1 ? -1 : -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public enum State
        {
            SwingDown,
            SwingUp,
            Wait
        }

        public virtual void ExtraEffects()
        {

        }

        public Vector2 sT;
        public Line SL;
        public virtual void SparkEdge(Player owner, float Scale, Color color, int BlendMode = 2)
        {
            sT = Projectile.Center + Projectile.rotation.ToRotationVector2() * (78f * SweepScale * AdjustedScale);
            SL = new Line(Owner.MountedCenter, sT);
            if (CurrentState == State.SwingDown)
            {
                if (SL != null)
                {
                    Spark Spark = new Spark();

                    Spark.PrepareSpark(sT, new Vector2(1, 0).RotatedBy(SL.GetLineRotation + MathHelper.PiOver2), SL.GetLineRotation, color, Scale, false, 30, SparkDrawMode.Additive);
                    Spark.TrackPlayer[Owner.whoAmI] = true;
                    ParticleEngine.BehindProjectiles.Add(Spark);
                }
            }
            if (CurrentState == State.SwingUp)
            {
                if (SL != null)
                {
                    Spark Spark = new Spark();

                    Spark.PrepareSpark(sT, new Vector2(-1, 0).RotatedBy(SL.GetLineRotation + MathHelper.PiOver2), SL.GetLineRotation, color, Scale, false, 30, SparkDrawMode.Additive);
                    Spark.TrackPlayer[Owner.whoAmI] = true;
                    ParticleEngine.BehindProjectiles.Add(Spark);
                }
            }
        }

        # region AI

        public State CurrentState;
        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;
        public float UpPoint = 0f;
        public float DownPoint = 0f;
        public virtual float ScaleMult { get; set; } = 1f;
        public float AdjustedScale = 0f;
        public int NPCHitCooldown = 15;

        public override bool PreAI()
        {
            float AdjScale = Owner.GetAdjustedItemScale(Owner.HeldItem);
            AdjustedScale = AdjScale  * ScaleMult;
            Projectile.scale = AdjustedScale;
            return true;
        }
        public override void AI()
        {
           
            //Slower swing speed, longer cooldown.
            //Swing speed gets slower the lower the number is.
            HitCooldownGlobal = (int)MathHelper.Lerp(5, 15, SwingSpeed / 1f);

            AITimer++;
            if (HitCooldown > 0)
            {
                HitCooldown--;
            }
            if (Owner.controlUseItem)
            {
                Owner.SetDummyItemTime(60);
                
                
                if (CurrentState == State.Wait)
                {
                    targetAngle = (Main.MouseWorld - Owner.MountedCenter);
                }


                UpPoint = targetAngle.ToRotation() - MathHelper.ToRadians(135f);
                DownPoint = targetAngle.ToRotation() + MathHelper.ToRadians(135f);
            }

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || !Owner.controlUseItem)
            {
                Projectile.Kill();
                return;
            }

            ExtraEffects();
            SetSwordPosition();
            ControlRotation();
        }

        public void SetSwordPosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);

            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            //Projectile.scale = 1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * ScaleMult;

            Owner.heldProj = Projectile.whoAmI;
        }

        public virtual void OnStartSwing()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LastSwing"> -1 if the last swing was an upward swing, 1 if it was a downward swing </param> 
        public virtual void BetweenSwing(ref int LastSwing)
        {

        }

        public float Dir => Projectile.spriteDirection;
        private float FixAngle(float angle)
        {
            return MathHelper.WrapAngle(angle);
        }


        bool SetPos = false;
        bool f1 = false;
        /// <summary>
        /// -1 = Up, 1 = Down
        /// <br/> If -1, the current swing will be down.
        /// </summary>
        public int LastSwing = -1;
        public int WaitTimer = 10;

        public float SlashStartRotation = 0f;
        public float SlashProgress = 0f;

        public virtual float SwingSpeed { get; set; } = 0.15f;
        public virtual float WaitTimeMultiplier { get; set; } = 1f;
        public void ControlRotation()
        {
            //float speedFactor = Owner.GetTotalAttackSpeed<DTTrueMeleeClass>();
            //float t = SwingSpeed * speedFactor;
            float baseT = SwingSpeed;
            float speedFactor = Owner.GetTotalAttackSpeed<DTTrueMeleeClass>();

            float t = 1f - MathF.Pow(
                1f - baseT,
                speedFactor / (Projectile.extraUpdates + 1)
            );

            switch (CurrentState)
            {
                case State.SwingUp:
                    {
                        if (!SetPos)
                        {
                            Projectile.rotation = DownPoint;
                            //WaitTimer = (int)((10 * WaitTimeMultiplier) * speedFactor);
                            WaitTimer = (int)((10 * WaitTimeMultiplier) * speedFactor * (Projectile.extraUpdates + 1));
                            SlashStartRotation = DownPoint;
                            SetPos = true;
                        }
                        else
                        {
                            if (!f1)
                            {
                                OnStartSwing();
                                SoundEngine.PlaySound(Swing, Projectile.Center);
                                SweepOpacity = 0.7f;
                                f1 = true;
                            }

                            SlashProgress = Math.Abs(Projectile.rotation - UpPoint) / Math.Abs(SlashStartRotation - UpPoint);

                            SweepOpacity = MathHelper.Lerp(SweepOpacity, 0f, t);
                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, UpPoint, t);
                            if (Math.Abs(Projectile.rotation - UpPoint) < 0.07)
                            {
                                LastSwing = -1;
                                CurrentState = State.Wait;
                            }
                        }

                        break;
                    }
                case State.SwingDown:
                    {
                        if (!SetPos)
                        {
                            Projectile.rotation = UpPoint;
                            //WaitTimer = (int)((10 * WaitTimeMultiplier) * speedFactor);
                            WaitTimer = (int)((10 * WaitTimeMultiplier) * speedFactor * (Projectile.extraUpdates + 1));
                            SlashStartRotation = UpPoint;
                            SetPos = true;
                        }
                        else
                        {
                            if (!f1)
                            {
                                OnStartSwing();
                                SoundEngine.PlaySound(Swing, Projectile.Center);
                                SweepOpacity = 0.7f;
                                f1 = true;
                            }

                            SlashProgress = Math.Abs(Projectile.rotation - DownPoint) / Math.Abs(SlashStartRotation - DownPoint);

                            SweepOpacity = MathHelper.Lerp(SweepOpacity, 0f, t);
                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, DownPoint, t);
                            if (Math.Abs(Projectile.rotation - DownPoint) < 0.07)
                            {
                                LastSwing = 1;
                                CurrentState = State.Wait;
                            }
                        }
                        break;
                    }
                case State.Wait:
                    {
                        BetweenSwing(ref LastSwing);
                        if (WaitTimer > 0)
                        {
                            SetPos = false;
                            f1 = false;
                            SlashStartRotation = 0f;
                            SlashProgress = 0f;
                            WaitTimer--;
                        }
                        else
                        {
                            if (LastSwing == -1)
                            {
                                CurrentState = State.SwingDown;
                            }
                            if (LastSwing == 1)
                            {
                                CurrentState = State.SwingUp;
                            }
                        }
                        break;
                    }
            }
        }

        #endregion

        #region DrawCode

        /// <summary>
        /// If you wanna draw stuff, do it here.
        /// </summary>
        public virtual void DrawOverBlade()
        {

        }

        public virtual void DrawUnderBlade()
        {

        }

        public float SweepOpacity = 0f;

        /// <summary>
        /// Used by the main sweep texture.
        /// </summary>
        public virtual Color SweepColor { get; set; } = Color.White;

        /// <summary>
        /// Using by the sweep highlight texture.
        /// </summary>
        public virtual Color SweepHighlightColor { get; set; } = Color.White;

        /// <summary>
        /// Attempting to do automatic scaling for the sweep textures to match up to the blade's tip have proven generally unsuccessful, plus, what if someone wants a big sweep?
        /// <br>Note that this value will be multiplied by both the player's melee scale modifier and ScaleMult. </br>
        /// </summary>
        public virtual float SweepScale { get; set; } = 1f;

        /// <summary>
        /// Return true if you want your sword to use a standard sweeping effect.
        /// Returns false by default.
        /// </summary>
        public bool UsesDefaultSweepFX { get; set; } = false;

        /// <summary>
        /// Used alongside UsesDefaultSweepFX to draw a fire variant of the sweep effect.
        /// </summary>
        public bool UsesFireSweepFX { get; set; } = false;

        Texture2D Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash").Value;
        Texture2D TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlashEdgeHighlight").Value;

        private void DrawSweepFX()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (UsesFireSweepFX)
            {
                Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3").Value;
                TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3Highlight").Value;
            }
            else
            {
                Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash").Value;
                TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlashEdgeHighlight").Value;
            }

            float rOffset = 0f;

            SpriteEffects FX = SpriteEffects.None;

            if (LastSwing == -1)
            {
                if (Projectile.spriteDirection > 0)
                {
                    FX = SpriteEffects.None;
                    rOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    FX  = SpriteEffects.None;
                    rOffset = MathHelper.ToRadians(45f);
                }
            }
            else
            {
                if (Projectile.spriteDirection > 0)
                {
                    FX  = SpriteEffects.FlipHorizontally;
                    rOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    FX = SpriteEffects.FlipHorizontally;
                    rOffset = MathHelper.ToRadians(45f);
                }
            }

            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;
            spriteBatch.End();
            spriteBatch.Begin(Cap);
            Main.EntitySpriteDraw(Tex, Owner.MountedCenter - Main.screenPosition, null, SweepColor with { A = 0 } * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (SweepScale * AdjustedScale), FX);
            Main.EntitySpriteDraw(TexH, Owner.MountedCenter - Main.screenPosition, null, SweepHighlightColor with { A = 0 } * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (SweepScale * AdjustedScale), FX);
            spriteBatch.ResetToDefault();
        }

        public float RotationManualOffset = 0f;

        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            //i swear to FUCKING GOD.
            //dont touch this shit.
            //FUCK ROTATIONS DUDE.

            if (LastSwing == -1)
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
            }
            else
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
                else
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
            }

            

            DrawUnderBlade();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale, effects, 0);
            }

            DrawOverBlade();
            return false;
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveNPCs;

        bool IDrawPixelated.ShouldDrawPixelated => true;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            if (UsesDefaultSweepFX)
            {
                DrawSweepFX();
            }
        }

        #endregion

        #region Collision
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * AdjustedScale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 25f * Projectile.scale * AdjustedScale, ref collisionPoint);
        }

        public override bool? CanCutTiles()
        {
            return CurrentState != State.Wait;
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * AdjustedScale);
            Utils.PlotTileLine(start, end, 15 * AdjustedScale, DelegateMethods.CutTiles);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return HitCooldown <= 0 && CurrentState != State.Wait && !target.friendly /*&& !OpusNPCDropHelper.IgnoreEnemies.Contains(target.type)*/;
        }

        public virtual void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCooldown = target.realLife == -1 ? HitCooldownGlobal : 15; 

            if (CurrentState != State.Wait)
            {
                HitNPCEffects(target, hit, damageDone);
            }
        }



        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X - Owner.Center.X) > 0 ? 1 : -1;
        }

        #endregion

        

        
    }
}