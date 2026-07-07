using BreadLibrary.Core;
using DestroyerTest.Common;
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
        public virtual SoundStyle JabSound { get; set; } = DTAssetLib.SwordSounds.Woosh;

        public Asset<Texture2D> Glowmask = null;

        public virtual float MaxExtension { get; set; } = 110f;
        public virtual float MinExtension { get; set; } = 1f;

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

        public virtual void OnStart()
        {

        }
        
        public virtual void AtFullExtension()
        {

        }


        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;

        bool OnStartFlag = false;
        bool OnExtendFlag = false;
        public override void AI()
        {
            //Dust.NewDustPerfect(ShotPos(), DustID.Torch);
            AITimer++;
            Owner.SetDummyItemTime(2);
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
        float Off = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (Projectile.spriteDirection > 0)
            {
                //Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                Off = 0;
            }
            else
            {
                //Draworigin = new Vector2(0, texture.Height);
                effects = SpriteEffects.FlipHorizontally;
                Off = MathHelper.PiOver2;
            }

            DrawUnder();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation + Off) + RotationManualOffset, texture.Size() / 2, Projectile.scale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + Off) + RotationManualOffset, texture.Size() / 2, Projectile.scale, effects, 0);
            }

            DrawOver();
            return false;
        }


        public Vector2 Tip;
        public float ExtraLength = 0f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center /*- (new Vector2(-Projectile.width / 2, Projectile.height / 2)).RotatedBy(Projectile.rotation)*/;
            Vector2 end = start + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (new Vector2(-((Projectile.width / 2) + ExtraLength), ((Projectile.height / 2) + ExtraLength)).Length()) * Projectile.scale;
            //Dust.NewDustPerfect(end, DustID.WhiteTorch).noGravity = true;
            Tip = end;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        public bool FirstHalf = true;
        public float progress;
        public void SetPosition()
        {
            Projectile.ai[0]++;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi + MathHelper.PiOver4);
            

            progress = Projectile.ai[0] / (30f / (Owner.GetTotalAttackSpeed(DamageClass.Melee))); // 20 = duration of thrust
            progress = Saturate(progress);

            /*
            if (progress == 1.00f)
            {
                Projectile.Kill();
            }
            */

            float bump = Convert01To010(progress);

            float CurrentExtension = MathHelper.Lerp(MinExtension, MaxExtension, bump);

            if (progress < 0.5f)
            {
                if (!OnStartFlag)
                {
                    SoundEngine.PlaySound(JabSound, Projectile.Center);
                    OnStart();
                    OnStartFlag = true;
                    OnExtendFlag = false;
                }
                OnStart();
            }
            if (progress >= 0.5f)
            {
                if (!OnExtendFlag)
                {
                    AtFullExtension();
                    OnStartFlag = false;
                    OnExtendFlag = true;
                }
              
            }

            //Projectile.Center = Owner.MountedCenter + new Vector2(CurrentExtension, 0f).RotatedBy(targetAngle.ToRotation());
            Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.None, targetAngle.ToRotation()) + new Vector2(CurrentExtension, 0f).RotatedBy(targetAngle.ToRotation());

            Projectile.scale = 1f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }

        /// <summary>
        ///     Clamps a given number between 0 and 1.
        /// </summary>
        /// <param name="x">The number to clamp.</param>
        public static float Saturate(float x)
        {
            if (x > 1f)
                return 1f;
            if (x < 0f)
                return 0f;
            return x;
        }

        /// <summary>
        ///     Commonly known as a sine bump. Converts 0 to 1 values to a 0 to 1 to 0 again bump.
        /// </summary>
        /// <param name="x">The input number.</param>
        public static float Convert01To010(float x) => MathF.Sin(float.Pi * Saturate(x));
    }
}
