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

    public class HoleCatHookSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 200;
            Projectile.height = 174;
            SweepColor = ColorLib.HoleCatFireBeige;
            SwingSpeed = 0.1f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => /*DTAssetLib.SwordSounds.TenebrisSwing*/ new SoundStyle("DestroyerTest/Assets/Audio/LCSlash") with { PitchVariance = 0.5f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            Vector2 toTarg = npc.Center - Owner.Center;
            toTarg.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, (toTarg * (12 * Owner.GetTotalAttackSpeed<DTTrueMeleeClass>())), ModContent.ProjectileType<HoleCatFireSmall>(), Projectile.damage / 2, 5, Projectile.owner);

            if (hit.Crit)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 Dir = LastSwing == 1 ? new Vector2(0, -1f) : new Vector2(0, 1f);

                    if (Owner.direction == -1)
                    {
                        Dir = LastSwing == 1 ? new Vector2(0, 1f) : new Vector2(0, -1f);
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, (Dir * (12 * Owner.GetTotalAttackSpeed<DTTrueMeleeClass>())).RotatedByRandom(0.5f), ModContent.ProjectileType<HoleCatFireSmall>(), Projectile.damage / 2, 5, Projectile.owner);
                }
            }
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

            Color C = DTColorUtils.MultiLerp(SlashProgress, ColorLib.HoleCatFireColormap);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, C * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, DTColorUtils.Pastel(C, 0.75f) * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
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
           
        }



        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);

            UpPoint = targetAngle.ToRotation() - MathHelper.ToRadians(135f + 360f);
            DownPoint = targetAngle.ToRotation() + MathHelper.ToRadians(135f - 360f);

            /*
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.RainbowGradient, 3f);
                //PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, ppt[Main.rand.Next(15)], SwordLine.GetLineRotation.ToRotationVector2() * 2, default, 0.5f);
            }
            */

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}