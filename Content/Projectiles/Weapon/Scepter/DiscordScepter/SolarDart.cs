using System.Collections.Generic;
using System.IO;
using System.Linq;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter
{
	public class SolarDart : ModProjectile, IDrawPixelated, IHomingProjectile
	{

		public ref float DelayTimer => ref Projectile.ai[1];

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveProjectiles;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 8;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.05f;

        float IHomingProjectile.HomingMaxAccel => 24f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer >= 35;

        public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 150;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true; 
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.damage += 15;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			

			Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
			return false;
		}

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            trailOffset += 0.01f;

			DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(12, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, ColorLib.Solar, trailOffset, 10);
        }
		public override bool? CanHitNPC(NPC target)
		{
			return DelayTimer >= 35 && Projectile.ManualCanHitFriendly(target);
		}

		public override void AI() 
		{
			Projectile.ResetExcessTrailPoints();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, ColorLib.Solar.ToVector3() * 0.01f);

            if (DelayTimer < 35)
			{
				DelayTimer++;
				return;
			}
		}
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, Projectile.damage / 2, 3, Projectile.owner);
			target.AddBuff(BuffID.Daybreak, 300);
		}
    }
}