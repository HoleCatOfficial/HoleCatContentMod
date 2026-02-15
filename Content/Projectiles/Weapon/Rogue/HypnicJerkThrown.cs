using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
	public class HypnicJerkThrown : ModProjectile
	{
		private const int DefaultWidthHeight = 15;
		private const int ExplosionWidthHeight = 250;

		public override void SetStaticDefaults() {
			ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.

			// This set handles some things for us already:
			// Sets the timeLeft to 3 and the projectile direction when colliding with an NPC or player in PVP (so the explosive can detonate).
			// Explosives also bounce off the top of Shimmer, detonate with no blast damage when touching the bottom or sides of Shimmer, and damage other players in For the Worthy worlds.
			ProjectileID.Sets.Explosive[Type] = true;
		}

		public override void SetDefaults() 
        {
			// While the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
			Projectile.width = DefaultWidthHeight;
			Projectile.height = DefaultWidthHeight;
			Projectile.friendly = true;
			Projectile.penetrate = -1;

			// 5 second fuse.
			Projectile.timeLeft = 300;

		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
			// Vanilla explosions do less damage to Eater of Worlds in expert mode, so we will too.
			if (Main.expertMode) {
				if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail) {
					modifiers.FinalDamage /= 5;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) 
        {
			// This code makes the projectile very bouncy.
			if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f) 
            {
                Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 6, Projectile.Center, 1, Color.White, 1f, 2.7f);
				Projectile.velocity.X = oldVelocity.X * -0.4f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f) 
            {
                Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 6, Projectile.Center, 1, Color.White, 1f, 2.7f);
				Projectile.velocity.Y = oldVelocity.Y * -0.4f;
			}
			return false;
		}

		public override void AI() 
        {
			// The projectile is in the midst of exploding during the last 3 updates.
			if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) 
            {
				Projectile.PrepareBombToBlow(); // Get ready to explode.
			}
		
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 10f) {
				Projectile.ai[0] = 10f;
				// Roll speed dampening.
				if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f) {
					Projectile.velocity.X = Projectile.velocity.X * 0.96f;

					if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01) {
						Projectile.velocity.X = 0f;
						Projectile.netUpdate = true;
					}
				}
				// Delayed gravity
				Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			}
			// Rotation increased by velocity.X
			Projectile.rotation += Projectile.velocity.X * 0.1f;
		}

		public override void PrepareBombToBlow() {
			Projectile.tileCollide = false; // This is important or the explosion will be in the wrong place if the bomb explodes on slopes.
			Projectile.alpha = 255; // Set to transparent. This projectile technically lives as transparent for about 3 frames

			// Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
			Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

			Projectile.damage = 250; // Bomb: 100, Dynamite: 250
			Projectile.knockBack = 10f; // Bomb: 8f, Dynamite: 10f
		}

		public override void OnKill(int timeLeft) 
        {
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HypnicJerkExplosion>(), Projectile.damage, 2, Projectile.owner);
		}
	}

    public class HypnicJerkExplosion : Explosion
    {

        public override string Texture => DTUtils.NoTexture;
        public override float AreaOfEffect => 120;
        public override SoundStyle Sound => DTAssetLib.Impacts.ExplosiveImpactSmall;
        public override void OnExplode()
        {
            Opus.RingParticleOutward(PRTLoader.GetParticleID<StarParticle>(), 16, Projectile.Center, 22, 0f, Color.White, 0.75f, 3f, offset: Projectile.rotation);

        }
    }
}