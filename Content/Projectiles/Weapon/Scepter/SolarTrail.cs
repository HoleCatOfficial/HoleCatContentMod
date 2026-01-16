
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
using System;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class SolarTrail : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
		
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
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
		}
		public override bool PreDraw(ref Color lightColor)
		{

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();
			float opacity = Projectile.Opacity;

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawTextureOnProj(DTAssetLib.PointGlow, Projectile, ColorLib.Rift * opacity, true, Projectile.rotation, Scale1, Scale1);
			Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5), Projectile, Color.White * opacity, false, 0f, Scale2, Scale2);
            

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public float Scale1 = 0f;
        public float Scale2 = 0f;
        public byte AlphaByte;
        
		public override void AI()
		{
            AlphaByte = (byte)Projectile.alpha;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

            Projectile.ai[1]++;

            Scale1 = Opus.Sine(0.5f, 0.8f, 0.01f);
            Scale2 = Opus.Sine(0.1f, 0.5f, 0.2f);

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3() * Scale2);

			if (Projectile.ai[1] > 200)
			{
				Projectile.Opacity -= 0.01f;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 600);
        }


    }
	
}