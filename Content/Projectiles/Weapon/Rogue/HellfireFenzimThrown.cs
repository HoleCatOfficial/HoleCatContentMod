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
    public class HellfireFenzimThrown : ModProjectile
    {
       
        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0;
        private SoundStyle Woosh = SoundID.Item71 with { MaxInstances = 0 };
        private SoundStyle TileHit = DTAssetLib.Charge.MetalTinkLight with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0, Volume = 0.4f };

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;

            Projectile.tileCollide = true;
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
            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally: SpriteEffects.None;

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            float ROff = Projectile.direction == 1 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(DTAssetLib.FireSwing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.HellFire * 0.8f, ROff, DTAssetLib.FireSwing.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.FireSwing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.HellFire * 0.8f, ROff + MathHelper.Pi, DTAssetLib.FireSwing.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
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

        bool gravity = false;

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
                soundCooldown = 6;
            }


            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            // Always spinning
            Projectile.rotation += 0.6f * Projectile.direction;

            

            /*
            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.Green, Color.Blue, 0.7f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);
            */

            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {

                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.2f, ColorLib.HellFire, 1f, 30);
                ParticleEngine.BehindProjectiles.Add(Glow);

                Fire fire = new();
                fire.PrepareFire(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.2f, DTUtils.RandomDirection(2), 0.01f, ColorLib.HellFire, 0.5f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.BehindProjectiles.Add(fire);
            }

            flightTime++;

            if (!gravity)
            {
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Projectile.velocity.Y += 1.7f;
            }

         
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.SwordSounds.ThinSlice with
            {
                PitchVariance = 0.1f,
                Volume = 2f
            };


            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            SoundEngine.PlaySound(Hit, Projectile.position);

            if (!hit.Crit)
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
            else
            {
                target.AddBuff(BuffID.OnFire3, 300);
            }

            for (int i = 0; i < 10; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(16f, 16f), 0f, ColorLib.HellFire, 0.4f, false, 15, SparkDrawMode.Additive, 3f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }


           

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
                Spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(16f, 16f), 0f, ColorLib.HellFire, 0.4f, false, 15, SparkDrawMode.Additive, 3f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            float X = -oldVelocity.X * 0.5f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);
            gravity = true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {

           SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);


            if (!DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                for (int i = 0; i < 12; i++)
                {
                    PointGlowPreMultiplied Glow = new();
                    Glow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ColorLib.HellFire, 1f, 30);
                    ParticleEngine.BehindProjectiles.Add(Glow);

                    Fire fire = new();
                    fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), DTUtils.RandomDirection(2), 0.01f, ColorLib.HellFire, 0.5f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
                    ParticleEngine.BehindProjectiles.Add(fire);
                }
            }
        }

    }
}

