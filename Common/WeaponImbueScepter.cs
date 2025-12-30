
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.player.Accessory;
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
        public bool Honey;
        public bool Mud;
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

        private void DustInEnchantVisuals(ThrownScepter t, int ID, int alpha, Color color, float scale, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(t.EnchantmentVisuals().TopLeft(), t.EnchantmentVisuals().Width, t.EnchantmentVisuals().Height, ID, 0, 0, alpha, color, scale);
            dust.noGravity = noGravity;
            dust.fadeIn = 10;
        }

        public override void AI(Projectile projectile)
        {
            if (projectile.ModProjectile is ThrownScepter thrown)
            {
                Scepter = true;
                

                if (Scepter)
                {
                    if (HeliouricShock)
                    {
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40, ColorLib.Rift, 0.5f, false);
                        PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(thrown.EnchantmentVisuals()), Vector2.Zero, ColorLib.Rift, 0.05f);
                    }
                    if (DaylightOverload)
                    {
                        DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                    if (ComaceraticBurn)
                    {
                        DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                    if (GalantineBurn)
                    {
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40, ColorLib.StellarColor, 0.7f, false);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.TintableDustLighted, 0, 0, 40, ColorLib.StellarColor, 1.0f);
                    }
                    if (Brine)
                    {
                        DustInEnchantVisuals(thrown, DustID.Water_Snow, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Water_Snow, 0, 0, 40, default, 1.0f);  
                    }
                    if (Mud)
                    {
                        DustInEnchantVisuals(thrown, DustID.Mud, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Mud, 0, 0, 40, default, 1.0f);
                    }
                    if (FrostBurn)
                    {
                        DustInEnchantVisuals(thrown, DustID.IceTorch, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.IceTorch, 0, 0, 40, default, 1.0f);
                    }
                    if (Fire)
                    {
                        DustInEnchantVisuals(thrown, DustID.Torch, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Torch, 0, 0, 40, default, 1.0f);
                    }
                    if (Ichor)
                    {
                        DustInEnchantVisuals(thrown, DustID.Ichor, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Ichor, 0, 0, 40, default, 1.0f);
                    }
                    if (CursedFlame)
                    {
                        DustInEnchantVisuals(thrown, DustID.CursedTorch, 40, default, 1f);
                    }
                    

                    if (Honey)
                    {
                        DustInEnchantVisuals(thrown, DustID.Honey, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Honey, 0, 0, 40, default, 1.0f);
                        if (Main.rand.NextBool(10))
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.Bee, projectile.damage / 3, 4, projectile.owner);
                        }
                    }
                    if (GalantineHoney)
                    {
                        DustInEnchantVisuals(thrown, DustID.Honey, 40, default, 1f);
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40, ColorLib.StellarColor, 0.7f, false);

                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Honey, 0, 0, 40, default, 1.0f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.TintableDustLighted, 0, 0, 40, ColorLib.StellarColor, 1.0f);
                        if (Main.rand.NextBool(10))
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<GalantineBee>(), projectile.damage / 3, 4, projectile.owner);
                        }   
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
                if (GalantineBurn || GalantineHoney)
                {
                    target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (Brine)
                {
                    target.AddBuff(ModContent.BuffType<Brine>(), 60 * Main.rand.Next(10, 17));
                }
                if (Mud)
                {
                    target.AddBuff(ModContent.BuffType<Muddy>(), 60 * Main.rand.Next(10, 17));
                }
                if (FrostBurn)
                {
                    target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(10, 17));
                }
                if (Fire)
                {
                    target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(10, 17));
                }
                if (Ichor)
                {
                    target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 17));
                }
                if (CursedFlame)
                {
                    target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(10, 17));
                }
                if (Honey || GalantineHoney)
                {
                    target.AddBuff(BuffID.Slow, 60 * Main.rand.Next(10, 17));
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
                if (GalantineBurn || GalantineHoney)
                {
                    target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
                }
                if (Brine)
                {
                    target.AddBuff(ModContent.BuffType<Brine>(), 60 * Main.rand.Next(10, 17));
                }
                if (Mud)
                {
                    target.AddBuff(ModContent.BuffType<Muddy>(), 60 * Main.rand.Next(10, 17));
                }
                if (FrostBurn)
                {
                    target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(10, 17));
                }
                if (Fire)
                {
                    target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(10, 17));
                }
                if (Ichor)
                {
                    target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 17));
                }
                if (CursedFlame)
                {
                    target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(10, 17));
                }
                if (Honey || GalantineHoney)
                {
                    target.AddBuff(BuffID.Slow, 60 * Main.rand.Next(10, 17));
                }
            }
        }
    }
}