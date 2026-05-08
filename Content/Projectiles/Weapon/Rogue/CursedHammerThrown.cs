using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
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
    public class CursedHammerThrown : ModProjectile
    {
        public int Variant = Main.rand.Next(0, 3);
        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0; // Initialize a cooldown timer
        private SoundStyle Woosh = DTAssetLib.SwordSounds.StandardSwing with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0 };
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
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 22;
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
            Projectile.rotation += 0.7f * Projectile.direction;

            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.WretchedColorMap, 1f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

            if (!returning)
            {
                // OutPhase: Count time before returning
                flightTime++;
                if (flightTime >= 60)
                {
                    returning = true;
                }
            }

            if (returning)
            {
                // InPhase: Smooth return using Lerp
                Vector2 returnDirection = player.Center - Projectile.Center;
                float speed = MathHelper.Lerp(Projectile.velocity.Length(), 25f, 0.08f); // Smooth acceleration
                Projectile.velocity = returnDirection.SafeNormalize(Vector2.Zero) * speed;



                // If close enough, remove the projectile
                if (returnDirection.LengthSquared() < 45f || RangeOfPlayer) // 4 pixels radius
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.Impacts.FleshHit with
            {
                PitchVariance = 0.5f
            };

            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0, 0, 150, ColorLib.CursedFlames, 1f);

                LerpingSpark Spark = new LerpingSpark();
                Spark.PrepareSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), 0f, ColorLib.WretchedColorMap, 0.5f, true, 15, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);

            }

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(target.Center, Vector2.Zero, ColorLib.CursedFlames, 0.1f, 0.02f, 0.75f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(target.Center, Vector2.Zero, DTColorUtils.Pastel(ColorLib.CursedFlames, 0.5f), 0.1f, 0.02f, 0.75f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

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

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, ColorLib.CursedFlames, 0.1f, 0.02f, 0.75f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(ColorLib.CursedFlames, 0.5f), 0.1f, 0.02f, 0.75f, BlendState.Additive);
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

