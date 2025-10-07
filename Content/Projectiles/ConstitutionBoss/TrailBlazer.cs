using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
public class TrailBlazer : ModProjectile
		{
			// Store the target NPC using Projectile.ai[0]
			public Player HomingTarget {
				get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
				set {
					Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
				}
			}

			public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to Projectile projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            Main.projFrames[Type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = 68; // The width of projectile hitbox
			Projectile.height = 72; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
		}

            private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

			public int trailLength = 20;
			public override bool PreDraw(ref Color lightColor)
			{
                
				lightColor = ColorLib.StellarColor;

				SpriteBatch spriteBatch = Main.spriteBatch;
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				DTUtils Utility = new DTUtils();

            	Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
				int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
                Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, projectileTexture.Width, frameHeight);

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    sourceRect, // Use the correct frame
                    lightColor,
                    Projectile.rotation,
                    new Vector2(projectileTexture.Width / 2f, frameHeight / 2f),
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );

				
				Main.EntitySpriteDraw(
					DTAssetLib.PointGlow.Value,
					Projectile.Center - Main.screenPosition,
					null,
					lightColor,
					Projectile.rotation,
					DTAssetLib.PointGlow.Value.Size() / 2,
					Projectile.scale,
					SpriteEffects.None,
					0
				);

				Utility.ReturnToDefaultDrawing(spriteBatch);

				return false;
			}

		// Custom AI
		public override void AI()
		{
			AnimateProjectile();
			Player player = Main.LocalPlayer;
			Projectile.rotation = Projectile.velocity.ToRotation();
			float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

			// First, we find a homing target if we don't have one
			if (HomingTarget == null)
			{
				HomingTarget = FindPlayer(maxDetectRadius);
			}

			// If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
			if (HomingTarget != null && !IsValidTarget(HomingTarget))
			{
				HomingTarget = null;
			}

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			// If found, we rotate the projectile velocity in the direction of the target.
			// We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation();

			Vector2 vector2_2 = new Vector2(Projectile.spriteDirection == -1 ? -6f : -2f, -26f).RotatedBy((double)Projectile.rotation, new Vector2());
			if (Main.rand.Next(24) == 0)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center + vector2_2, 4, 4, DustID.TintableDustLighted, 0.0f, 0.0f, 100, ColorLib.StellarColor, 2f);
				if (Main.rand.Next(3) != 0)
				{
					dust.noGravity = true;
					dust.velocity.Y -= 3f;
					dust.noLight = true;
				}
				else if (Main.rand.Next(2) != 0)
				{
					dust.noLight = true;
					dust.velocity *= 0.5f;
					dust.velocity.Y -= 0.9f;
					dust.scale += (float)(0.100000001490116 + (double)Main.rand.NextFloat() * 0.600000023841858);
				}
			}
			
			DelegateMethods.v3_1 = new Vector3(0.3f, 0.5f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 6f, 20f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(Projectile.Left, Projectile.Right, 20f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(player.Center, player.Center + player.velocity * 6f, 40f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(player.Left, player.Right, 40f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
			}

			// Finding the closest NPC to attack within maxDetectDistance range
			// If not found then returns null
			public Player FindPlayer(float maxDetectDistance) {
				Player ClosestTarget = null;

				// Using squared values in distance checks will let us skip square root calculations, drastically improving Projectile method's speed.
				float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

				// Loop through all NPCs
				foreach (var targetplayer in Main.player) {
					// Check if NPC able to be targeted. 
					if (IsValidTarget(targetplayer)) {
						// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
						float sqrDistanceToTarget = Vector2.DistanceSquared(targetplayer.Center, Projectile.Center);

						// Check if it is within the radius
						if (sqrDistanceToTarget < sqrMaxDetectDistance) {
							sqrMaxDetectDistance = sqrDistanceToTarget;
							ClosestTarget = targetplayer;
						}
					}
				}

				return ClosestTarget;
			}

			public bool IsValidTarget(Player target) {
				// Projectile method checks that the NPC is:
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				// 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
				return target.active == true && target.statLife > 5 && target.MountedCenter.DistanceSQ(Projectile.Center) < 25000;
			}

       
    }
}