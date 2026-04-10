using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using FargowiltasSouls.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
    public abstract class BaseSpearProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual SoundStyle Woosh { get; set; } = DTAssetLib.SwordSounds.Woosh;

        public Asset<Texture2D> Glowmask = null;

        public virtual float MaxExtension => 110f;
        public virtual float MinExtension => 1f;

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
            Projectile.DamageType = DamageClass.Melee;
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


        public virtual Vector2 ShotPos()
        {
            Vector2 orig = Projectile.Center + new Vector2(-(Projectile.width / 2), Projectile.height / 2);
            return orig + new Vector2(20, 20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
        }

        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;


        public override void AI()
        {
            //Dust.NewDustPerfect(ShotPos(), DustID.Torch);
            AITimer++;
            if (Owner.controlUseItem)
            {
                targetAngle = (Main.MouseWorld - Owner.MountedCenter);

                Projectile.rotation = targetAngle.ToRotation() + MathHelper.PiOver4;

                
            }

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || !Owner.controlUseItem)
            {
                Projectile.Kill();
                return;
            }

            ExtraEffects();
            SetPosition();
        }

        public void UpdatePosition()
        {

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
                //Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
            }
            else
            {
                //Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
            }

            DrawUnder();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, texture.Size() / 2, Projectile.scale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, texture.Size() / 2, Projectile.scale, effects, 0);
            }

            DrawOver();
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center - (Projectile.Size / 2).RotatedBy(Projectile.rotation);
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }


        public void SetPosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));

            float DistTraveled = MaxExtension - MinExtension;
            float CurrentExtension = MathHelper.Lerp(MinExtension, MaxExtension, Utilities.Convert01To010(DistTraveled / MaxExtension));
            Projectile.Center = Owner.MountedCenter + Owner.RotatedRelativePoint(new Vector2(CurrentExtension, 0f).RotatedBy(targetAngle.ToRotation()), false, true);
            
            Projectile.scale = 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }
    }
}
