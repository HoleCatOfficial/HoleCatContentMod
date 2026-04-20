using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
	public class ShimmeringShardCluster : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 34; // The width of projectile hitbox
			Projectile.height = 44; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.timeLeft = 2000; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}


        public void visuals(float rad)
        {
            var Positions = Opus.GetEquidistantOrbitVectors(6, Projectile.Center, 0.5f, rad);

            foreach(Vector2 Pos in Positions)
            {
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Pos, Vector2.Zero, ColorLib.TenebrisGradient, 0.75f, 30, ai2: 2);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Pos, Vector2.Zero, ColorLib.TenebrisGradient * 0.4f, 1.25f, 30, ai2: 2);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Pos, Vector2.Zero, ColorLib.TenebrisGradient * 0.5f, 1.25f);
            }
        }

        public void players(float rad)
        {
            foreach(Player player in Main.player)
            {
                if(player.active && player.Center.Distance(Projectile.Center) < rad && player.whoAmI == Projectile.owner)
                {
                    player.GetDamage<ScepterClass>() *= 1.4f;

                }
            }
        }

        public float Radius;
        public bool Flag1 = false;
		public override void AI()
		{
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 1)
            {
                Radius = 1;
            }
            if(Projectile.ai[0] > 1 && Radius < 400 && !Flag1)
            {
                Radius++;
            }
            if (Radius >= 800 && !Flag1)
            {
                Flag1 = true;
            }

            if (Flag1 == true & Projectile.ai[0] < 1200)
            {
                Radius = Opus.Sine(800f, 850f);
            }

            if(Projectile.ai[0] >= 1200 && Radius > 0)
            {
                Radius--;

                if(Radius <= 0)
                {
                    Projectile.Kill();
                }
            }

            visuals(Radius);
            players(Radius);
            
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Radius, Radius), DustID.TintableDustLighted, Vector2.Zero, 0, ColorLib.TenebrisGradient, 1f);
                dust.noGravity = true;
                dust.noLight = false;
            }

            
        }   
	}

}