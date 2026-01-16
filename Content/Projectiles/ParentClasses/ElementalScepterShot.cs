using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using OpusLib;
using System.Collections.Generic;
using ReLogic.Content;
using DestroyerTest.Content.Equips.ScepterAccessories;
using System.Linq;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public abstract class ElementalScepterShot : ModProjectile
    {
        private NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public ref float DelayTimer => ref Projectile.ai[1];

        /// <summary>
        /// Color for drawing the trail. Defaults to the theme color.
        /// </summary>
        public Color TrailColor = Color.White;
        /// <summary>
        /// The Color to apply the 
        /// </summary>
        public Color DustColor = Color.White;
        /// <summary>
        /// The dust that is created by the projectile as it travels.
        /// </summary>
        public int TravelDust = DustID.WhiteTorch;
        /// <summary>
        /// The Dust that is created by the projectile when bouncing off of a tile.
        /// </summary>
        public int BounceDust = DustID.WhiteTorch;
        /// <summary>
        /// The Dust that is created by the projectile on death by tile.
        /// </summary>
        public int KillDust = DustID.WhiteTorch;

        /// <summary>
        /// The Sound that plays when bouncing off of a tile.
        /// </summary>
        public SoundStyle BounceSound = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/Tink1") with { PitchVariance = 1f, MaxInstances = 0 };

        /// <summary>
        /// The debuff the projectile applies to enemies and players
        /// </summary>
        public int Debuff = -1;

        /// <summary>
        /// The time that the debuff is applied for.
        /// </summary>
        public int DebuffTime = 300;

        /// <summary>
        /// The Radius at which the projectile will detect enemies when homing.
        /// </summary>
        public float DetectionRad = 400;

        public int TrailType = 1;
        public float TrailAmplitude = 20f;
        public float TrailScroll = 0.04f;
        


        public override void SetStaticDefaults() {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            ScepterRegistry.AllScepterShots.Add(Projectile);
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Projectile.GetAlpha(TrailColor);
			trailOffset += TrailScroll;

			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * TrailAmplitude;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * TrailAmplitude;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(TrailType).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(0.5f, 1 + (0.1f * Projectile.velocity.Length())), SpriteEffects.None, 0);

			return false;
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

        public void DustSpawn1()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, -8).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, 8).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            Dust.NewDustPerfect(DustPos, TravelDust, Vector2.Zero, 0, default, 0.75f);
        }

        public void DustSpawn2()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, 8).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, -8).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            Dust.NewDustPerfect(DustPos, TravelDust, Vector2.Zero, 0, default, 0.75f);
        }

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            CacheTrail();
            DustSpawn1();
            DustSpawn2();
            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(8))
                {
                    Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, TravelDust, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, DustColor, 1f);
                }
            }
            
            if (HomingTarget == null) {
                HomingTarget = FindClosestNPC(DetectionRad);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 1.01f;
        }

        public NPC FindClosestNPC(float maxDetectDistance) {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs) {
                if (IsValidTarget(target)) {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (sqrDistanceToTarget < sqrMaxDetectDistance) {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target) {
            if (Projectile.tileCollide == true)
            {
                return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
            }
            else
            {
                return target.CanBeChasedBy();
            }
        }

        public void AccessoryHandler_ChlorophyteLifesteal(ref int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (!player.TryGetModPlayer<LivingPendantPlayer>(out var Pendant))
            {
                return;
            }
            else
            {
                if (!Pendant.Active)
                {
                    return;
                }
                else
                {
                    player.Heal((int)(damageDone * 0.05f));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Debuff != -1)
            {
                target.AddBuff(Debuff, DebuffTime);
            }
            AccessoryHandler_ChlorophyteLifesteal(ref damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(Debuff, DebuffTime);
        }
    }
}