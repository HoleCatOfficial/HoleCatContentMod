using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class DesertSpiritDart : ModProjectile, IDrawPixelated
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 170;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true; 
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public Color FireColor;

        public float trailOffset = 0;
        public override bool PreDraw(ref Color lightColor)
		{

			return false;
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.04f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, PixelationSystem.PixelationMatrix);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, FireColor with { A = 0 }, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.AlphaBlend, DTAssetLib.Streak(4, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 40, FireColor with { A = 0}, trailOffset, 1);

            
            
            
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }

        public float spiritFireTime;
        public override bool PreAI()
        {
            spiritFireTime += 0.025f;
            FireColor = ColorLib.SpiritFireGradient(spiritFireTime);
            return true;
        }

     

		public override void AI()
		{
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }

            Lighting.AddLight(Projectile.Center, FireColor.ToVector3() * 0.6f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            float maxDetectRadius = 4000f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            int turnspeed = 5;
            turnspeed += 10;
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(turnspeed)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }


        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            return target.CanBeChasedBy();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown);
            target.AddBuff(BuffID.ShadowFlame, 600);
        }

        
    }
}