using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
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
    public class ExoticFenzimThrown : ModProjectile, IHomingProjectile
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
        private SoundStyle Woosh = DTAssetLib.SwordSounds.TenebrisSwing with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0, Volume = 0.4f };
        private SoundStyle TileHit = DTAssetLib.Charge.Anvil;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;

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
            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            float ROff = Projectile.direction == 1 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, Color.SlateBlue, ROff, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);



            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            return false;
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

            /*
            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.Green, Color.Blue, 0.7f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);
            */

            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {

                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, Color.SlateBlue, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow);

                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), DustID.FireworksRGB, new Vector2(4, 0).RotatedBy(Projectile.rotation + (Main.rand.NextFloat(0.25f) * Projectile.direction)), 0, Main.DiscoColor);
                d.noGravity = true;
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
                Projectile.tileCollide = false;



                if (RangeOfPlayer)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.SwordSounds.LightSnap with
            {
                PitchVariance = 0.1f,
                Volume = 1.6f
            };


            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);

            for (int i = 0; i < 10; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(16f, 16f), 0f, Color.SlateBlue, 1f, false, 15, SparkDrawMode.Additive, 3f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, Main.DiscoColor, 0.3f, 0.01f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, Color.SlateBlue, 0.05f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);

            Vector2[] PossiblePositions = Opus.GetEquidistantVectors(12, target.Center, 140);

            int idx  = Main.rand.Next(PossiblePositions.Length);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), PossiblePositions[idx], PossiblePositions[idx].DirectionTo(target.Center) * 30, ModContent.ProjectileType<RainbowSlash>(), Projectile.damage / 2, 5, Projectile.owner);

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
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(16f, 16f), 0f, Color.SlateBlue, 1f, false, 15, SparkDrawMode.Additive, 3f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            SimpleExplosionParticle ExplosionFX = new SimpleExplosionParticle();
            ExplosionFX.Prepare(Projectile.Center, Vector2.Zero, Main.DiscoColor, 0.3f, 0.01f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(ExplosionFX);

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, Color.SlateBlue, 0.05f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);

            returning = true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!RangeOfPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

        }

    }
}

