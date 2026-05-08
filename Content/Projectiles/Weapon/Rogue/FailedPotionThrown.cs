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
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using System.Collections.Generic;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class FailedPotionThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
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

            if (LifeTime < 30)
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
           
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

            List<Color> dustColors = new List<Color>
            {
                Color.Black,
                new Color(32, 11, 40),
                new Color(0, 32, 19)
            };

            Opus.RadialSpreadDustRandom(DustID.Glass, 7, Projectile.Center, 0, default, 2f, 2.5f);

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Main.rand.NextVector2Circular(3, 3), 100, Main.rand.NextFromCollection<Color>(dustColors), 1.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<FailedPotionGreenSmoke>(), 5, Projectile.Center, Projectile.damage, 0, Main.rand.NextFloat(0.03f, 1f));
            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<FailedPotionPurpleSmoke>(), 5, Projectile.Center, Projectile.damage, 0, Main.rand.NextFloat(0.03f, 1f));
        }
    }
}
