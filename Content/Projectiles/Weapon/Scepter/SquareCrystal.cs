
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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class SquareCrystal : ModProjectile
	{
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		
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
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			}
		public int trailLength = 10;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.Red;

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5), Projectile, Color.White, true, Projectile.rotation, ShineScale, ShineScale);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				Color.White,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false;
		}

        public float ShineScale = 0f;
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

			Projectile.velocity *= 0.96f;

            Projectile.ai[1]++;

            if (Projectile.ai[1] > 120)
            {
                ShineScale += 0.01F;
            }

			if (Projectile.ai[1] > 200)
			{
				Projectile.Center += Main.rand.NextVector2Circular(3, 3);
			}
		}

        public override void OnKill(int timeLeft)
        {
			//Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.05f, 2f);
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Impacts/IceMagicImpact", 3) with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Opus.RadialSpreadProjectile(ModContent.ProjectileType<HeliciteDart>(), 4, Projectile.Center, Projectile.damage, 4, 8, offset: Projectile.rotation);
        }


    }
	
}