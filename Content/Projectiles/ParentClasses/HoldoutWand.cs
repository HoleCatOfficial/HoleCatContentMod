using BreadLibrary.Core.Graphics;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
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

    public abstract class HoldoutWand : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual SoundStyle ShootSound { get; set; } = DTAssetLib.Impacts.AmbitionChargeBurst;

        public Asset<Texture2D> Glowmask = null;

        /// <summary>
        /// Use this in place of SetStaticDefaults.
        /// </summary>
        public virtual void SetStaticDefaultsExtra()
        {

        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            SetStaticDefaultsExtra();
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
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
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public virtual void ExtraEffects()
        {

        }

        public abstract void Shoot();
        
        public virtual Vector2 ShotPos()
        {
            Vector2 orig = Projectile.Center + new Vector2(-(Projectile.width / 2), Projectile.height / 2);
            return orig + new Vector2(20, 20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
        }

        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;

        public virtual int Interval { get; set; } = 60;
        private int interval = 60;
        public virtual int ManaCostPerShot { get; set; } = 10;

        public override void AI()
        {
            //Dust.NewDustPerfect(ShotPos(), DustID.Torch);
            AITimer++;
            if (Owner.controlUseItem)
            {
                interval = (int)(Interval * Owner.GetTotalAttackSpeed(DamageClass.Magic));

                if (AITimer % interval == 0)
                {
                    if (Owner.CheckMana(ManaCostPerShot, true))
                    {
                        SoundEngine.PlaySound(ShootSound, Projectile.Center);
                        Shoot();
                    }
                }

                targetAngle = (Main.MouseWorld - Owner.MountedCenter);

                Projectile.rotation = targetAngle.ToRotation();
              
            }

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || !Owner.controlUseItem)
            {
                Projectile.Kill();
                return;
            }

            ExtraEffects();
            SetPosition();
        }

        /// <summary>
        /// If you wanna draw stuff, do it here.
        /// </summary>
        public virtual void DrawOver()
        {

        }

        public virtual void DrawUnder()
        {

        }


        public float RotationManualOffset = 0f;
        public Vector2 Draworigin;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (Projectile.spriteDirection > 0)
            {
                Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
            }
            else
            {
                Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
            }

            DrawUnder();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, Draworigin, Projectile.scale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, Draworigin, Projectile.scale, effects, 0);
            }

            DrawOver();
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }


        public void SetPosition()
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


    }
}