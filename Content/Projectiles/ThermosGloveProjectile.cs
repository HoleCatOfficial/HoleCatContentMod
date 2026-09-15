using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace DestroyerTest.Content.Projectiles
{
    public class ThermosGloveProjectile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public float CurrentDistance = 0f;

        public float MaxDistance = 16f * 12f;

        public float Progress => CurrentDistance / MaxDistance;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
        }

        float r = 0;

        bool Draw = true;
        public override bool PreDraw(ref Color lightColor)
        {
            r += 0.1f;

            if (Draw)
            {
                if (Plot != null)
                {
                    Vector2[] Points = PrimPlot.GetPointsAlongLine(20);
                    float[] rots = [Plot.GetLineRotation, Plot.GetLineRotation, Plot.GetLineRotation, Plot.GetLineRotation, Plot.GetLineRotation];

                    DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2, true).Value, Points.ToList(), rots.ToList(), 30f, Color.Lerp(Color.Brown, Color.OrangeRed, Progress), r * -0.2f, 2);
                }


                Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Brown, Color.OrangeRed, Progress) with { A = 0 }, r, DTAssetLib.Swirl.Value.Size() / 2, 0.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Brown, Color.OrangeRed, Progress) with { A = 0 }, r * 1.5f, DTAssetLib.Swirl.Value.Size() / 2, 0.25f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, r * 2, DTAssetLib.Swirl.Value.Size() / 2, 0.2f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, r * 2, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.4f, SpriteEffects.None);

                Texture2D Tx = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph").Value;
                Texture2D Glo = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph2").Value;
                Main.EntitySpriteDraw(Glo, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Brown, Color.OrangeRed, Progress) with { A = 0 }, Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation(), new Vector2(0f, Glo.Height / 2), new Vector2(MathHelper.Lerp(0f, 3f, Progress), 1f), SpriteEffects.None);
                Main.EntitySpriteDraw(Tx, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Brown, Color.OrangeRed, Progress) with { A = 0 }, Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation(), new Vector2(0f, Tx.Height / 2), new Vector2(MathHelper.Lerp(0f, 0.5f, Progress), 0.2f), SpriteEffects.None);
            }
            return false;
            
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.LoopedSounds.GenericLaser with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        float P = -1.3f;

        Line Plot;
        Line PrimPlot;

        int Idx = 0;
        bool Deactivated = false;

        bool soundFlag = false;
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

          
            
            if (Owner.HeldItem.ModItem is ThermosGlove Glove && Owner.controlUseItem && !Owner.CCed && !Deactivated)
            {
                Plot = new Line(Projectile.Center, Projectile.Center + new Vector2(CurrentDistance, 0).RotatedBy(Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation()));
                PrimPlot = new Line(Owner.Center, Owner.Center + new Vector2(CurrentDistance * 2f, 0).RotatedBy(Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation()));

                Draw = true;
                Owner.SetDummyItemTime(30);

                Vector2 Pos = Owner.MountedCenter + new Vector2(25, 0).RotatedBy(Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation());
                Projectile.Center = Owner.RotatedRelativePoint(Pos);

                PixelParticle pixel = new();
                pixel.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.Center.DirectionTo(Main.MouseWorld) * Main.rand.NextFloat(1f, 3f), Color.Lerp(Color.Brown, Color.OrangeRed, Progress), 2f);
                ParticleEngine.Particles.Add(pixel);

                Projectile.timeLeft = 120;

                if (CurrentDistance < MaxDistance)
                {
                    CurrentDistance++;
                }

                if (CurrentDistance == MaxDistance)
                {
                    if (!soundFlag)
                    {
                        SoundEngine.PlaySound(SoundID.Item100);
                        soundFlag = true;
                    }
                }

                P = MathHelper.Lerp(-1.3f, 0.6f, Progress);

               

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

                Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Brown, Color.OrangeRed, Progress).ToVector3());
            }
            else
            {
                Projectile.ai[0]++;
                Draw = false;

                Deactivated = true;

                if (SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
                {
                    activeSound.Volume = 0f;
                }

                if (Owner.CCed)
                {
                    Projectile.Kill();
                }
                else
                {
                    int count = (int)MathHelper.Lerp(1, 12, Progress);
                    Vector2[] ExplosionPoints = Plot.GetPointsAlongLine(count);

                    if (Projectile.ai[0] % 1 == 0 && Idx < ExplosionPoints.Length - 1)
                    {
                        Idx++;

                        int i = Idx;

                        Point p = ExplosionPoints[i].ToTileCoordinates();

                        Point resultPoint;
                        Rectangle SearchArea = Utils.CenteredRectangle(p.ToWorldCoordinates(), new Vector2(60, 60));
                        //Dust.DrawDebugBox(SearchArea);

                        Ref<int> TooHardCount = new Ref<int>(0);

                        GenShapeActionPair TunnelSegment = new(new Shapes.Circle(Main.rand.Next(2, 3)), Actions.Chain(new GenAction[] { new Modifiers.IsSolid(), new Actions.Custom((i, j, args) => { Owner.PickTile(i, j, Owner.GetBestPickaxe().pick); return true; }), new Actions.Scanner(TooHardCount) }));

                        if (WorldUtils.Gen(p, TunnelSegment))
                        {
                            BloomRingSharp Ring = new();
                            Ring.Prepare(ExplosionPoints[i], Vector2.Zero, Color.OrangeRed, 0.06f, 0.01f, 0.2f, BlendState.Additive);
                            ParticleEngine.Particles.Add(Ring);

                            for (int j = 0; j < 5; j++)
                            {
                                Dust dust = Dust.NewDustPerfect(ExplosionPoints[i], DustID.FireworksRGB, Main.rand.NextVector2Circular(3f, 3f), 0, Color.OrangeRed, 2f);
                                dust.noGravity = true;

                                PixelParticle pixel = new();
                                pixel.Initialize(ExplosionPoints[i], Main.rand.NextVector2Circular(1f, 1f), Color.OrangeRed, 2f);
                                ParticleEngine.Particles.Add(pixel); 
                            }

                            SoundEngine.PlaySound(SoundID.Item14);
                            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion);
                        }


                        Rectangle DMG = Utils.CenteredRectangle(ExplosionPoints[i], new Vector2(48, 48));

                        foreach(NPC N in Main.ActiveNPCs)
                        {
                            if (N.Hitbox.Intersects(DMG))
                            {
                                N.SimpleStrikeNPC(Projectile.damage, Math.Sign(Projectile.Center.DirectionTo(Main.MouseWorld).X), false, 4f, DamageClass.Generic, true);
                            }
                        }
                    }

                    if (Idx >= ExplosionPoints.Length)
                    {

                        Projectile.Kill();
                    }
                }
            }
        }
    }
}
