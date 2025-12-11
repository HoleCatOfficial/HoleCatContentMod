using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.CurseRunes;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class TenebrousLightDart : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true; 
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ColorLib.TenebrisGradient;
			
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();
            Effect effect = DTAssetLib.TrailScroller.Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

                effect.Parameters["TotalTime"].SetValue((float)Main.GameUpdateCount / 60f);
                effect.Parameters["ScrollSpeed"].SetValue(1.0f);

                

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 16;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 16;

					ve.Add(new ColoredVertex(
						TrailPositions[i] - Main.screenPosition + offset,
						new Vector3(t, 1, 1),
						b));

					ve.Add(new ColoredVertex(
						TrailPositions[i] - Main.screenPosition + offset2,
						new Vector3(t, 0, 1),
						b));
				}

				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(7).Value;
                    effect.CurrentTechnique.Passes["ScrollingUV"].Apply();
                    spriteBatch.End();
                    spriteBatch.Begin(0, BlendState.Additive, spriteBatch.GraphicsDevice.SamplerStates[0], spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, effect, Main.GameViewMatrix.TransformationMatrix);
                
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				}
			}
        
			Opus.DrawGlowOnProj(Projectile, lightColor, true);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10;
        }

        private List<Vector2> TrailPositions = new List<Vector2>();
        public List<float> TrailRotations = new();
        private const int TrailLength = 40;

        public override void AI()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 8f;

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);
			}


			// Cap trail
			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.6f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            float maxDetectRadius = 4000f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            int turnspeed = 5;
            turnspeed += 10;
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(turnspeed)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }


        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            return target.CanBeChasedBy();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown);
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 480);
        }
	}
}