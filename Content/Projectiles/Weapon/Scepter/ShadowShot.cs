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
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class ShadowShot : ModProjectile
	{

		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 2;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10; // The width of projectile hitbox
			Projectile.height = 22; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 60; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
		}


		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(179, 54, 201);

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			Opus.DrawGlowOnProj(Projectile, lightColor, false, 0);

			for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(1f, 0.0005f, progress);
                Color color = lightColor;

                Main.EntitySpriteDraw(
                    projectileTexture,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
                    Projectile.rotation,
                    projectileTexture.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}
		
		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;

		public override void AI() {
			TrailPositions.Insert(0, Projectile.Center);
			TrailRotations.Insert(0, Projectile.rotation);

			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);
				
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.ShadowFlame, 600);
		}

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/CursedFlamesWarn") with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowflameAOE>(), Projectile.damage, 0, Projectile.owner);
        }


    }
}