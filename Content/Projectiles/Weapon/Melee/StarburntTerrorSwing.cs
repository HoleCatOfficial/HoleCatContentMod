using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
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

    public class StarburntTerrorSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 52;
            Projectile.height = 50;
            SweepColor = ColorLib.StellarFireGradientLooping();
            SwingSpeed = 0.17f;

            //Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MagicSwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            npc.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);

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
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, Color.Lerp(Color.Orange, Color.OrangeRed, SlashProgress) * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
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

        public override void OnStartSwing()
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (toMouse * (7 * Owner.GetTotalAttackSpeed(DamageClass.Melee))), ModContent.ProjectileType<HotHeadPumpkin>(), Projectile.damage, 5, Projectile.owner);
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                //Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.CursedFlames * 0.5f, 3f);
                //PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, ppt[Main.rand.Next(15)], SwordLine.GetLineRotation.ToRotationVector2() * 2, default, 0.5f);
            }

            ScaleMult = 1.25f;

            //SparkEdge(Main.player[Projectile.owner], 1f, ColorLib.Wretched3);
        }
    }
}