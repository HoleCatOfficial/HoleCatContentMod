using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
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
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = 11;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            Lighting.AddLight(Projectile.Center, new Color(184, 45, 117).ToVector3() * 0.6f);

            PointGlowPreMultiplied Particle = new PointGlowPreMultiplied();
            Particle.Initialize(Projectile.Center, Main.rand.NextVector2Circular(0.3f, 0.3f), new Color(184, 45, 117), 0.5f);
            ParticleEngine.Particles.Add(Particle);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/SeedStick") { PitchVariance = 0.2f, MaxInstances = 0 });
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HekateStaffExplosion>(), Projectile.damage, 12, Projectile.owner);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        
	}
}