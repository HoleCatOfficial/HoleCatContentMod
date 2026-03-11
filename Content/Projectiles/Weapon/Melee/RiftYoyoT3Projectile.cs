
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	public class RiftYoyoT3Projectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// The following sets are only applicable to yoyo that use aiStyle 99.

			// YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
			// Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
			ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;

			// YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
			// Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
			ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;

			// YoyosTopSpeed is top speed of the yoyo Projectile.
			// Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
			ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
		}

        public override void SetDefaults()
        {
            Projectile.width = 20; // The width of the projectile's hitbox.
            Projectile.height = 20; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
                                       // Projectile.scale = 1f; // The scale of the projectile. Most yoyos are 1f, but a few are larger. The Kraken is the largest at 1.2f
            Projectile.netImportant = true;
		}

        float AuraScale = 0.3f;
        public override bool PreDraw(ref Color lightColor)
        {
			DTUtils.DrawRiftBall(Projectile.Center, 0.2f, Main.spriteBatch, blendState: BlendState.Additive, TrailPositions, AuraScale);

            return true;
        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();

        private const int TrailLength = 200;
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
            Vector2 newPos = Projectile.Center;

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
            {
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            }
            while (TrailRotations.Count > TrailLength)
            {
                TrailRotations.RemoveAt(TrailRotations.Count - 1);
            }


            foreach (Vector2 pt in TrailPositions)
            {
                Lighting.AddLight(pt, ColorLib.Rift.ToVector3() * 0.1f);
            }
        }

        int ScaleTimer = 0;
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (ScaleTimer % 60 == 0 && AuraScale < 1f)
            {
                hitbox.Inflate(20, 20);
            }
        }

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ElectricLoop1")
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public float SoundVolume = 0.0f;
        public float BrightnessMod = 1f;
        public override void PostAI() 
		{
            base.PostAI();
            ScaleTimer++;
            CacheTrail();

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Volume = SoundVolume;
                activeSound.Position = Projectile.Center;
            }

            if (ScaleTimer % 60 == 0 && AuraScale < 1f)
            {
                AuraScale += 0.1f;
                SoundVolume += 0.1f;
            }
            if (AuraScale >= 1f)
            {
                if (BrightnessMod < 1f)
                {
                    BrightnessMod += 0.005f;
                }
                SunlightModification.Sunlight(BrightnessMod);
            }
            
            if (Main.rand.NextBool(5)) 
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, newColor: ColorLib.Rift);
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AuraScale >= 1f)
            {
                hit.SourceDamage = (int)(hit.SourceDamage * 1.15f);
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRing>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.01f, 0.5f);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftStarFriendly>(), 3, Projectile.Center, Projectile.damage / 3, 0, 5, offset: Projectile.rotation);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SunlightModification.Reset();
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRing>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 0.01f, 1.8f);
        }
	}
}