
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.player.Accessory;
 
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles.Stellar;
using OpusLib.Content.Particles;
using BreadLibrary.Core.Graphics.Pixelation;


namespace DestroyerTest.Common
{
    public class WeaponImbueScepter : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private void DustInEnchantVisuals(ThrownScepter t, int ID, int alpha, Color color, float scale, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(t.EnchantmentVisuals().TopLeft(), t.EnchantmentVisuals().Width, t.EnchantmentVisuals().Height, ID, 0, 0, alpha, color, scale);
            dust.noGravity = noGravity;
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.ModProjectile is ThrownScepter thrown)
            {
                if (player.HasBuff(BuffID.WeaponImbueFire))
                {
                    DustInEnchantVisuals(thrown, DustID.Torch, 40, default, 1f);
                }
                if (player.HasBuff(BuffID.WeaponImbueIchor))
                {
                    DustInEnchantVisuals(thrown, DustID.IchorTorch, 40, default, 1f);
                }
                if (player.HasBuff(BuffID.WeaponImbueCursedFlames))
                {
                    DustInEnchantVisuals(thrown, DustID.CursedTorch, 40, default, 1f);
                }
                if (player.HasBuff(BuffID.WeaponImbuePoison))
                {
                    DustInEnchantVisuals(thrown, DustID.Poisoned, 40, default, 1f);
                }
                if (player.HasBuff(BuffID.WeaponImbueVenom))
                {
                    DustInEnchantVisuals(thrown, DustID.Venom, 40, default, 1f);
                }
                if (player.HasBuff(BuffID.WeaponImbueNanites))
                {
                    DustInEnchantVisuals(thrown, DustID.HallowSpray, 40, default, 1f);
                }

                switch (player.GetModPlayer<WeaponImbuePlayer>().currentImbue)
                {
                    case WeaponImbuePlayer.Imbues.None:
                        {
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Brine:
                        {
                            DustInEnchantVisuals(thrown, DustID.Water_Snow, 40, default, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.ComaceraticBurn:
                        {
                            DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.DaylightOverload:
                        {
                            DustInEnchantVisuals(thrown, ModContent.DustType<RiftDust>(), 40, Color.White, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Defilement:
                        {
                            
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.FrostBite:
                        {
                            DustInEnchantVisuals(thrown, DustID.IceTorch, 40, default, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.FrostBurn:
                        {
                            DustInEnchantVisuals(thrown, DustID.IceTorch, 40, default, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.GalantineBurn:
                        {
                            ConstitutionParticle Particle = new();
                            Particle.Initialize(Main.rand.NextVector2FromRectangle(thrown.EnchantmentVisuals()), projectile.Center.DirectionTo(Utils.Center(thrown.EnchantmentVisuals())) * 3f, Main.rand.NextFloat(0.5f, 1f), 30);
                            ParticleEngine.ShaderParticles.Add(Particle);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.HeliouricShock:
                        {
                            DustInEnchantVisuals(thrown, DustID.FireworksRGB, 40, ColorLib.Rift, 0.5f, false);

                            ElectricArc Arc = new();
                            Arc.Create(Main.rand.NextVector2FromRectangle(thrown.EnchantmentVisuals()), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                            ParticleEngine.ShaderParticles.Add(Arc);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Hellfire:
                        {
                            DustInEnchantVisuals(thrown, DustID.Lava, 40, default, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Honey:
                        {
                            DustInEnchantVisuals(thrown, DustID.Honey, 40, default, 1f);
                            //Dust.NewDust(thrown.EnchantmentVisuals().TopLeft(), thrown.EnchantmentVisuals().Width, thrown.EnchantmentVisuals().Height, DustID.Honey, 0, 0, 40, default, 1.0f);
                            if (Main.rand.NextBool(10))
                            {
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.Bee, projectile.damage / 3, 4, projectile.owner);
                            }
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Mud:
                        {
                            DustInEnchantVisuals(thrown, DustID.Mud, 40, default, 1f);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.SpiritDrift:
                        {
                            
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.ShimmeringFlames:
                        {
                            Fire fire = new Fire();
                            fire.PrepareFire(Main.rand.NextVector2FromRectangle(thrown.EnchantmentVisuals()), projectile.Center.DirectionTo(Utils.Center(thrown.EnchantmentVisuals())) * 3f, DTUtils.RandomDirection(2), 0.1f, ColorLib.TenebrisGradient, Main.rand.NextFloat(0.5f, 1f), 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                            ParticleEngine.BehindProjectiles.Add(fire);
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.SoulInferno:
                        {
                            
                            break;
                        }
                    case WeaponImbuePlayer.Imbues.Withering:
                        {

                            break;
                        }
                }
            }
        }

        public void ApplyImbue(NPC target, Projectile projectile)
        {
            Player player = Main.player[projectile.owner];

            if (player.TryGetModPlayer<WeaponImbuePlayer>(out var imbuePlayer))
            {
                if (imbuePlayer.currentImbue != WeaponImbuePlayer.Imbues.ShimmeringFlames)
                {
                    target.AddBuff(imbuePlayer.GetImbueToBuff(), imbuePlayer.GetImbueTime());
                }
                else
                {
                    ShimmeringFlames.ShimmerBurn(target, true);
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyImbue(target, projectile);
        }
    }
}