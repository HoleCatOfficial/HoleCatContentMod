using System;
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
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class VoltaicFenzimThrown : ModProjectile
    {
        private SoundStyle Woosh = SoundID.Item1;
        //private SoundStyle TileHit = DTAssetLib.Impacts.LightMetalHit with { PitchVariance = 0.4f, MaxInstances = 0, Pitch = -0.8f, Volume = 0.4f };
        private SoundStyle TileHit = SoundID.Item109;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.tileCollide = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;


        }



        public override void OnSpawn(IEntitySource source)
        {


        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            return false;
        }

        float MaskAlpha = 0f;
        public override void PostDraw(Color lightColor)
        {
            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/VoltaicFenzimMask").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * MaskAlpha, Projectile.rotation, origin, Projectile.scale, FX, 0f);

        }


        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.ElectricLoopSound(4) with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        float P = -0.3f;

        public float effectRadius = 150;
        public override void AI()
        {
            Projectile.ai[0]++;

            if (MaskAlpha > 0f)
            {
                MaskAlpha -= 0.02f;
            }

           

            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());

            if (!Stuck)
            {
                if (Projectile.ai[0] % 5 == 0)
                {
                    SoundEngine.PlaySound(Woosh, Projectile.Center);

                }

                Projectile.velocity.Y += 1.7f;

                Projectile.rotation += 0.6f * Projectile.direction;

                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Vector2.Zero, Color.DeepSkyBlue with { A = 0 } * 0.2f, 1f);
                ParticleEngine.BehindProjectiles.Add(Glow);

                PixelParticle Pixel = new();
                Pixel.Initialize(Projectile.Center + new Vector2(Projectile.width / 2, -(Projectile.width / 2)).RotatedBy(Projectile.rotation), Main.rand.NextVector2Circular(1f, 1f), Color.SkyBlue with { A = 0 }, 2f);
                ParticleEngine.BehindProjectiles.Add(Pixel);

                float Len = Projectile.velocity.Length();
                effectRadius = 150 + Len;
            }
            else
            {
                Projectile.velocity *= 0;

                if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
                {
                    var tracker = new ProjectileAudioTracker(Projectile);
                    LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                        soundInstance.Position = Projectile.Center;
                        return tracker.IsActiveAndInGame();
                    });
                }
                else
                {
                    activeSound.Volume = 0.5f;
                    activeSound.Position = Projectile.Center;
                    activeSound.Pitch = P;
                }

                if (Projectile.timeLeft > 30)
                {
                    if (P < 0f)
                    {
                        P += 0.01f;
                    }
                }
                else
                {
                    if (P > -0.3f)
                    {
                        P -= 0.01f;
                    }
                }

                if (Main.rand.NextBool(5))
                {
                    PointGlowPreMultiplied Glow = new();
                    Glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-5, -3)).RotatedBy(RecordedOldVelocity.ToRotation()), Color.DeepSkyBlue with { A = 0 } * 0.2f, 2.5f);
                    ParticleEngine.BehindProjectiles.Add(Glow);
                }

                PixelParticle Pixel = new();
                Pixel.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-5, -3)).RotatedBy(RecordedOldVelocity.ToRotation()), Color.DeepSkyBlue, 2f, 30);
                ParticleEngine.BehindProjectiles.Add(Pixel);


                PointGlowPreMultiplied Glow2 = new();
                Glow2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(effectRadius, effectRadius), Vector2.Zero, Color.DeepSkyBlue with { A = 0 } * 0.2f, 1f, 20);
                ParticleEngine.BehindProjectiles.Add(Glow2);

                PixelParticle Pixel2 = new();
                Pixel2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(effectRadius, effectRadius), Main.rand.NextVector2Circular(1f, 1f), Color.DeepSkyBlue with { A = 0 }, 2f);
                ParticleEngine.BehindProjectiles.Add(Pixel2);

                if (Projectile.ai[0] % 10 == 0)
                {
                    Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(effectRadius, effectRadius);
                    var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                    var points = curve.GetEvenlySpacedPoints(15);
                    for (int i = 0; i < points.Count; i++)
                    {
                        PointGlowPreMultiplied ArcGlow = new();
                        ArcGlow.Initialize(points[i], Main.rand.NextVector2Circular(0.2f, 0.2f), Color.DeepSkyBlue with { A = 0 } * 0.05f, 2.5f);
                        ParticleEngine.BehindProjectiles.Add(ArcGlow);

                        PixelParticle Arc = new();
                        Arc.Initialize(points[i], Vector2.Zero, Color.DeepSkyBlue with { A = 0 }, 2f, 60);
                        ParticleEngine.BehindProjectiles.Add(Arc);
                    }

                    BloomRingSharp Ring = new BloomRingSharp();
                    Ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue * 0.1f, 0.02f, 0.01f, 0.6f, BlendState.Additive);
                    ParticleEngine.BehindProjectiles.Add(Ring);


                }

                if (Projectile.ai[0] % 30 == 0)
                {
                    foreach (NPC target in Main.ActiveNPCs)
                    {
                        if (target.Center.Distance(Projectile.Center) < effectRadius && !target.friendly)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, target.Center);

                            Vector2 outer = target.Center;
                            var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                            var points = curve.GetEvenlySpacedPoints(15);
                            for (int i = 0; i < points.Count; i++)
                            {
                                PointGlowPreMultiplied ArcGlow = new();
                                ArcGlow.Initialize(points[i], Main.rand.NextVector2Circular(0.2f, 0.2f), Color.DeepSkyBlue with { A = 0 } * 0.05f, 2.5f);
                                ParticleEngine.BehindProjectiles.Add(ArcGlow);

                                PixelParticle Arc = new();
                                Arc.Initialize(points[i], Vector2.Zero, Color.DeepSkyBlue with { A = 0 }, 2f, 60);
                                ParticleEngine.BehindProjectiles.Add(Arc);
                            }

                            target.AddBuff(BuffID.Electrified, 300);
                            int Damage = (int)(Projectile.damage * 0.3f);
                            Damage = Utils.Clamp(Damage, 10, 50);
                            target.SimpleStrikeNPC(Damage, Math.Sign((target.Center - Projectile.Center).X), false, 1f, DamageClass.Default);
                            target.velocity *= 0.1f;

                            MaskAlpha = 0.5f;

                            if (target.life < 0)
                            {
                                Projectile.timeLeft += 120;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Hit = DTAssetLib.Impacts.ShortShine with { PitchVariance = 0.5f };

            MaskAlpha = 1f;

            Player player = Main.player[Main.myPlayer]; 
            SoundEngine.PlaySound(Hit, Projectile.position);
            SoundEngine.PlaySound(DTAssetLib.Zap, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f), Color.DeepSkyBlue with { A = 0 } * 0.05f, 2.5f);
                ParticleEngine.BehindProjectiles.Add(Glow);

                PixelParticle Pixel = new();
                Pixel.Initialize(Projectile.Center, Main.rand.NextVector2Circular(0.25f, 0.25f), Color.DeepSkyBlue with { A = 0 }, 2f, 30);
                ParticleEngine.BehindProjectiles.Add(Pixel);
            }

            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(target.Center, Vector2.Zero, Color.DeepSkyBlue, 0.02f, 0.01f, 0.3f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

            target.AddBuff(BuffID.Electrified, 480);
        }

        bool Stuck = false;
        Vector2 RecordedOldVelocity = Vector2.Zero;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Stuck)
            {
                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/VoltaicFenzimImpact") with { Volume = 0.5f, PitchVariance = 0.4f }, Projectile.Center);
                SoundEngine.PlaySound(TileHit, Projectile.Center);
                SoundEngine.PlaySound(DTAssetLib.Impacts.Deflect, Projectile.Center);

                BloomRingSharp Ring = new BloomRingSharp();
                Ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.1f, 0.01f, 1f, BlendState.Additive);
                ParticleEngine.BehindProjectiles.Add(Ring);

                for (int t = 0; t < 6; t++)
                {
                    Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200);
                    var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                    var points = curve.GetEvenlySpacedPoints(15);
                    for (int i = 0; i < points.Count; i++)
                    {
                        PointGlowPreMultiplied ArcGlow = new();
                        ArcGlow.Initialize(points[i], Main.rand.NextVector2Circular(0.2f, 0.2f), Color.DeepSkyBlue with { A = 0 } * 0.05f, 2.5f);
                        ParticleEngine.BehindProjectiles.Add(ArcGlow);

                        PixelParticle Arc = new();
                        Arc.Initialize(points[i], Vector2.Zero, Color.DeepSkyBlue with { A = 0 }, 2f, 60);
                        ParticleEngine.BehindProjectiles.Add(Arc);
                    }
                }

                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (target.Center.Distance(Projectile.Center) < effectRadius && !target.friendly)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, target.Center);

                        Vector2 outer = target.Center;
                        var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                        var points = curve.GetEvenlySpacedPoints(15);
                        for (int i = 0; i < points.Count; i++)
                        {
                            PointGlowPreMultiplied ArcGlow = new();
                            ArcGlow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Color.DeepSkyBlue with { A = 0 } * 0.05f, 2.5f);
                            ParticleEngine.BehindProjectiles.Add(ArcGlow);

                            PixelParticle Arc = new();
                            Arc.Initialize(points[i], Vector2.Zero, Color.DeepSkyBlue with { A = 0 }, 2f, 60);
                            ParticleEngine.BehindProjectiles.Add(Arc);


                        }

                        target.AddBuff(BuffID.Electrified, 300);
                        int Damage = (int)(Projectile.damage);
                        Damage = Utils.Clamp(Damage, 10, 100);
                        target.SimpleStrikeNPC(Damage, Math.Sign((target.Center - Projectile.Center).X), false, 1f, DamageClass.Default);
                        target.velocity *= 0.1f;

                        MaskAlpha = 0.5f;

                        if (target.life < 0)
                        {
                            Projectile.timeLeft += 60;
                        }
                    }
                }

                RecordedOldVelocity = -oldVelocity.RotatedBy(MathHelper.PiOver2);

                MaskAlpha = 1f;
                Projectile.timeLeft = 300;
                Projectile.netUpdate = true;
                Stuck = true;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.MagicBeep, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Zap, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                PointGlowPreMultiplied Glow = new();
                Glow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Color.DeepSkyBlue with { A = 0 } * 0.2f, 2.5f);
                ParticleEngine.BehindProjectiles.Add(Glow);

                PixelParticle Pixel = new();
                Pixel.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Color.DeepSkyBlue, 2f, 30);
                ParticleEngine.BehindProjectiles.Add(Pixel);
            }

        }

    }
}

