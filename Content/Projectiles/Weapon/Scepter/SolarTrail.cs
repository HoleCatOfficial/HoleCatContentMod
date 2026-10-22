
using System;
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
 
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class SolarTrail : ModProjectile, IDrawPixelated
	{
        public override string Texture => DTUtils.NoTexture;
		
		public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
		}

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;
        bool IDrawPixelated.ShouldDrawPixelated => true;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            
        }
		public override bool PreDraw(ref Color lightColor)
		{
            DTUtils Utility = new DTUtils();
            float opacity = Projectile.Opacity;

           // Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            Opus.DrawTextureOnProj(DTAssetLib.PointGlowPreMultiplied, Projectile, OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap) with { A = 0 } * opacity, true, Projectile.rotation, Scale1, Scale1);
            Opus.DrawTextureOnProj(DTAssetLib.Sparkle(5, true), Projectile, Color.White with { A = 0 } * opacity, false, 0f, Scale2, Scale2);

            //Opus.ReturnToDefaultDrawing(spriteBatch);

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

            Lighting.AddLight(Projectile.Center, OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap).ToVector3() * Scale2);

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