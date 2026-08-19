
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Comaceratic;
using DestroyerTest.Content.Particles.Stellar;
using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
	public class WeaponImbuePlayer : ModPlayer
    {

        public enum Imbues
        {
            None,
            Brine,
            ComaceraticBurn,
            DaylightOverload,
            Defilement,
            FrostBite,
            FrostBurn,
            GalantineBurn,
            HeliouricShock, 
            Hellfire,
            Honey,
            Mud,
            SpiritDrift,
            ShimmeringFlames,
            SoulInferno,
            Withering
        }

        public Imbues currentImbue;

        public override void ResetEffects()
        {
            currentImbue = Imbues.None;
        }

        public int GetImbueTime()
        {
            int time;

            if (DestroyerTestMod.EternityIsActive || DestroyerTestMod.DeathIsActive)
                time = 60 * 35;
            else
                time = 60 * 20;

            return time;
        }

        public int GetImbueToBuff()
        {
            switch (currentImbue)
            {
                case Imbues.None:
                    {
                        return 0;
                    }
                case Imbues.Brine:
                    {
                        return ModContent.BuffType<Brine>();
                    }
                case Imbues.ComaceraticBurn:
                    {
                        return ModContent.BuffType<ComaceraticBurn>();
                    }
                case Imbues.DaylightOverload:
                    {
                        return ModContent.BuffType<DaylightOverload>();
                    }
                case Imbues.Defilement:
                    {
                        return ModContent.BuffType<Defilement>();
                    }
                case Imbues.FrostBite:
                    {
                        return BuffID.Frostburn2;
                    }
                case Imbues.FrostBurn:
                    {
                        return BuffID.Frostburn;
                    }
                case Imbues.GalantineBurn:
                    {
                        return ModContent.BuffType<GalantineBurn>();
                    }
                case Imbues.HeliouricShock:
                    {
                        return ModContent.BuffType<HeliouricShock>();
                    }
                case Imbues.Hellfire:
                    {
                        return BuffID.OnFire3;
                    }
                case Imbues.Honey:
                    {
                        //I dont feel like making a clone of muddy. This is basically the same thing.
                        return BuffID.Slow;
                    }
                case Imbues.Mud:
                    {
                        return ModContent.BuffType<Muddy>();
                    }
                case Imbues.SpiritDrift:
                    {
                        return ModContent.BuffType<SpiritDrift>();
                    }
                case Imbues.ShimmeringFlames:
                    {
                        return 0;
                    }
                case Imbues.SoulInferno:
                    {
                        return ModContent.BuffType<SoulInferno>();
                    }
                case Imbues.Withering:
                    {
                        return ModContent.BuffType<Withering>();
                    }
            }
            return 0;
        }

        public void ApplyImbue(NPC target)
        {
            if (currentImbue != Imbues.ShimmeringFlames)
            {

                int buff = GetImbueToBuff();
                int time = GetImbueTime();

                target.AddBuff(buff, time);
            }
            else
            {
                ShimmeringFlames.ShimmerBurn(target, true);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                ApplyImbue(target);
            }


		}

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type] && !proj.noEnchantments)
            {
                ApplyImbue(target);
            }
		}

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (item.DamageType.CountsAsClass(DamageClass.Melee))
            {
                switch (currentImbue)
                {
                    case Imbues.None:
                        {
                            break;
                        }
                    case Imbues.Brine:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Water_Snow);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.ComaceraticBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {

                                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, ColorLib.Rift, 1);
                                StarParticle Star = new();
                                Star.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(1f, 1f), ColorLib.LightRift1, Main.rand.NextFloat(0.1f, 0.6f));
                                ParticleEngine.BehindProjectiles.Add(Star);


                                if (Main.rand.NextBool(8))
                                {
                                    ComaceraticParticle FX = new();
                                    FX.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(2f, 2f), ColorLib.LightRift2, Main.rand.NextFloat(0.05f, 0.1f));
                                    ParticleEngine.BehindProjectiles.Add(FX);
                                }
                            }
                            break;
                        }
                    case Imbues.DaylightOverload:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<RiftDust>());
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Defilement:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                DamnationParticle.Create(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextFloat(1f, 2f), 30, PixelLayer.AboveTiles);
                            }
                            break;
                        }
                    case Imbues.FrostBite:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.IceTorch);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.FrostBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.IceTorch);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.GalantineBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                ConstitutionParticle Particle = new();
                                Particle.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextFloat(1f, 2f), 30);
                                ParticleEngine.BehindProjectiles.Add(Particle);
                            }
                            break;
                        }
                    case Imbues.HeliouricShock:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                ElectricArc Arc = new();
                                Arc.Create(new Vector2(hitbox.Width, (hitbox.Height / 2) - (hitbox.Height / 2)), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                                ParticleEngine.ShaderParticles.Add(Arc);
                            }
                            break;
                        }
                    case Imbues.Hellfire:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Lava);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Honey:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Honey);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Mud:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Mud);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.SpiritDrift:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.BlueMoss);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.ShimmeringFlames:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Fire fire = new();
                                fire.PrepareFire(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Player.direction, 0.06f, ColorLib.TenebrisGradient, Main.rand.NextFloat(1f, 2f), 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                                ParticleEngine.BehindProjectiles.Add(fire);
                            }
                            break;
                        }
                    case Imbues.SoulInferno:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<SoulDust>());
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Withering:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                WitheringSpark Particle = new();
                                Particle.PrepareSpark(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), 0f, Color.DeepPink, Main.rand.NextFloat(1f, 2f), false, 30, SparkDrawMode.NonPremultiplied, 2f);
                                ParticleEngine.BehindProjectiles.Add(Particle);
                            }
                            break;
                        }
                }
            }

		}

        public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
        {
            Rectangle hitbox = new Rectangle((int)boxPosition.X, (int)boxPosition.Y, boxWidth, boxHeight);

            if (projectile.friendly && (projectile.DamageType.CountsAsClass(DamageClass.Melee) || projectile.DamageType.CountsAsClass(DamageClass.MeleeNoSpeed) || projectile.DamageType.CountsAsClass<ScepterClass>()))
            {
                switch (currentImbue)
                {
                    case Imbues.None:
                        {
                            break;
                        }
                    case Imbues.Brine:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Water_Snow);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.ComaceraticBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {

                                Dust.NewDust(boxPosition, boxWidth, boxHeight, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, ColorLib.Rift, 1);
                                StarParticle Star = new();
                                Star.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(1f, 1f), ColorLib.LightRift1, Main.rand.NextFloat(0.1f, 0.6f));
                                ParticleEngine.BehindProjectiles.Add(Star);


                                if (Main.rand.NextBool(8))
                                {
                                    ComaceraticParticle FX = new();
                                    FX.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(2f, 2f), ColorLib.LightRift2, Main.rand.NextFloat(0.05f, 0.1f));
                                    ParticleEngine.BehindProjectiles.Add(FX);
                                }
                            }
                            break;
                        }
                    case Imbues.DaylightOverload:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, ModContent.DustType<RiftDust>());
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Defilement:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                DamnationParticle.Create(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextFloat(1f, 2f), 30, PixelLayer.AboveTiles);
                            }
                            break;
                        }
                    case Imbues.FrostBite:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.IceTorch);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.FrostBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.IceTorch);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.GalantineBurn:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                ConstitutionParticle Particle = new();
                                Particle.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextFloat(1f, 2f), 30);
                                ParticleEngine.BehindProjectiles.Add(Particle);
                            }
                            break;
                        }
                    case Imbues.HeliouricShock:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                ElectricArc Arc = new();
                                Arc.Create(new Vector2(hitbox.Width, (hitbox.Height / 2) - (hitbox.Height / 2)), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                                ParticleEngine.ShaderParticles.Add(Arc);
                            }
                            break;
                        }
                    case Imbues.Hellfire:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Lava);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Honey:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Honey);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Mud:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Mud);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.SpiritDrift:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.BlueMoss);
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.ShimmeringFlames:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Fire fire = new();
                                fire.PrepareFire(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), Player.direction, 0.06f, ColorLib.TenebrisGradient, Main.rand.NextFloat(1f, 2f), 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                                ParticleEngine.BehindProjectiles.Add(fire);
                            }
                            break;
                        }
                    case Imbues.SoulInferno:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, ModContent.DustType<SoulDust>());
                                dust.velocity *= 0.5f;
                            }
                            break;
                        }
                    case Imbues.Withering:
                        {
                            if (Main.rand.NextBool(5))
                            {
                                WitheringSpark Particle = new();
                                Particle.PrepareSpark(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3f, 3f), 0f, Color.DeepPink, Main.rand.NextFloat(1f, 2f), false, 30, SparkDrawMode.NonPremultiplied, 2f);
                                ParticleEngine.BehindProjectiles.Add(Particle);
                            }
                            break;
                        }
                }
            }
        }
	}
}