
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class WeaponImbueScepter : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool GalantineBurn = false;
        public bool GalantineHoney = false;
        public bool Brine = false;
        public bool FrostBurn = false;
        public bool Fire = false;
        public bool Ichor = false;
        public bool CursedFlame = false;
        public bool HeliouricShock = false;
        public bool DaylightOverload = false;
        public bool ComaceraticBurn = false;
        public bool Scepter = false;

        public override void AI(Projectile projectile)
        {
            Scepter = projectile.DamageType == ModContent.GetInstance<ScepterClass>();
            int[] types = new int[]
                {
                    PRTLoader.GetParticleID<Arc1>(),
                    PRTLoader.GetParticleID<Arc2>(),
                    PRTLoader.GetParticleID<Arc3>()
                };

            if (Scepter)
            {
                if (HeliouricShock)
                {
                    if (Main.rand.NextBool(5))
                    {
                        PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Main.rand.NextVector2FromRectangle(projectile.Hitbox), Vector2.Zero, ColorLib.Rift, 0.05f);
                    }
                }
                if (DaylightOverload)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust.NewDust(projectile.Center, projectile.Hitbox.Width, projectile.Hitbox.Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                }
                if (ComaceraticBurn)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust.NewDust(projectile.Center, projectile.Hitbox.Width, projectile.Hitbox.Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                }
                if (GalantineBurn)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust.NewDust(projectile.Center, projectile.Hitbox.Width, projectile.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 40, ColorLib.StellarColor, 1.0f);
                    }
                }
                if (Brine)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Dust.NewDust(projectile.Center, projectile.Hitbox.Width, projectile.Hitbox.Height, DustID.Water_Snow, 0, 0, 40, default, 1.0f);
                    }
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Scepter)
            {
                if (HeliouricShock)
                {
                    target.AddBuff(ModContent.BuffType<HeliouricShock>(), 60 * Main.rand.Next(10, 17));
                }
                if (DaylightOverload)
                {
                    target.AddBuff(ModContent.BuffType<DaylightOverload>(), 60 * Main.rand.Next(10, 17));
                }
                if (ComaceraticBurn)
                {
                    target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (GalantineBurn)
                {
                    target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (Brine)
                {
                    target.AddBuff(ModContent.BuffType<Brine>(), 60 * Main.rand.Next(10, 17));
                }
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (Scepter)
            {
                if (HeliouricShock)
                {
                    target.AddBuff(ModContent.BuffType<HeliouricShock>(), 60 * Main.rand.Next(10, 17));
                }
                if (DaylightOverload)
                {
                    target.AddBuff(ModContent.BuffType<DaylightOverload>(), 60 * Main.rand.Next(10, 17));
                }
                if (ComaceraticBurn)
                {
                    target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (GalantineBurn)
                {
                    target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (Brine)
                {
                    target.AddBuff(ModContent.BuffType<Brine>(), 60 * Main.rand.Next(10, 17));
                }
            }
        }
    }
}