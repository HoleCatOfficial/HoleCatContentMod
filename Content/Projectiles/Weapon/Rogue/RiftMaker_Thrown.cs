using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Terraria.Audio;
 
using DestroyerTest.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;
using System.Linq;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class RiftMaker_Thrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 200;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            stuck = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            if (!stuck)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                Spark Spark = new();
                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.Rift, 0.4f, false, 20, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(Spark);

            }
            else
            {
                Spark Spark = new();
                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -(Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 16f, Projectile.rotation + MathHelper.PiOver4, ColorLib.Rift, 0.4f, false, 30, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(Spark);

                Projectile.velocity *= 0;
            }

            

        }
        public override bool PreDraw(ref Color lightColor)
        {

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Square.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 2f, ColorLib.Rift, 0f);
            
            Opus.DrawProjectileShadowsRotating(Projectile, 8, ColorLib.Rift with { A = 0 }, 0.2f);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Zap with { MaxInstances = 0, PitchVariance = 0.3f}, target.Center);
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
        }

        bool stuck = false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!stuck)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.MetalImpact with {  }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 2f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RiftMakerEclipseAura>(), Projectile.damage, 10, Projectile.owner);
                Projectile.timeLeft = 120;
                stuck = true;
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RiftMakerEclipseAura>(), Projectile.damage, 10, Projectile.owner);
            }
        }

    }
}