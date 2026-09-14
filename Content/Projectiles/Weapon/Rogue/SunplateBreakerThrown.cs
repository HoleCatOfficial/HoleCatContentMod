using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class SunplateBreakerThrown : ModProjectile
    {
        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0; // Initialize a cooldown timer
        private SoundStyle Woosh = DTAssetLib.SwordSounds.Woosh with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = 0.3f };
        //private SoundStyle TileHit = DTAssetLib.Impacts.LightMetalHit with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = -0.8f, Volume = 0.4f };
        private SoundStyle TileHit = DTAssetLib.Charge.FlatTick with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = -0.3f, Volume = 1.2f };
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300; // 10 seconds max lifespan
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
            float rOff = Projectile.direction == 1 ? 0f : MathHelper.PiOver4;
            SpriteEffects Fx = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            //Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);


            //Main.EntitySpriteDraw(DTAssetLib.SwingFX.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation - rOff, DTAssetLib.SwingFX.Value.Size() / 2, Projectile.scale * 0.5f, Fx, 0);
            //Opus.ReturnToDefaultDrawing(Main.spriteBatch);
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
            if (soundCooldown <= 0 && !returning)
            {
                SoundEngine.PlaySound(Woosh, Projectile.Center);
                soundCooldown = 12;
            }



            if (returning)
            {
                Projectile.velocity.Y += 1.5f;
                Projectile.tileCollide = false;
                if (Projectile.Opacity > 0)
                {
                    Projectile.Opacity -= 0.01f;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                flightTime++;
                if (flightTime > 30)
                {
                    returning = true;
                }
            }

            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            // Always spinning
            Projectile.rotation += 0.4f * Projectile.direction;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !returning;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = hit.Crit ? DTAssetLib.Impacts.HeavyCrit with { PitchVariance = 0.2f, Volume = 1.7f } : DTAssetLib.Impacts.DreamHit with { Pitch = 1.3f, PitchVariance = 0.1f };

            Player player = Main.player[Main.myPlayer];  // Accessing the current player


            Vector2 targetPos  = target.Center;
            float ceilingLimit = targetPos.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }
            for (int i = 0; i < (hit.Crit ? 8 : 5); i++)
            {
                Vector2 position2 = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                //position2.Y -= 100 * i;
                Vector2 heading = position2.DirectionTo(targetPos);


                Projectile Star = Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), position2, heading * 30f, ProjectileID.FallingStar, Projectile.damage / 8, 4f, player.whoAmI, 0f, ceilingLimit);
            }


            SoundEngine.PlaySound(Hit, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Vector2 Vel = -Projectile.oldVelocity.RotatedByRandom(1f);
                Vel.Normalize();
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Vel * Main.rand.NextFloat(9f, 15f), 0f, Color.CadetBlue, 0.2f, false, 10, SparkDrawMode.Additive, 2.5f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            ImpactCracks cracks = new();
            cracks.Prepare(target.Center, Color.CadetBlue, 0.7f);
            ParticleEngine.Particles.Add(cracks);


            float X = -Projectile.oldVelocity.X * 0.2f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -Projectile.oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);


            returning = true;
            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(TileHit, Projectile.Center);


            for (int i = 0; i < 10; i++)
            {
                Vector2 Vel = -Projectile.oldVelocity.RotatedByRandom(1f);
                Vel.Normalize();
                Spark Spark = new Spark();
                Spark.PrepareSpark(Projectile.Center, Vel * Main.rand.NextFloat(9f, 15f), 0f, Color.CadetBlue, 0.2f, false, 10, SparkDrawMode.Additive, 2.5f);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            float X = -oldVelocity.X * 0.2f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);
            returning = true;
            Projectile.penetrate--;
            return false;
        }

        public override void OnKill(int timeLeft)
        {

        }

    }
}

