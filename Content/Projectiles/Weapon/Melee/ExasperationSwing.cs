using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
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

    public class ExasperationSwing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 116;
            Projectile.height = 116;
            SweepColor = Color.DarkMagenta;
            SwingSpeed = 0.22f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.TenebrisSwing with { PitchVariance = 0.6f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            npc.AddBuff(ModContent.BuffType<Withering>(), 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.HellWeaponImpact with { MaxInstances = 0, PitchVariance = 0.8f }, npc.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.LightMetalHit with { MaxInstances = 0, Pitch = -0.6f, PitchVariance = 1f, Volume = 0.6f }, npc.Center);

            Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShadowExplosion2>(), Projectile.damage / 3, 10, Owner.whoAmI);
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

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, Color.Black * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, Color.DarkMagenta * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);

            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {

        }

        public override void OnStartSwing()
        {
            Vector2 tocur = (Main.MouseWorld - Owner.Center);
            tocur.Normalize();
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, tocur * (8 + Owner.GetTotalAttackSpeed(DamageClass.Melee)), ModContent.ProjectileType<ExasperationDart>(), (int)(Projectile.damage * 0.6f), 6, Owner.whoAmI);
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public int Counter = 0;
        public override void ExtraEffects()
        {
            Counter++;




            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[15..30];

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, Color.DarkMagenta * 0.5f, 3f);
                //PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, ColorLib.Wretched3, 0.5f, 20, ai2: 2);
            }

            ScaleMult = 1.22f;

            SparkEdge(Main.player[Projectile.owner], 1f, Color.DarkMagenta);
        }
    }
}