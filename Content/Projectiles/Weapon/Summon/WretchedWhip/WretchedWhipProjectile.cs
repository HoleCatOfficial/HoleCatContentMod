
using System;
using System.Collections.Generic;
using BreadLibrary.Common.Whip;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Buffs.Whip;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon.WretchedWhip
{
    public class WretchedWhipProjectile : BaseWhipProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        #region IwhipMOtion
        protected override IWhipMotion CreateMotion()
        {
            return new WhipMotions.VanillaWhipMotion();
        }

        public override SoundStyle? WhipCrack_SFX => new SoundStyle(DTAssetLib.AudioPath + "/WretchedWhip/Snap") { MaxInstances = 0, PitchVariance = 0.4f };
        protected override void SetupModifiers(ModularWhipController controller)
        {
            //controller.AddModifier(new WhipModifiers.TwirlModifier(4, 12, 0.05f* Projectile.spriteDirection));
            //controller.AddModifier(new WhipModifiers.SmoothSineModifier(6, 30, 8f, 4f, 1f, Direction: Projectile.spriteDirection));
        }
        #endregion
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.aiStyle = -1;
            
        }

        public override void Prepare()
        {
            AddHitEffects(BuffID.CursedInferno, 600);

            WhipController = new ModularWhipController(CreateMotion());

            SetupModifiers(WhipController);

            Projectile.WhipSettings.Segments = 20;
        }

        public bool PowerStrike = false;
        public override void AI2()
        {
            if (HitCooldown > 0)
            {
                HitCooldown--;
            }

            if (PowerStrike)
            {
                SoundEngine.PlaySound(SoundID.LiquidsWaterLava with { Volume = 0.5f }, Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1]);
                for (int i = 0; i < 2; i++)
                {
                    LerpingFire backfire = new();
                    backfire.PrepareFire(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), OpusColorUtils.Darken(ColorLib.Wretched6, 0.6f), ColorLib.Wretched7, 1f, 30, FireDrawMode.AlphaBlend);
                    ParticleEngine.BehindProjectiles.Add(backfire);

                    LerpingFire fire = new();
                    fire.PrepareFire(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(0.01f, 0.2f), ColorLib.WretchedColorMap, 0.5f, 30, FireDrawMode.Additive);
                    ParticleEngine.BehindProjectiles.Add(fire);
                }
            }

            WretchedPointGlow glow = new();
            glow.Prepare(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1], Main.rand.NextVector2Circular(1, 1), 1f);
            ParticleEngine.Particles.Add(glow);
            
        }

        public override void PlayCrackSound()
        {
            LerpingBloomRingSharp Ring = new();
            Ring.Prepare(Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1], Vector2.Zero, ColorLib.WretchedColorMap, 0.05f, 0.01f, 0.5f);
            ParticleEngine.Particles.Add(Ring);

            SoundEngine.PlaySound(WhipCrack_SFX, Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1]);


        }

        public override void OnTipHit(Entity target)
        {
            if (PowerStrike)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { Volume = 0.6f }, target.Center);
                SoundEngine.PlaySound(DTAssetLib.Impacts.HeavyCrit with { PitchVariance = 0.4f, Pitch = -0.6f }, target.Center);

                for (int i = 0; i < 7; i++)
                {
                    WretchedPointGlow glow = new();
                    glow.Prepare(target.Center, Main.rand.NextVector2Circular(3, 3), 1f);
                    ParticleEngine.Particles.Add(glow);
                }

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<MalevolenceBolt>(), 3, target.Center, Projectile.damage, 4, 9, offset: Main.rand.NextFloat(MathHelper.TwoPi));

                Owner.AddBuff(ModContent.BuffType<WretchedWhipSpeedBoost>(), 300);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.HeavyCrit with { PitchVariance = 0.4f }, target.Center);
            }
        }

        bool f1 = false;
        protected override void UpdateWhip(float progress)
        {
            base.UpdateWhip(progress);




            Projectile.GetWhipSettings(base.Projectile, out var timeToFlyOut, out var segments, out var rangeMultiplier);
            float Progress = MathHelper.Clamp((float)Time / timeToFlyOut, 0f, 1f);

            float OnePointProgress = (float)Math.Round(Progress, 1);
            float TwoPointProgress = (float)Math.Round(Progress, 2);
            //Main.NewText($"Progress: {OnePointProgress}, 2PProgress: {TwoPointProgress}");

        }

        public int HitCooldown = 0;

        public override bool? CanHitNPC(NPC target)
        {
            return HitCooldown <= 0 && target.friendly == false;
        }

        float Pitch = -2f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HeldItem.ModItem is SummonItems.WretchedWhip Whip)
            {
                Whip.HitCount++;
                Pitch = MathHelper.Lerp(-3f, -2f, (float)Whip.HitCount / 5f);
            }

            

            

            if (PowerStrike)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { Volume = 0.6f }, target.Center);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 4f }, target.Center);
                SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 4f }, target.Center);

                for (int i = 0; i < 7; i++)
                {
                    WretchedPointGlow glow = new();
                    glow.Prepare(target.Center, Main.rand.NextVector2Circular(3, 3), 1f);
                    ParticleEngine.Particles.Add(glow);
                }

                Owner.AddBuff(ModContent.BuffType<WretchedWhipSpeedBoost>(), 300);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { Pitch = Pitch, Volume = 1.6f }, target.Center);
            }

            HitCooldown = 60;
        }

        #region Drawing
        public override float GetWhipWidth(float baseWidth, float t)
        {
            _HeadOffset = new Vector2(0, -_HeadRectangle.Height / 2f);
            _DebugMode = false;
            _ShouldDrawNormal = true;
            _Head_VerticalFrames = 1;
            baseWidth = 2;
            return baseWidth;
        }
        protected override float RenderSpacing => 10f;
        public override float _PrimitiveScrollRate() => -1f;
        public override Color GetWhipColor(float t, float w)
        {
            Projectile.alpha = 0;
            return ColorLib.Wretched2;
            //return Color.Lerp(Color.White, Color.Blue, MathF.Sin(Main.GlobalTimeWrappedHourly) * MathF.Cos(t * 10f));
        }


        public float Saturate(float x)
        {
            if (x > 1f)
                return 1f;
            if (x < 0f)
                return 0f;
            return x;
        }

        public string Path = "DestroyerTest/Content/Projectiles/Weapon/Summon/WretchedWhip";
        protected override void DrawOverPrimitive(List<Vector2> points)
        {
            //_Head_y = (int)(5 * Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)));


            Texture2D tex = ModContent.Request<Texture2D>($"{Path}/WretchedWhip_MidChain1").Value;

            float whipLength = Projectile.WhipSettings.Segments;

            float spacingPixels = tex.Width;

            int count = Math.Max(1, (int)(whipLength / spacingPixels));

            Vector2 End = GetPointAlongWhip(points, whipLength);


            // shared sliding parameter
            float slide = 1;

            for (int i = 0; i < count; i++)
            {
                tex = ModContent.Request<Texture2D>($"{Path}/WretchedWhip_MidChain{(i % 2 == 0 ? 1 : 2)}").Value;

                // fixed offset per element
                float offset = i / (float)count;

                // sliding + offset
                float t = (slide + offset) % 1f;

                Vector2 point = GetPointAlongWhip(points, t);

                float rot = GetRotationAlongWhip(points, t);

                Color color = Color.White;

                Main.EntitySpriteDraw(tex, point - Main.screenPosition, null, color, rot, tex.Size() / 2, 1f, 0);
            }
        }
        public override bool _PrimitiveIsScrollingTexture => true;
        protected override Asset<Texture2D> PrimitiveTex => DTAssetLib.Square;

        protected override Asset<Texture2D> WhipHandle => ModContent.Request<Texture2D>($"{Path}/WretchedWhipHilt");
        protected override Asset<Texture2D> WhipHead => ModContent.Request<Texture2D>($"{Path}/WretchedWhipHead");

        #endregion
    }
}