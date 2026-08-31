using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
 
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class TrueRiftMakerThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            if (!stuck)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                Spark Spark = new();
                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Color.OrangeRed, 0.4f, false, 20, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(Spark);

            }
            else
            {
                Spark Spark = new();
                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -(Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 16f, Projectile.rotation + MathHelper.PiOver4, Color.OrangeRed, 0.4f, false, 30, SparkDrawMode.Additive, 4f);
                ParticleEngine.Particles.Add(Spark);

                Projectile.velocity *= 0;
            }



        }
        public override bool PreDraw(ref Color lightColor)
        {

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 40f, Color.OrangeRed, 0f);

            Opus.DrawProjectileShadowsRotating(Projectile, 8, Color.OrangeRed with { A = 0 }, 0.2f);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }

        bool stuck = false;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!stuck)
            {
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Hit, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 2f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrueRiftMakerEclipseAura>(), Projectile.damage, 10, Projectile.owner);
                Projectile.timeLeft = 120;
                stuck = true;
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrueRiftMakerEclipseAura>(), Projectile.damage, 10, Projectile.owner);
            }
        }


    }
}