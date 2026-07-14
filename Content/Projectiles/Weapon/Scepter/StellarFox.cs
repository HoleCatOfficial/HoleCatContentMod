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
using Terraria.Graphics.Shaders;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Buffs;
using System;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using OpusLib.Content.Particles;
using Terraria.Graphics.Renderers;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class StellarFox : ModProjectile
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
            Projectile.width = 24;
            Projectile.height = 24;

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
			lightColor = TrailColor;
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
					Color b = GetTrailColor(i);
                    b *= t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * TrailAmplitude;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * TrailAmplitude;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Streak(2).Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.DrawGlowOnProj(Projectile, lightColor, true);

            Opus.DrawProjectileShadowsRotating(Projectile, 6, lightColor, 0.3f);

			Opus.ReturnToDefaultDrawing(spriteBatch);

            SpriteEffects effects = SpriteEffects.None;

            if (Projectile.direction == -1)
            {
                effects = SpriteEffects.FlipVertically;
            }
            else
            {
                effects = SpriteEffects.None;
            }

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, 1f, effects, 0);

			return false;
		}

        public List<Color> StellarFireColormap = new List<Color>
        {
            Color.White,
            ColorLib.StellarFire1,
            ColorLib.StellarFire2,
            ColorLib.StellarFire3,
            ColorLib.StellarFire4,
            ColorLib.StellarFire5,
            ColorLib.StellarFire6,
            ColorLib.StellarFire7,
            ColorLib.StellarFire8
        };

        private Color GetTrailColor(int index)
        {
            if (TrailPositions.Count <= 1)
                return StellarFireColormap[0];

            float t = index / (float)(TrailPositions.Count - 1);
            t = MathHelper.Clamp(t, 0f, 1f);

            float scaled = t * (StellarFireColormap.Count - 1);
            int low = (int)scaled;
            int high = Math.Min(low + 1, StellarFireColormap.Count - 1);

            float lerp = scaled - low;
            return Color.Lerp(StellarFireColormap[low], StellarFireColormap[high], lerp);
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

            ConstitutionParticle trail = new();
            trail.Initialize(DustPos, Projectile.velocity * 0.05f, 0.75f, 30);
            ParticleEngine.BehindProjectiles.Add(trail);
        }

        public void DustSpawn2()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, 8).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, -8).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            ConstitutionParticle trail = new();
            trail.Initialize(DustPos, Projectile.velocity * 0.05f, 0.75f, 30);
            ParticleEngine.BehindProjectiles.Add(trail);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI() 
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            CacheTrail();
            DustSpawn1();
            DustSpawn2();
            
            if(DelayTimer < 20)
            {
                DelayTimer++;
                return;
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
            int turn = 20;
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(turn)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.08f;
            if (Main.GameUpdateCount % 3 == 0)
            {
                turn++;
            }

            Projectile.velocity.Clamp(30);
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

        public bool IsValidTarget(NPC target) 
        {
            if (Projectile.tileCollide == true)
            {
                return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
            }
            else
            {
                return target.CanBeChasedBy();
            }
        }

        public void Explosion()
        {
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BoomCloud>(), Projectile.Center, Vector2.Zero, ColorLib.StellarFire5, 0.01f, 1.5f);
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BoomCloud>(), Projectile.Center, Vector2.Zero, ColorLib.StellarFire3, 0.01f, 1.0f);
            //Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BoomCloud>(), Projectile.Center, Vector2.Zero, ColorLib.StellarFire1, 0.01f, 0.7f);

            Vector2[] Dirs = Opus.RadialVectorOutwardRandom(20, Projectile.Center, Main.rand.NextFloat(2f, 5f));

            for (int i = 0; i < Dirs.Length; i++)
            {
                LerpingFire fire = new ();
                fire.PrepareFire(Projectile.Center, Dirs[i], 0f, ColorLib.StellarFireColormap, 1f, 50, FireDrawMode.Additive);

            }
            DTUtils.ConstitutionStarExplosionEffects(Projectile);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            SoundEngine.PlaySound(DTAssetLib.Impacts.StellarFox with { PitchVariance = 0.3f, MaxInstances = 0, Volume = 0.5f }, Projectile.Center);
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            Explosion();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(10, 10);
        }
    }
}