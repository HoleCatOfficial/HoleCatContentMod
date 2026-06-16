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
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class StardustDartSmall : ModProjectile, IDrawPixelated, IHomingProjectile
	{
		public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 12;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.05f;

        float IHomingProjectile.HomingMaxAccel => 24f;

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
			Projectile.tileCollide = false;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(0, 174, 238);
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
			return false;
		}


		public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 35;
        }

		public override void AI()
		{
			Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, ColorLib.Stardust.ToVector3() * 0.005f);

            if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.StardustMinionBleed, 300);
		}

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.01f;

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(1, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, ColorLib.Stardust, trailOffset, 10);
        }
    }
}