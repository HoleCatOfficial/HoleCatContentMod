using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RogueItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class CursedHammerThrown : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => !returning;

        bool IHomingProjectile.TracksPlayers => returning;

        float IHomingProjectile.HomingTurnSpeed => 13f;
        
        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.005f;

        float IHomingProjectile.HomingMaxAccel => 50f;

        float IHomingProjectile.DetectRadius => 3200;

        bool IHomingProjectile.CanHome => returning;

        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0; // Initialize a cooldown timer
        private SoundStyle Woosh = DTAssetLib.SwordSounds.StandardSwing with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0, Volume = 0.4f };
        private SoundStyle TileHit = DTAssetLib.Charge.MetalTinkLight;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 22;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(returning);
            writer.Write(flightTime);
            writer.Write(soundCooldown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            returning = reader.ReadBoolean();
            flightTime = reader.ReadInt32();

            soundCooldown = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.FireSwing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.CursedFlames, Projectile.rotation, DTAssetLib.FireSwing.Value.Size() / 2, 0.65f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return true;
        }

        public bool RangeOfPlayer = false;

       
        public override void AI()
        {
            // Decrease the cooldown timer on each tick
            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            // Play the sound every 30 ticks
            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(Woosh, Projectile.Center);
                soundCooldown = 10;
            }


            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            // Always spinning
            Projectile.rotation += 0.35f * Projectile.direction;

            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.WretchedColorMap, 0.7f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {

                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, ColorLib.Wretched3, 2f);
                ParticleEngine.BehindProjectiles.Add(Glow);
            }

            if (!returning)
            {
                // OutPhase: Count time before returning
                flightTime++;
                Projectile.velocity *= 0.95f;

                if (flightTime >= 120)
                {
                    returning = true;
                }
            }

            if (returning)
            {
                Projectile.SmoothMoveToPoint(player.MountedCenter, 30f);




                if  (RangeOfPlayer) 
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.Impacts.FlameImpact with
            {
                PitchVariance = 0.1f, Pitch = -0.8f
            };

            SoundStyle Hit2 = DTAssetLib.Impacts.BrightBell with
            {
                PitchVariance = 0.1f,
                Pitch = -0.8f
            };

            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);

            if (player.HeldItem.ModItem is CursedHammer hammer)
            {
                hammer.HitCount++;



                if (hammer.HitCount >= 4)
                {
                    SoundEngine.PlaySound(Hit2, Projectile.Center);
                    for (int j = 0; j < 7; j++)
                    {
                        float offset = MathHelper.TwoPi / 7f * j;
                        Vector2 vel = new Vector2(12, -4).RotatedBy(offset);
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, vel, ModContent.ProjectileType<CursedHammerBolt>(), Projectile.damage / 3, 10, player.whoAmI);
                    }

                    hammer.HitCount = 0;
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        float offset = MathHelper.TwoPi / 3f * j;
                        Vector2 vel = new Vector2(12, -2).RotatedBy(offset);
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, vel, ModContent.ProjectileType<CursedHammerBolt>(), Projectile.damage / 3, 10, player.whoAmI);
                    }
                }
            }



            
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0, 0, 150, ColorLib.CursedFlames, 1f);

                LerpingSpark Spark = new LerpingSpark();
                Spark.PrepareSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), 0f, ColorLib.WretchedColorMap, 0.5f, true, 15, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);

            }

            LerpingSimpleExplosionParticle ExplosionFX = new LerpingSimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, ColorLib.WretchedColorMap, 0.3f, 0.01f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(ColorLib.CursedFlames, 0.2f), 0.05f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);

            

            if (Projectile.penetrate == 1)
            {
                returning = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(TileHit, Projectile.Center);
            Projectile.penetrate--;

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0, 0, 150, ColorLib.CursedFlames, 1f);

                LerpingSpark Spark = new LerpingSpark();
                Spark.PrepareSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), 0f, ColorLib.WretchedColorMap, 0.5f, true, 15, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);

            }

            LerpingSimpleExplosionParticle ExplosionFX = new LerpingSimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, ColorLib.WretchedColorMap, 0.3f, 0.01f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(ColorLib.CursedFlames, 0.4f), 0.05f, 0.01f, 0.4f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);

            returning = true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!RangeOfPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0, 0, 150, ColorLib.CursedFlames, 5f);
                }
            }

        }

    }
}

