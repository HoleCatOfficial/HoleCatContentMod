using System;
using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Thrower.StealthStrike;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower.HellDisc
{
    public class HellDiscThrown : ModProjectile, IHomingProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public bool Special = false;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = DTAssetLib.DreamDiscHighlight.Value;

            SpriteBatch spriteBatch = Main.spriteBatch;

            SpriteEffects fx = SpriteEffects.None;

            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, ColorLib.HellFire * 0.6f, Projectile.rotation, DTAssetLib.CircularSwingThin.Value.Size() / 2, Projectile.scale, fx);
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, ColorLib.HellFire * 0.6f, Projectile.rotation + MathHelper.Pi, DTAssetLib.CircularSwingThin.Value.Size() / 2, Projectile.scale, fx);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.OldCenter()[i] - Main.screenPosition, null, Color.White with { A = 0 } * ((float)i / (float)Projectile.oldPos.Length).Inverse() * 0.5f, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, fx);
            }

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, fx);
            return false;
        }
        Player Owner => Main.player[Projectile.owner];
        public bool HasHit = false;
        bool IHomingProjectile.TracksNPCs => !HasHit;

        bool IHomingProjectile.TracksPlayers => false;

        float TSpeed = 1f;
        float IHomingProjectile.HomingTurnSpeed => TSpeed;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.02f;

        float IHomingProjectile.HomingMaxAccel => 50f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => !HasHit && Projectile.ai[0] >= 10;

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void AI()
        {
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.rotation += 0.5f * Projectile.direction;
            Projectile.ai[0]++;

            TSpeed += 0.2f;
         
            if (Projectile.ai[0] % 5 == 0)
            {
                SoundEngine.PlaySound(DTAssetLib.SwordSounds.HeavySwing with { MaxInstances = 0, PitchVariance = 0.2f, Pitch = 1f }, Projectile.Center);
            }

            if (Special)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
            }
            
            if (HasHit)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 60)
                {
                    if (Projectile.ai[1] == 61)
                    {
                        SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact, Projectile.Center);
                        BloomRingSharp ring = new();
                        ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.HellFire, 0.1f, 0.01f, 0.8f, BlendState.Additive);
                        ParticleEngine.Particles.Add(ring);

                        for (int i = 0; i < 10; i++)
                        {
                            Spark spark = new();
                            spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.Center.DirectionFrom(Owner.Center) * Main.rand.NextFloat(4f, 9f), Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.HellFire, 0.4f, false, 40, SparkDrawMode.Additive, 2.5f);
                            ParticleEngine.Particles.Add(spark);
                        }
                    }

                    if (Special)
                    {
                        if (Projectile.ai[2] > 2)
                        {
                            Special = false;
                        }
                        else
                        {
                            Projectile.timeLeft = 240;
                            HasHit = false;
                            Projectile.velocity *= 70f;
                            Projectile.ai[1] = 0;
                            Projectile.ai[2]++;
                        }
                    }
                    else
                    {
                        Projectile.SmoothMoveToPoint(Owner.MountedCenter, 25f * Owner.GetAttackSpeed(DamageClass.Throwing), 80f);
                    }
                    
                }
                else
                {
                    
                    Projectile.velocity *= 0.93f;
                }

                if (Projectile.Distance(Owner.MountedCenter) < 30)
                {
                    Projectile.Kill();
                }
            }
          
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !HasHit && Projectile.ManualCanHitFriendly(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
            HasHit = true;
            Projectile.timeLeft = 180;

            SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit with { MaxInstances = 0, PitchVariance = 0.9f, Pitch = -1f }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.HellWeaponImpact with { MaxInstances = 0, PitchVariance = 0.9f, Volume = 0.7f }, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Spark spark = new();
                spark.PrepareSpark(target.Center, -Projectile.oldVelocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.6f), Projectile.oldVelocity.ToRotation() + MathHelper.PiOver2, ColorLib.HellFire, 1f, false, 20, SparkDrawMode.Additive, 2.5f);
                ParticleEngine.Particles.Add(spark);
            }

            Projectile.velocity = Projectile.velocity.RotatedByRandom(1f) * -0.75f;
        }

        public override void OnKill(int timeLeft)
        {
         
        }
    }
}