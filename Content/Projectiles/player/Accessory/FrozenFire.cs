using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using InnoVault.PRT;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;

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

			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero, Ice, 0.5f, 40, ai2: 2);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero, Ice * 0.3f, 1.25f, 40, ai2: 2);
			PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Projectile.Center, Vector2.Zero, Ice * 0.25f, 1.25f);
		}
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Frostburn, 300);
		}
    }
}