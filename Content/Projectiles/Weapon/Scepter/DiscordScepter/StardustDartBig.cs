using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class StardustDartBig : ModProjectile, IDrawPixelated, IHomingProjectile
	{

		public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => true;

        float IHomingProjectile.HomingTurnSpeed => 8f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer >= 35;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 150;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

		

			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
			return false;
		}


        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 35 && Projectile.ManualCanHitFriendly(target);
        }


		public override void AI()
		{
			Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Lighting.AddLight(Projectile.Center, ColorLib.Stardust.ToVector3() * 0.01f);

			if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (timeLeft > 0)
			{
				SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact, Projectile.Center);
				Opus.RadialSpreadProjectile(ModContent.ProjectileType<StardustDartSmall>(), 8, Projectile.Center, Projectile.damage / 2, 0, 4, offset: Main.rand.NextFloat(MathHelper.TwoPi));
			}
		}

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.01f;

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 48, ColorLib.Stardust, trailOffset, 10);
        }
    }
}