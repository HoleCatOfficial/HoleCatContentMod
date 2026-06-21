using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class BlitzCrystalBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 4;
            Projectile.ArmorPenetration = 20;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust FX = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.5f, 0, ColorLib.Ichor, 0.5f);
            FX.noGravity = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 300);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.Deflect with { Pitch = -0.7f, PitchVariance = 0.2f, Volume = 0.5f }, Projectile.Center);


            var D1 = Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 2, Projectile.Center, 0, ColorLib.IchorCrystal1, 2f, Main.rand.NextFloat(5f, 12f));
            var D2 = Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 3, Projectile.Center, 0, ColorLib.IchorCrystal2, 0.6f, 3f);
            var D3 = Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 5, Projectile.Center, 70, ColorLib.IchorCrystal3, 1f, 4f);

            for (int i = 0; i < D1.Length; i++)
            {
                D1[i].noGravity = true;
            }

            for (int i = 0; i < D2.Length; i++)
            {
                D2[i].noGravity = true;
            }

            for (int i = 0; i < D3.Length; i++)
            {
                D3[i].noGravity = true;
            }
        }
    }
}