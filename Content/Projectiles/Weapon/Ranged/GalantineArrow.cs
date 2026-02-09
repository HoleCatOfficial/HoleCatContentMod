using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class GalantineArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public SoundStyle kill = new SoundStyle($"DestroyerTest/Assets/Audio/StellarBow/StellarBowArrowImpact", 4) with
        {
            PitchVariance = 0.2f,
            MaxInstances = 0
        };

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, default, 0.5f);
            }
			
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null,  ColorLib.StellarFireGradientLooping(3f), Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DTUtils.ConstitutionStarExplosionEffects(Projectile);
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
            SoundEngine.PlaySound(kill, target.Center);
        }
    }
}