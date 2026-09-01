using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
 
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
    public class TenebrisWaraxeProjectile : ModProjectile
    {
        public int Variant = Main.rand.Next(0, 3);
        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0; // Initialize a cooldown timer
        private SoundStyle Woosh = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HeavySwing", 3) with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = 0.7f };
        private SoundStyle TileHit = DTAssetLib.Impacts.LightMetalHit with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = -0.8f, Volume = 0.4f };
        public Color clr = Color.White;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600; // 10 seconds max lifespan
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 10;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;

            
        }

        
        
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Variant;
            
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
            Main.EntitySpriteDraw(DTAssetLib.SwingFX.Value, Projectile.Center - Main.screenPosition, null, clr, Projectile.rotation, DTAssetLib.SwingFX.Value.Size() / 2, Projectile.scale * 1f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return true;
        }

        public bool RangeOfPlayer = false;

        public override void AI()
        {
            if (Variant == 0)
            {
                clr = ColorLib.TenebrisMagenta;
            }
            if (Variant == 1)
            {
                clr = ColorLib.TenebrisBlue;
            }
            if (Variant == 2)
            {
                clr = ColorLib.TenebrisBeige;
            }
            // Decrease the cooldown timer on each tick
            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            // Play the sound every 30 ticks
            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(Woosh, Projectile.Center);
                soundCooldown = 12;
            }



            if (returning)
            {
                Projectile.velocity.Y += 1.7f;
            }
            else
            {
                flightTime++;
                if (flightTime > 2)
                {
                    returning = true;
                }
            }

            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            // Always spinning
            Projectile.rotation += 0.4f * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.Impacts.DarkShot with
            {
            PitchVariance = 0.5f
            };

            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, -Projectile.velocity.X, -Projectile.velocity.Y, 150, clr, 2f);

                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), 0f, clr, 1f, true, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);

            }

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(target.Center, Vector2.Zero, clr, 0.1f, 0.02f, 2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(target.Center, Vector2.Zero, DTColorUtils.Pastel(clr, 0.5f), 0.02f, 0.01f, 0.3f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

            if (Projectile.penetrate == 1)
            {
                returning = true;
            }
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 3, Projectile.Center, Projectile.damage / 3, 4, 15, ai2: 1, offset: Projectile.rotation);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(TileHit, Projectile.Center);
            Projectile.penetrate--;

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, -oldVelocity.X, -oldVelocity.Y, 150, clr, 2f);
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), 0f, clr, 1f, true, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);

            }

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, clr, 0.1f, 0.02f, 2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(clr, 0.5f), 0.02f, 0.01f, 0.3f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

            float X = -oldVelocity.X * 0.5f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);
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
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0, 0, 150, clr, 5f);
                }
            }
            
        }

    }
}

