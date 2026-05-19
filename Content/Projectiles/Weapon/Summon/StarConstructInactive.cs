using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;

using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class StarConstructInactive : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<StarConstructMinionBuff>(), 20);

            if (Main.GameUpdateCount % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.75f, MaxInstances = 0 }, Projectile.Center);
            }

            Projectile.velocity *= 0.99f;
            Projectile.rotation += 0.5f * Projectile.direction;
        
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ConstitutionDust1>(), Projectile.velocity * 0.2f, 100,  ColorLib.StellarFireGradientLooping(), 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StellarBow/StellarBowEmpoweredShoot", 3)
            {
                PitchVariance = 0.4f,
                MaxInstances = 0
            }, Projectile.Center);

            Opus.RadialSpreadDustRandom(ModContent.DustType<ConstitutionDust1>(), 15, Projectile.Center, 0,  ColorLib.StellarFireGradientLooping(), 1f, 3);

            DTUtils.ConstitutionStarExplosionEffects(Projectile);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0.00003f, 0.00003f), ModContent.ProjectileType<StarConstructMinion>(), 20, 1f, Projectile.owner);
        }
    }
}
