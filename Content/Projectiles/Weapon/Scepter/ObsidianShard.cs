using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class ObsidianShard : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox
			Projectile.friendly = true;
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, 1f, SpriteEffects.None, 0);
            return false;
        }


		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrousKatana/GoreSlice", 2) with { PitchVariance = 0.5f });
			for(int i = 0; i < 5; i++)
			{
				Dust.NewDustPerfect(target.Center, DustID.Blood, Projectile.velocity.RotatedByRandom(0.5f) * 0.5f, 0, default, 2f);
				PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), target.Center, Projectile.velocity.RotatedByRandom(0.5f), Color.Red, 1f, 1);
			}
            target.AddBuff(BuffID.Bleeding, 600);
        }


		public override void OnKill(int timeLeft) {
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Obsidian);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			} 
		}
	}
}