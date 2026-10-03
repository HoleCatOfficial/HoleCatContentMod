using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using DestroyerTest.Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace DestroyerTest.Content.Projectiles.EntitiesProjectiles
{
	public class PosessedScepterSoulShot : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.hostile = true;
			Projectile.timeLeft = 240;
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
			trailOffset -= 0.01f;
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTTrail.DrawTrail(spriteBatch, DTAssetLib.SoulStreak.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 12f, ColorLib.PossessedScepterColor, trailOffset);

            
			Opus.DrawTextureOnProj(DTAssetLib.PointGlowPreMultiplied, Projectile, DTColorUtils.Pastel(ColorLib.PossessedScepterColor, 0.4f) with { A = 0 }, false, 0f, 0.7f, 0.7f);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, ColorLib.PossessedScepterColor with { A = 0 }, false, 0f, S, S);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0 }, false, 0f, S * 0.6f, S * 0.6f);
			return false;
        }

	
     
		public float S = 0f;
		public override void AI() 
		{
			Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation();
			Lighting.AddLight(Projectile.Center, ColorLib.PossessedScepterColor.ToVector3() * 0.25f);
			S = Opus.Sine(0.75f, 1f);
		}

		public override void OnKill(int timeLeft) 
		{

		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SpiritDrift>(), 300);
        }
    }
}