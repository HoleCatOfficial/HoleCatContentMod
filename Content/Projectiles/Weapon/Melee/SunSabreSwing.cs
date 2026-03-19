using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class SunSaberSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 46;
            SweepColor = Color.DarkOrange;
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.SpiritOfJusticeSwing;

        public override void ExtraEffects()
        {
            SparkEdge(Main.player[Projectile.owner], 1f, Color.PaleGoldenrod);
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot);
            npc.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }
        /*
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
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

        public State CurrentState;
        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;
        public float UpPoint = 0f;
        public float DownPoint = 0f;

        public override void AI()
        {
            AITimer++;
            if (Owner.controlUseItem)
            {
                Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
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

            SetSwordPosition();
            ControlRotation();
        }

        bool SetPos = false;
        bool f1 = false;
        public int LastSwing = -1;
        public int WaitTimer = 10;
        public void ControlRotation()
        {
            float speedFactor = Owner.GetAttackSpeed(DamageClass.Melee);
            float t = 0.15f * speedFactor;

            switch (CurrentState)
            {
                case State.SwingUp:
                {
                    if (!SetPos)
                    {
                        Projectile.rotation = DownPoint;
                        WaitTimer = (int)(10 * Owner.GetAttackSpeed(DamageClass.Melee));
                        SetPos = true;
                    }
                    else
                    {
                        if (!f1)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                            f1 = true;
                        }

                        
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
                        SetPos = true;
                    }
                    else
                    {
                        if (!f1)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
                            f1 = true;
                        }
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
                    if (WaitTimer > 0)
                    {
                        SetPos = false;
                        f1 = false;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D powertexture = DTAssetLib.QuixotismPowerAura.Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            else
            {
                origin = new Vector2(texture.Width, texture.Height);
                effects = SpriteEffects.FlipHorizontally;
                rotationOffset = MathHelper.ToRadians(135f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
        }



        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.HitDirectionOverride = (int?)(target.position.Y + 15);
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
            Projectile.scale = 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }
        */
    }
}