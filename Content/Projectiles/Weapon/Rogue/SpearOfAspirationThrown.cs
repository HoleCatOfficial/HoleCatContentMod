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
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Dusts;
using BreadLibrary.Core.Graphics.Particles;
using System.Collections.Generic;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class SpearOfAspirationThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.ContinuouslyUpdateDamageStats = true;
            AdditiveDamage = 0;
            OSE = new List<SpriteEffects>();
            OR = new List<float>();
        }

        public int AdditiveDamage = 0;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            SpriteEffects Fx = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float Rot = Projectile.direction == 1 ? Projectile.velocity.ToRotation() + MathHelper.PiOver4 : Projectile.velocity.ToRotation() - MathHelper.PiOver4;
            OSE.Add(Fx);
            OR.Add(Rot);

            Projectile.ai[0]++;

            if (Projectile.ai[0] % 2 == 0)
            {
                Spark R = new Spark();
                R.PrepareSpark(Projectile.Center + new Vector2(5, 5).RotatedBy(Projectile.rotation), (Projectile.velocity * -0.1f).RotatedBy(-0.05f), Projectile.rotation + MathHelper.PiOver4, Main.DiscoColor, 0.5f, false, 30, SparkDrawMode.Additive, 4f);
                ParticleEngine.BehindProjectiles.Add(R);
                Spark L = new Spark();
                L.PrepareSpark(Projectile.Center + new Vector2(-5, -5).RotatedBy(Projectile.rotation), (Projectile.velocity * -0.1f).RotatedBy(0.05f), Projectile.rotation + MathHelper.PiOver4, Main.DiscoColor, 0.5f, false, 30, SparkDrawMode.Additive, 4f);
                ParticleEngine.BehindProjectiles.Add(L);
            }

            
            if (AdditiveDamage < 50)
            {
                AdditiveDamage++;
            }
                
            
            if (Main.rand.NextBool(3))
            {
                /*
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Projectile.velocity * 0.2f, 100, Color.White, 1.2f);
                dust.noGravity = true;
                */

                //PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), Projectile.Center, Projectile.velocity * 0.5f, Color.White, 1f);
            }

            Projectile.damage = Projectile.originalDamage + AdditiveDamage;

        }

        List<SpriteEffects> OSE;
        List<float> OR;
        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.DrawDirectionalAfterimages(Main.spriteBatch, TextureAssets.Projectile[Type].Value, Color.White, OSE.ToArray(), OR.ToArray(), 1f, true);
            //Projectile.DrawAfterimagesWithRotOffset(Main.spriteBatch, Color.White, 1f, true, RotOffset: MathHelper.PiOver4, shrink: false);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            

            Opus.DrawProjectileShadowsRotating(Projectile, 4, Color.White, 0.2f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            DTUtils.GenericSparkleEffect(target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            DTUtils.GenericSparkleEffect(Projectile.Center);
        }
    }
}