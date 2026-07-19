
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Terraria.Audio;
using OpusLib.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class HolyOrb : ModProjectile
	{
		
		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();

			Opus.DrawGlowOnProj(Projectile, Color.Red with { A = 0 }, false, 0);

			for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = i / (float)Projectile.oldPos.Length;
                float scale = MathHelper.Lerp(1f, 0.0005f, progress);
                Color color = Color.Red with { A = 0 };

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.OldCenter()[i] - Main.screenPosition,
                    null,
                    color,
                    Projectile.rotation,
                    projectileTexture.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				Color.Red,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false;
		}

		public override void AI()
		{

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

			Projectile.velocity *= 0.95f;

			if (Projectile.velocity.Length() < 0.005f)
			{
				Projectile.Kill();
			}
		}

        public override void OnKill(int timeLeft)
        {
			BloomRingSharp Ring = new();
			Ring.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.08f, 0.01f, 1f, BlendState.Additive);
			ParticleEngine.BehindProjectiles.Add(Ring);


			
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Impacts/IceMagicImpact", 3) with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Opus.RadialSpreadProjectile(ModContent.ProjectileType<Condemnation>(), 4, Projectile.Center, Projectile.damage, 4, 2f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
        }


    }
	
}