using System;
using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ThunderScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.SkyBlue;
            WidthDim = 36;
            HeightDim = 34;
            DustType = DustID.Electric;
            base.SetDefaults();

            Projectile.tileCollide = false;
        }

        public override void DefaultBehaviour()
        {
            Projectile.ai[0]++;
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.4f * Projectile.direction;


            
            if (player.controlUseTile && player.HeldItem.type == ModContent.ItemType<ThunderScepter>())
            {
                returning = false;
                Projectile.SmoothMoveToPoint(Main.MouseWorld, 32f, 100f);

                if (Projectile.ai[0] % 15 == 0)
                {
                    foreach (NPC target in Main.ActiveNPCs)
                    {
                        if (target.Center.Distance(Projectile.Center) < 160 && !target.friendly)
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
                            int Damage = (int)(Projectile.damage);
                            target.SimpleStrikeNPC(Damage, Math.Sign((target.Center - Projectile.Center).X), false, 1f, DamageClass.Default);

                            if (target.life < 0)
                            {
                                Projectile.timeLeft += 60;
                            }
                        }
                    }
                }

            }
            else
            {
                returning = true;
            }

            if (returning)
            {
                ArmCatchAnimate(player);

                Projectile.SmoothMoveToPoint(player.MountedCenter, 32f, 100f);
                
                if (Projectile.Distance(player.MountedCenter) < 32)
                {
                    HitCount = 0;
                    existenceTimer = 0;
                    Projectile.Kill();
                }
            }
        }
    }
}

