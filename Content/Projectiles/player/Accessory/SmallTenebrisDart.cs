using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class SmallTenebrisDart : ModProjectile
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
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[Type] = 3;
        }

        public int Variant = Main.rand.Next(3);
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.frame = Variant;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = c;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);
            if (TrailPositions.Count > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();

                for (int i = TrailPositions.Count - 1; i > 0; i--)
                {
                    float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
                    Color b = lightColor * t;

                    Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
                    Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 5;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 5;
                        
                    DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, 0f);
                }


                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = DTAssetLib.Square.Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
			Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 200;
        
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 1f; // how closely to sample. tweak this!

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
        }
        public Color c;
        public override void AI()
        {
            if (Variant == 0)
            {
                c = ColorLib.TenebrisMagenta;
            }
            if (Variant == 1)
            {
                c = ColorLib.TenebrisBlue;
            }
            if (Variant == 2)
            {
                c = ColorLib.TenebrisBeige;
            }

            CacheTrail();
            Projectile.rotation = Projectile.velocity.ToRotation();
            float maxDetectRadius = 400f;

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

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
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(4)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
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
            ShimmeringFlames.ShimmerBurn(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            if (Variant == 0)
            {
                List<Vector2> Star1 = Polar.GenerateCurvedStar(4, 3, 10, Projectile.Center, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p1 in Star1)
                {
                    Vector2 Vel = p1 - Projectile.Center;
                    Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Vel, 0, c, 1f);
                }
            }
            if (Variant == 1)
            {
                List<Vector2> Star2 = Polar.GenerateCurvedStar(5, 4, 10, Projectile.Center, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p2 in Star2)
                {
                    Vector2 Vel = p2 - Projectile.Center;
                    Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Vel, 0, c, 1f);
                }
            }
            if (Variant == 2)
            {
                List<Vector2> Star3 = Polar.GenerateCurvedStar(6, 5, 10, Projectile.Center, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                foreach (Vector2 p3 in Star3)
                {
                    Vector2 Vel = p3 - Projectile.Center;
                    Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Vel, 0, c, 1f);
                }
            }
        }
    }
}