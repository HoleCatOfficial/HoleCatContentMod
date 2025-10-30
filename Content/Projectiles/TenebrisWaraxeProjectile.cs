using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System.IO;
using InnoVault.PRT;
using Terraria.GameContent.UI.Chat;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisWaraxeProjectile : ModProjectile
    {
        public int Variant = Main.rand.Next(0, 3);
        private bool returning = false;
        private int flightTime = 0;
        private int soundCooldown = 0; // Initialize a cooldown timer
        private SoundStyle Woosh = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HeavySwing", 3) with { PitchVariance = 0.4f, MaxInstances = 0 };
        private SoundStyle TileHit = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5) with { PitchVariance = 0.4f, MaxInstances = 0 };
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
            Projectile.DamageType = DamageClass.Generic;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 10;
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
            Opus.Opus opus = new Opus.Opus();

            opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.SwingFX.Value, Projectile.Center - Main.screenPosition, null, clr, Projectile.rotation, DTAssetLib.SwingFX.Value.Size() / 2, 0.65f, SpriteEffects.None, 0);
            opus.ReturnToDefaultDrawing(Main.spriteBatch);
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
                soundCooldown = 10;
            }





            Player player = Main.player[Projectile.owner];

            RangeOfPlayer = Projectile.Center.Distance(player.Center) < 20;

            // Always spinning
            Projectile.rotation += 0.4f * Projectile.direction;

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
            SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3) with
            {
            PitchVariance = 0.5f
            };

            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            hit.Knockback = 4f;
            target.StrikeNPC(hit);
            SoundEngine.PlaySound(Hit, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0, 0, 150, clr, 5f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), clr, 2);

            }
            DTUtils.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, Color.White, 0.01f, ai0: 0.8f);
            DTUtils.NewParticleFloatAI(PRTLoader.GetParticleID<Boom5>(), Projectile.Center, Vector2.Zero, clr, 0.01f, ai0: 1.5f);
            if (Projectile.penetrate == 1)
            {
                returning = true;
            }
            DTUtils.instance.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStar>(), 9, Projectile.Center, Projectile.damage / 3, 4, 15, AI2: 1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(TileHit, Projectile.Center);
            Projectile.penetrate--;

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0, 0, 150, clr, 5f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), Projectile.Center, new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-15, -10)), clr, 2);

            }
            DTUtils.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, Color.White, 0.01f, ai0: 0.8f);
            DTUtils.NewParticleFloatAI(PRTLoader.GetParticleID<Boom5>(), Projectile.Center, Vector2.Zero, clr, 0.01f, ai0: 1.5f);
            DTUtils.instance.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStar>(), 9, Projectile.Center, Projectile.damage / 3, 4, 15, AI2: 1);

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

