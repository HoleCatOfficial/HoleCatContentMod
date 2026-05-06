using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HekateStaffEmber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            Lighting.AddLight(Projectile.Center, new Color(184, 45, 117).ToVector3() * 0.6f);

            PointGlowPreMultiplied Particle = new PointGlowPreMultiplied();

            Particle.Initialize(Projectile.Center, new Vector2((Projectile.velocity.X / 2) + Main.rand.NextFloat(-1, 1), (Projectile.velocity.Y / 2) + Main.rand.NextFloat(-1, 1)), new Color(184, 45, 117), 0.25f);

            ParticleEngine.Particles.Add(Particle);

            if (player.channel)
            {
                Projectile.timeLeft = 120;
                return;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Impacts/ExplosiveImpactSmall") { PitchVariance = 0.2f, MaxInstances = 0 });
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HekateStaffExplosion>(), Projectile.damage, 12, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 20 * 60);
        }
	}
}