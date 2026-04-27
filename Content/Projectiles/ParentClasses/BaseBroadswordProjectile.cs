using BreadLibrary.Core.Graphics;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using FargowiltasSouls.Content.UI;
using InnoVault.PRT;
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
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{

    public abstract class BaseBroadswordProjectile : ModProjectile
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
        }

        public virtual void OnSpawnExtras()
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            OnSpawnExtras();

            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;

            targetAngle = (Main.MouseWorld - Owner.MountedCenter);
            if (targetAngle == Vector2.Zero)
                targetAngle = Vector2.UnitX * Projectile.spriteDirection;

            LastSwing = Owner.direction == 1 ? 1 : -1;
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
            sT = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            SL = new Line(Owner.MountedCenter, sT);
            if (CurrentState == State.SwingDown)
            {
                if (SL != null)
                {
                    Spark Spark = new Spark();

                    Spark.PrepareSpark(sT, new Vector2(1, 0).RotatedBy(SL.GetLineRotation + MathHelper.PiOver2), 0f, color, Scale, false, 30, SparkDrawMode.Additive);
                    Spark.position += Owner.velocity;
                    ParticleEngine.BehindProjectiles.Add(Spark);
                }
            }
            if (CurrentState == State.SwingUp)
            {
                if (SL != null)
                {
                    Spark Spark = new Spark();

                    Spark.PrepareSpark(sT, new Vector2(-1, 0).RotatedBy(SL.GetLineRotation + MathHelper.PiOver2), 0f, color, Scale, false, 30, SparkDrawMode.Additive);
                    Spark.position += Owner.velocity;
                    ParticleEngine.BehindProjectiles.Add(Spark);
                }
            }
        }

        public State CurrentState;
        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;
        public float UpPoint = 0f;
        public float DownPoint = 0f;
        public virtual float ScaleMult { get; set; } = 1f;
        public float AdjustedScale = 0f;

        public override void AI()
        {
            AITimer++;
            if (HitCooldown > 0)
            {
                HitCooldown--;
            }
            if (Owner.controlUseItem)
            {
                Owner.SetDummyItemTime(60);
                AdjustedScale = Owner.GetAdjustedItemScale(Owner.HeldItem) * ScaleMult;
                FactorFargosScaling();
                Projectile.scale = AdjustedScale;
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

        private void FactorFargosScaling()
        {
            if (DTCrossMod.FargosSoulsIsLoaded)
            {
                GlobalProjectile gp =
                Projectile.GetGlobalProjectile(
                    DTCrossMod.FargosSoulsMod.Find<GlobalProjectile>("FargoSoulsGlobalProjectile")
                );

                var field = gp.GetType().GetField(
                    "TungstenScale",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                );

                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("TungstenEnchantment", out var Tungst))
                {
                    if (Owner.miscEquips.Contains(Tungst.Item))
                    {

                        if (field != null)
                        {
                            float tungstenScale = (float)field.GetValue(gp);
                            AdjustedScale = Owner.GetAdjustedItemScale(Owner.HeldItem) * ScaleMult * tungstenScale;
                        }
                    }
                }
            }
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
        public void ControlRotation()
        {
            float speedFactor = Owner.GetTotalAttackSpeed(DamageClass.Melee);
            float t = SwingSpeed * speedFactor;

            switch (CurrentState)
            {
                case State.SwingUp:
                    {
                        if (!SetPos)
                        {
                            Projectile.rotation = DownPoint;
                            WaitTimer = (int)(10 * Owner.GetAttackSpeed(DamageClass.Melee));
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
                            WaitTimer = (int)(10 * Owner.GetAttackSpeed(DamageClass.Melee));
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
        public virtual Color SweepColor { get; set; } = Color.White;

        private void DrawSweepFX()
        {
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash2").Value;
            float TexBasedMod = (Projectile.Size.Length() * 0.015f);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(Tex, Owner.MountedCenter - Main.screenPosition, null, SweepColor * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4), Tex.Size() / 2, (AdjustedScale * TexBasedMod), SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
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


            DrawSweepFX();

            DrawUnderBlade();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale * AdjustedScale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + rotationOffset) + RotationManualOffset, origin, Projectile.scale * AdjustedScale, effects, 0);
            }

            DrawOverBlade();
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * AdjustedScale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * AdjustedScale);
            Utils.PlotTileLine(start, end, 15 * AdjustedScale, DelegateMethods.CutTiles);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return HitCooldown <= 0 && !target.friendly && !OpusNPCDropHelper.IgnoreEnemies.Contains(target.type);
        }

        public virtual void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCooldown = HitCooldownGlobal;
            HitNPCEffects(target, hit);
        }



        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X - Owner.Center.X) > 0 ? 1 : -1;
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
            Projectile.scale = 1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * ScaleMult;

            Owner.heldProj = Projectile.whoAmI;
        }

        
    }
}