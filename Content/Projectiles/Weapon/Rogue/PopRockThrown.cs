using DestroyerTest.Common;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    [AutoloadGlowmask]
    public class PopRockThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return true;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (LifeTime < 15)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else
            {
                if (Main.GameUpdateCount % 12 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.75f, MaxInstances = 0 }, Projectile.Center);
                }

                Projectile.velocity.Y += 0.2f;
                Projectile.rotation += 0.5f * Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Opus.RadialProjectileRandomDir(ModContent.ProjectileType<PopRockShrapnel>(), 3, Projectile.Center, Projectile.damage / 2, 0, Main.rand.NextFloat(0.1f, 3f), friendly: true);

            Opus.RadialProjectileRandomDir(ModContent.ProjectileType<RiftSpark>(), 5, Projectile.Center, Projectile.damage / 6, 0, 2, friendly: true);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(3, 3), 100, ColorLib.Rift, 1.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
    }
}
