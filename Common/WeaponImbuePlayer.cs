
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Comaceratic;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
	public class WeaponImbuePlayer : ModPlayer
    {
		public bool HeliouricShock = false;
        public bool DaylightOverload = false;
        public bool ComaceraticBurn = false;
        public bool GalantineBurn = false;
        public bool Honey = false;
        public bool Mud = false;
        public bool GalantineHoney = false;
        public bool Brine = false;
        public bool FrostBurn = false;
        public bool Fire = false;
        public bool Ichor = false;
        public bool CursedFlame = false;

        public override void ResetEffects()
        {
            HeliouricShock = false;
            DaylightOverload = false;
            ComaceraticBurn = false;
            GalantineBurn = false;
            Honey = false;
            Mud = false;
            GalantineHoney = false;
            FrostBurn = false;
            Fire = false;
            Ichor = false;
            CursedFlame = false;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {  
            if (HeliouricShock && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<HeliouricShock>(), 60 * Main.rand.Next(10, 17));
            }
            if (DaylightOverload && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<DaylightOverload>(), 60 * Main.rand.Next(10, 17));
            }
            if (ComaceraticBurn && item.DamageType.CountsAsClass<MeleeDamageClass>()) 
            {
				target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 60 * Main.rand.Next(10, 17));
			}
            if (GalantineBurn && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
            }
            if (Honey && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffID.Slow, 60 * Main.rand.Next(10, 17));
            }
            if (Mud && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<Muddy>(), 60 * Main.rand.Next(10, 17));
            }
            if (GalantineHoney && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
                target.AddBuff(BuffID.Slow, 60 * Main.rand.Next(10, 17));
            }
            if (FrostBurn && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(10, 17));
            }
            if (Fire && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(10, 17));
            }
            if (Ichor && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 17));
            }
            if (CursedFlame && item.DamageType.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(10, 17));
            }
		}

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (GalantineBurn && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments)
            {
                target.AddBuff(ModContent.BuffType<GalantineBurn>(), 60 * Main.rand.Next(10, 17));
            }
            if (HeliouricShock && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments)
            {
                target.AddBuff(ModContent.BuffType<HeliouricShock>(), 60 * Main.rand.Next(10, 17));
            }
            if (DaylightOverload && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments) {
				target.AddBuff(ModContent.BuffType<DaylightOverload>(), 60 * Main.rand.Next(10, 17));
			}
            if (ComaceraticBurn && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments) {
				target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 60 * Main.rand.Next(10, 17));
			}
		}

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (HeliouricShock && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(5))
                {
                    ElectricArc Arc = new();
                    Arc.Create(new Vector2(hitbox.Width, (hitbox.Height / 2) - (hitbox.Height / 2)), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                    ParticleEngine.ShaderParticles.Add(Arc);
                }
            }
            if (GalantineBurn && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.TintableDustLighted, 0, 0, 100,  ColorLib.StellarFireGradientLooping(), 1.85f);
                    dust.velocity *= 0.5f;
                }
            }
            if (DaylightOverload && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<RiftDust>());
                    dust.velocity *= 0.5f;
                }
            }
            if (ComaceraticBurn && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic) {
                if (Main.rand.NextBool(5)) {
                    //Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<RiftDust>());
                    //dust.velocity *= 0.5f;

                    Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, ColorLib.Rift, 1);
                    StarParticle Star = new();
                    Star.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(1f, 1f), ColorLib.LightRift1, Main.rand.NextFloat(0.1f, 0.6f));
                    ParticleEngine.ShaderParticles.Add(Star);
           

                    if (Main.rand.NextBool(8))
                    {
                        ComaceraticParticle FX = new();
                        FX.Initialize(Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(2f, 2f), ColorLib.LightRift2, Main.rand.NextFloat(0.05f, 0.1f));
                        ParticleEngine.ShaderParticles.Add(FX);
                    }
                }
			}
		}

        public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
        {
           
            if (HeliouricShock && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            {
                if (Main.rand.NextBool(5))
                {
                    ElectricArc Arc = new();
                    Arc.Create(Main.rand.NextVector2FromRectangle(new Rectangle((int)projectile.position.X, (int)projectile.position.Y, boxWidth, boxHeight)), ColorLib.Rift, Main.rand.NextFloat(0.5f, 1f), 0.08f);
                    ParticleEngine.ShaderParticles.Add(Arc);
                }
            }
            if (DaylightOverload && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, ModContent.DustType<RiftDust>());
                    dust.velocity *= 0.5f;
                }
            }
            if (ComaceraticBurn && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments) {
				if (Main.rand.NextBool(5)) {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<ColorableNeonDust>(), 0.0f, 0.5f, 0, ColorLib.Rift, 1);

                    StarParticle Star = new();
                    Star.Initialize(Main.rand.NextVector2FromRectangle(projectile.Hitbox), Main.rand.NextVector2Circular(1f, 1f), ColorLib.LightRift1, Main.rand.NextFloat(0.1f, 0.6f));
                    ParticleEngine.ShaderParticles.Add(Star);

                    if (Main.rand.NextBool(8))
                    {
                        ComaceraticParticle FX = new();
                        FX.Initialize(Main.rand.NextVector2FromRectangle(projectile.Hitbox), Main.rand.NextVector2Circular(2f, 2f), ColorLib.LightRift2, Main.rand.NextFloat(0.05f, 0.1f));
                        ParticleEngine.ShaderParticles.Add(FX);
                    }
                }
			}
		}
	}
}