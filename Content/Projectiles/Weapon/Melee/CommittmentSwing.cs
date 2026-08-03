using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
 
using InnoVault.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class CommittmentSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 116;
            Projectile.height = 106;
            SweepColor = Main.DiscoColor;
            SwingSpeed = 0.08f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing with { Pitch = Pitch};

        public float Pitch = -1;
        public int HitCount = 0;
        public int HitTimer = 20;
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);
                npc.AddBuff(ModContent.BuffType<LightInferno>(), 600);
            }
            SoundEngine.PlaySound(DTAssetLib.Impacts.AmbitionChargeBurst with { MaxInstances = 0, Pitch = Pitch }, npc.Center);
            Vector2 toTarg = npc.Center - Owner.Center;
            toTarg.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, (toTarg * (7 * Owner.GetTotalAttackSpeed(DamageClass.Melee))), ModContent.ProjectileType<ContinuumStar>(), Projectile.damage / 2, 5, Projectile.owner);
            if (HitCount < 20)
            {
                HitCount++;
            }
            HitTimer = 60;
        }

        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3").Value;
            var TexH = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3Highlight").Value;
            float TexBasedMod = (Projectile.Size.Length() * 0.015f);
            float rOffset = 0f;

            SpriteEffects FX = SpriteEffects.None;

            if (LastSwing == 1)
            {
                FX = SpriteEffects.FlipHorizontally;
                rOffset = MathHelper.PiOver2;
            }
            else
            {
                FX = SpriteEffects.None;
                rOffset = 0f;
            }

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, Main.DiscoColor * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, Color.White * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            //Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, swordTip, null, ColorLib.Wretched2, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, 2f, SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public int SwingCount = 0;
        public override void OnStartSwing()
        {
            if (HitTimer <= 0)
            {
                if (HitCount > 0)
                {
                    HitCount--;
                }
            }
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            /*
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.RainbowGradient, 3f);
                //PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, ppt[Main.rand.Next(15)], SwordLine.GetLineRotation.ToRotationVector2() * 2, default, 0.5f);
            }
            */

            if (HitTimer > 0)
            {
                HitTimer--;
            }

            float T = HitCount / 20f;
            Pitch = MathHelper.Lerp(-1, 0, T);
            ScaleMult = MathHelper.Lerp(1f, 2.2f, T);
            SwingSpeed = MathHelper.Lerp(0.08f, 0.2f, T);
            

            

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}