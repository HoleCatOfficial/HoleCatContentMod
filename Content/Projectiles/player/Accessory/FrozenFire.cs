using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class FrozenFire : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.1f;
			Projectile.timeLeft = 60;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		
		
		public override void OnSpawn(IEntitySource source)
		{
			
		}

        Color Ice = new Color(40, 152, 240);
		public override void AI()
		{

			Lighting.AddLight(Projectile.Center, Ice.ToVector3() * 0.2f);

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, Ice, 1f, 100, FireDrawMode.Additive);
            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, Main.rand.Next(1, 3), 0.1f, Ice * 0.5f, 1.5f, 100, FireDrawMode.Additive);

            ParticleEngine.BehindProjectiles.Add(fire);
            ParticleEngine.BehindProjectiles.Add(fire2);

			PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Vector2.Zero, Ice * 0.25f, 1.25f);
		}
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Frostburn, 300);
		}
    }
}