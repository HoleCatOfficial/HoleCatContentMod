using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Comaceratic;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class RiftMakerEclipseAura : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.ThrowerProjectilesThatCantTriggerEquipEffects[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/Riftmaker/RiftmakerSwallow"), Projectile.Center);
        }

        float Scale = 0f;
        bool f1 = false;
       
        public override void AI()
        {
            Projectile.ai[0]++;

            //Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(180, 180), DustID.Torch, Vector2.Zero).noGravity = true;

            if (Projectile.ai[0] < 40)
            {
                HeatseekerSilohSpark Spark = new();
                Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(180, 180);
                Spark.PrepareSpark(outer, outer.DirectionTo(Projectile.Center) * 4f, outer.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver2, ColorLib.Rift, 0.4f, false, 20, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(Spark);
            }
            else
            {
                if (!f1)
                {
                    LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                    Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Rift, ColorLib.DarkRift1, 0.05f, 0.01f, 3f);
                    ParticleEngine.ShaderParticles.Add(Ring);

                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftStarFriendly>(), 4, Projectile.Center, Projectile.damage / 8, 5, 10, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    f1 = true;
                }
                if (Projectile.timeLeft > 30)
                {

                    if (Scale < 1f)
                    {
                        Scale += 0.08f;
                    }
                    else
                    {
                        
                        Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(180, 180);
                        StarParticle glow = new();
                        glow.Initialize(outer, outer.DirectionFrom(Projectile.Center) * 1.2f, ColorLib.Rift, 0.5f);
                        ParticleEngine.BehindProjectiles.Add(glow);
                        
                        if (Main.rand.NextBool(4))
                        {
                            HeatseekerSilohSpark Spark = new();
                            Spark.PrepareSpark(outer, outer.DirectionFrom(Projectile.Center) * 4f, outer.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver2, ColorLib.Rift, 0.4f, false, 20, SparkDrawMode.Additive, 3f);
                            ParticleEngine.Particles.Add(Spark);
                        }
                    }
                }
                else
                {
                    if (Scale > 0f)
                    {
                        Scale -= 0.08f;

                        if (Scale <= 0.0001f)
                        {
                            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f, Volume = 1.5f }, Projectile.Center);

                            SmallShine shine = new();
                            shine.Prepare(Projectile.Center, Vector2.Zero, Color.White, 1f);
                            ParticleEngine.ShaderParticles.Add(shine);

                            for (int i = 0; i < 7f; i++)
                            {
                                StarParticle Star = new();
                                Star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.Rift, 0.5f);
                                ParticleEngine.BehindProjectiles.Add(Star);
                            }


                            LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Rift, ColorLib.DarkRift1, 0.05f, 0.01f, 0.5f);
                            ParticleEngine.ShaderParticles.Add(Ring);
                        }
                    }
                }
            }
        }

        float roff = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            roff += 0.3f;
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.DarkRift2 with { A = 0 } * 0.5f, -roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.3f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.22f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff * 0.5f, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.20f) * Projectile.scale, SpriteEffects.None, 0);


            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, DTAssetLib.Circle.Value.Size() / 2, (Scale * 0.5f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Rift with { A = 0 }, roff * 0.5f, DTAssetLib.BloomRing.Value.Size() / 2, (Scale * 2.2f) * Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, 180, targetHitbox) && Projectile.ai[0] > 40;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
        }
    }

    public class TrueRiftMakerEclipseAura : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.ThrowerProjectilesThatCantTriggerEquipEffects[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/Riftmaker/TrueRiftmakerSwallow"), Projectile.Center);
        }

        float Scale = 0f;
        bool f1 = false;

        public override void AI()
        {
            Projectile.ai[0]++;

            //Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(180, 180), DustID.Torch, Vector2.Zero).noGravity = true;

            if (Projectile.ai[0] < 40)
            {
                HeatseekerSilohSpark Spark = new();
                Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(360, 360);
                Spark.PrepareSpark(outer, outer.DirectionTo(Projectile.Center) * 4f, outer.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver2, ColorLib.Rift, 0.4f, false, 20, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(Spark);
            }
            else
            {
                if (!f1)
                {
                    LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                    Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Rift, ColorLib.DarkRift1, 0.05f, 0.01f, 4f);
                    ParticleEngine.ShaderParticles.Add(Ring);

                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftStarFriendly2>(), 4, Projectile.Center, Projectile.damage / 8, 5, 10, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    f1 = true;
                }
                if (Projectile.timeLeft > 30)
                {

                    if (Scale < 1f)
                    {
                        Scale += 0.08f;
                    }
                    else
                    {

                        Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(360, 360);
                        StarParticle glow = new();
                        glow.Initialize(outer, outer.DirectionFrom(Projectile.Center) * 2.2f, ColorLib.Rift, 0.5f);
                        ParticleEngine.BehindProjectiles.Add(glow);

                        if (Main.rand.NextBool(2))
                        {
                            ComaceraticParticle particle = new();
                            particle.Initialize(outer, outer.DirectionFrom(Projectile.Center) * 10f, Color.OrangeRed, 0.25f);
                            ParticleEngine.Particles.Add(particle);
                        }
                    }
                }
                else
                {
                    if (Scale > 0f)
                    {
                        Scale -= 0.08f;

                        if (Scale <= 0.0001f)
                        {
                            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") { MaxInstances = 0, PitchVariance = 0.2f, Volume = 1.5f }, Projectile.Center);

                            SmallShine shine = new();
                            shine.Prepare(Projectile.Center, Vector2.Zero, Color.White, 1f);
                            ParticleEngine.ShaderParticles.Add(shine);

                            for (int i = 0; i < 7f; i++)
                            {
                                StarParticle Star = new();
                                Star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4, 4), ColorLib.Rift, 0.5f);
                                ParticleEngine.BehindProjectiles.Add(Star);
                            }


                            LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Rift, ColorLib.DarkRift1, 0.05f, 0.01f, 0.5f);
                            ParticleEngine.ShaderParticles.Add(Ring);
                        }
                    }
                }
            }

            int npctarg = Projectile.AutoTarget();

            if (npctarg != -1 && Projectile.ai[0] % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0f, -1400f), (Projectile.Center + new Vector2(0f, -1400f)).DirectionTo(Main.npc[npctarg].Center).RotatedByRandom(0.1f) * 30f, ModContent.ProjectileType<TrueRiftmakerClone>(), Projectile.damage / 2, 4f);
            }
        }

        float roff = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            roff += 0.3f;
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, Color.Red with { A = 0 } * 0.5f, -roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.6f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.44f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.Corona.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, roff * 0.5f, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.40f) * Projectile.scale, SpriteEffects.None, 0);


            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, DTAssetLib.Circle.Value.Size() / 2, Scale * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, roff * 0.5f, DTAssetLib.BloomRing.Value.Size() / 2, (Scale * 4.4f) * Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, 360, targetHitbox) && Projectile.ai[0] > 40;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }
    }
}
