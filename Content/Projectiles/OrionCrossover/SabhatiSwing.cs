using BreadLibrary.Core.Graphics.PixelationShit;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

namespace DestroyerTest.Content.Projectiles.OrionCrossover
{
    public class SabhatiSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            SweepColor = Color.Black;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MediumSwing with { MaxInstances = 0, PitchVariance = 0.6f };
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            npc.AddBuff(ModContent.BuffType<DescendantInferno>(), 600);
            SoundEngine.PlaySound(DTAssetLib.Impacts.StellarFox with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);

            Vector2 Sky = npc.Center + new Vector2(Main.rand.NextFloat(-200, 200), -600);
            Vector2 d = Main.MouseWorld - Sky;
            d.Normalize();

            int Damage = (int)(Projectile.damage / 16);
            Damage = (int)MathHelper.Clamp(Damage, 20, 600);

            Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), Sky, d * 7, ModContent.ProjectileType<SabhatiMeteor>(), Damage, 3, Owner.whoAmI);
        }

        public override void OnStartSwing()
        {
            Vector2 toMouse = Main.MouseWorld - Owner.MountedCenter;
            toMouse.Normalize();
            //Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, toMouse * 16, ModContent.ProjectileType<SabhatiSlash>(), Projectile.damage / 2, 9, Owner.whoAmI);
            //p.scale = 4;
        }
        private void DrawSweepFX2()
        {
            Player player = Main.player[Projectile.owner];
            var Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CircularSlash3").Value;
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
            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, ColorLib.StellarFireGradient(SlashProgress) * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }
        public override void DrawUnderBlade()
        {
            DrawSweepFX2();
        }
        public override void DrawOverBlade()
        {

        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);
            Vector2[] ppt = pt[10..30];

            for (int i = 0; i < 2; i++)
            {
                //Dust.NewDustPerfect(ppt[Main.rand.Next(15)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, ColorLib.CursedFlames * 0.5f, 3f);
                PRTLoader.NewParticle(StellarParticleIndex.PointGlow, ppt[Main.rand.Next(20)], SwordLine.GetLineRotation.ToRotationVector2() * 2, default, 1.5f, 20, ai2: 2);
            }

            ScaleMult = 1.25f;

            SweepColor = ColorLib.StellarFireGradient(SlashProgress);

            SparkEdge(Main.player[Projectile.owner], 1f, SweepColor);
        }
    }
}
