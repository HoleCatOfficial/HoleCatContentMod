using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class ElectricField : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.ElectricLoopSound(4) with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        float P = -0.3f;
        public override void AI()
        {
            Projectile.ai[0]++;

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

            PointGlowPreMultiplied Glow2 = new();
            Glow2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f), Vector2.Zero, Color.DeepSkyBlue with { A = 0 } * 0.2f, 1f, 20);
            ParticleEngine.BehindProjectiles.Add(Glow2);

            PixelParticle Pixel2 = new();
            Pixel2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f), Main.rand.NextVector2Circular(1f, 1f), Color.DeepSkyBlue with { A = 0 }, 2f);
            ParticleEngine.BehindProjectiles.Add(Pixel2);

            if (Projectile.ai[0] % 10 == 0)
            {
                Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f);
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


            if (Projectile.ai[0] % 15 == 0)
            {
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (target.Center.Distance(Projectile.Center) < 220f && !target.friendly)
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

                        if (!target.boss)
                        {
                            target.velocity *= 0.1f;
                        }
                    }
                }
            }
        }
    }
}
