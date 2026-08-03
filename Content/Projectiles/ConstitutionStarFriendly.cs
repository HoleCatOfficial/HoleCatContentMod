using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class ConstitutionStarFriendly : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 400;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 420;
            Projectile.tileCollide = false;
        }

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
        public float trailOffset = 0;
        public Color MainColor = Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.ConstitutionStarTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, MainColor, trailOffset, 4);

            Opus.DrawTextureOnProj(DTAssetLib.StarAura, Projectile, MainColor * Projectile.Opacity, false, Projectile.velocity.ToRotation(), Projectile.scale, Projectile.scale);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.ColorlessStar, Projectile, Color.White * Projectile.Opacity, true, Projectile.rotation, Projectile.scale, Projectile.scale);

            return false;
        }

        public SoundStyle Chase = new SoundStyle($"DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionStar/Chase") { PitchVariance = 1f, MaxInstances = 0 };

        public bool Flag1 = false;
        public int HomingTime = 60;

        public int Lifetime = 300;
        public int Time = 0;

        public bool StartKill = false;
        public void UpdateLerpTime()
        {
            Time++;

            if (Time > Lifetime)
            {
                StartKill = true;
            }
        }
        public float LifetimeCompletion
        {
            get
            {
                if (Lifetime <= 0)
                {
                    return 0f;
                }

                return (float)Time / (float)Lifetime;
            }
        }

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 15f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 140f;

        float IHomingProjectile.DetectRadius => 2800;

        bool IHomingProjectile.CanHome => !StartKill && DelayTimer >= 20;

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            UpdateLerpTime();
            MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            DelayTimer++;

            if (DelayTimer == 21)
            {
                if (!Flag1)
                {
                    SoundEngine.PlaySound(SoundID.AbigailUpgrade, Projectile.Center);
                    Flag1 = true;
                }
            }

            Projectile.rotation += Projectile.direction * 0.1f;

            Lighting.AddLight(Projectile.Center, MainColor.ToVector3() * 0.2f);



            if (!StartKill)
            {
                if (Main.rand.NextBool(3))
                {
                    ConstitutionParticle Particle = new();
                    Particle.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, 0.6f, 60);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }



            }

            if (StartKill)
            {

                Projectile.velocity *= 0.97f;
                Projectile.scale *= 0.97f;
                Projectile.Opacity -= 0.01f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !StartKill && DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            if (!StartKill)
            {
                SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
                StellarParticleUtils.BloomRing(Projectile.Center, 0.5f, ParticleEngine.BehindProjectiles);
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.StellarFireGradientLooping(), 2f);
                DTUtils.ConstitutionStarExplosionEffects(Projectile);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

            }
        }

    }

    public class ConstitutionStarFriendly_NoHoming : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 400;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 420;
            Projectile.tileCollide = false;
        }

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
        public float trailOffset = 0;
        public Color MainColor = Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.ConstitutionStarTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 24, MainColor, trailOffset, 4);

            Opus.DrawTextureOnProj(DTAssetLib.StarAura, Projectile, MainColor * Projectile.Opacity, false, Projectile.velocity.ToRotation(), Projectile.scale, Projectile.scale);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(DTAssetLib.ColorlessStar, Projectile, Color.White * Projectile.Opacity, true, Projectile.rotation, Projectile.scale, Projectile.scale);

            return false;
        }


        public SoundStyle Chase = new SoundStyle($"DestroyerTest/Assets/Audio/ConstitutionBoss/ConstitutionStar/Chase") { PitchVariance = 1f, MaxInstances = 0 };

        public int Lifetime = 300;
        public int Time = 0;

        public bool StartKill = false;
        public void UpdateLerpTime()
        {
            Time++;

            if (Time > Lifetime)
            {
                StartKill = true;
            }
        }
        public float LifetimeCompletion
        {
            get
            {
                if (Lifetime <= 0)
                {
                    return 0f;
                }

                return (float)Time / (float)Lifetime;
            }
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.rotation += Projectile.direction * 0.1f;

            UpdateLerpTime();
            MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            Lighting.AddLight(Projectile.Center, MainColor.ToVector3() * 0.2f);

            if (!StartKill)
            {
                if (Main.rand.NextBool(3))
                {
                    ConstitutionParticle Particle = new();
                    Particle.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, 0.6f, 60);
                    ParticleEngine.BehindProjectiles.Add(Particle);
                }
            }

            if (StartKill)
            {
                Projectile.velocity *= 0.97f;
                Projectile.scale *= 0.97f;
                Projectile.Opacity -= 0.01f;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            if (!StartKill)
            {
                SoundEngine.PlaySound(DTAssetLib.ConstitutionStarKill, Projectile.Center);
                StellarParticleUtils.BloomRing(Projectile.Center, 0.5f, ParticleEngine.BehindProjectiles);
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.StellarFireGradientLooping(), 2f);
                DTUtils.ConstitutionStarExplosionEffects(Projectile);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);

            }
        }
    }
}