
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content;
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
using BreadLibrary.Core.Graphics.Particles;


namespace DestroyerTest.Common
{
    public class WeaponImbueScepter : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool HasImbue = false;

        public bool GalantineBurn = false;
        public bool Honey;
        public bool Mud;
        public bool GalantineHoney = false;
        public bool Brine = false;
        public bool FrostBurn = false;
        public bool FrostBite = false;
        public bool Fire = false;
        public bool HellFire = false;
        public bool Ichor = false;
        public bool CursedFlame = false;
        public bool HeliouricShock = false;
        public bool DaylightOverload = false;
        public bool ComaceraticBurn = false;
        public bool shimmeringFlames = false;
        public bool Scepter = false;

        private void DustInEnchantVisuals(ThrownScepter t, int ID, int alpha, Color color, float scale, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(t.EnchantmentVisuals().TopLeft(), t.EnchantmentVisuals().Width, t.EnchantmentVisuals().Height, ID, 0, 0, alpha, color, scale);
            dust.noGravity = noGravity;
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
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40, ColorLib.Rift, 0.5f, false);

                        ElectricArc Arc = new();
                        Arc.Create(Main.rand.NextVector2FromRectangle(thrown.EnchantmentVisuals()), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                        ParticleEngine.ShaderParticles.Add(Arc);

                    }
                    if (DaylightOverload)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                    if (ComaceraticBurn)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, ModContent.DustType<RiftDust>(), 0, 0, 40, ColorLib.Rift, 1.0f);
                    }
                    if (GalantineBurn)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40,  ColorLib.StellarFireGradientLooping(), 0.7f, false);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.TintableDustLighted, 0, 0, 40,  ColorLib.StellarFireGradientLooping(), 1.0f);
                    }
                    if (Brine)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Water_Snow, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Water_Snow, 0, 0, 40, default, 1.0f);  
                    }
                    if (Mud)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Mud, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Mud, 0, 0, 40, default, 1.0f);
                    }
                    if (FrostBurn)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.IceTorch, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.IceTorch, 0, 0, 40, default, 1.0f);
                    }
                    if (FrostBite)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.IceTorch, 40, default, 1f);
                    }
                    if (shimmeringFlames)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.TintableDustLighted, 0, ColorLib.TenebrisGradient, 1f);
                    }
                    if (Fire)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Torch, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Torch, 0, 0, 40, default, 1.0f);
                    }
                    if (HellFire)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Lava, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Torch, 0, 0, 40, default, 1.0f);
                    }
                    if (Ichor)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Ichor, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Ichor, 0, 0, 40, default, 1.0f);
                    }
                    if (CursedFlame)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.CursedTorch, 40, default, 1f);
                    }
                    

                    if (Honey)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Honey, 40, default, 1f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Honey, 0, 0, 40, default, 1.0f);
                        if (Main.rand.NextBool(10))
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.Bee, projectile.damage / 3, 4, projectile.owner);
                        }
                    }
                    if (GalantineHoney)
                    {
                        if (!Main.masterMode)
                        {
                            HasImbue = true;
                        }
                        DustInEnchantVisuals(thrown, DustID.Honey, 40, default, 1f);
                        DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40,  ColorLib.StellarFireGradientLooping(), 0.7f, false);

                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Honey, 0, 0, 40, default, 1.0f);
                        //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.TintableDustLighted, 0, 0, 40,  ColorLib.StellarFireGradientLooping(), 1.0f);
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
                if (FrostBite)
                {
                    target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(10, 17));
                }
                if (Fire)
                {
                    target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(10, 17));
                }
                if (HellFire)
                {
                    target.AddBuff(BuffID.OnFire3, 60 * Main.rand.Next(10, 17));
                }
                if (Ichor)
                {
                    target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 17));
                }
                if (CursedFlame)
                {
                    target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(10, 17));
                }
                if (shimmeringFlames)
                {
                    ShimmeringFlames.ShimmerBurn(target);
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