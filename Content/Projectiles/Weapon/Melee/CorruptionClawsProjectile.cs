
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
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
using static tModPorter.ProgressUpdate;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class CorruptionClawsProjectile : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 30;
            SweepColor = ColorLib.Wretched4;
            SwingSpeed = 0.6f;
            ScaleMult = 1f;

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => new SoundStyle("DestroyerTest/Assets/Audio/CorruptionClawsSwing") { PitchVariance = 0.4f, MaxInstances = 0 };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.CursedInferno, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.5f }, npc.Center);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings() { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = npc.Center });
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

            Main.EntitySpriteDraw(Tex, player.MountedCenter - Main.screenPosition, null, DTColorUtils.MultiLerp(SlashProgress, ColorLib.WretchedColorMap) with { A = 0 } * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
            Main.EntitySpriteDraw(TexH, player.MountedCenter - Main.screenPosition, null, Color.White with { A = 0 } * SweepOpacity, (Projectile.rotation + MathHelper.PiOver4) + rOffset, Tex.Size() / 2, (AdjustedScale * TexBasedMod), FX);
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
         

            
        }
    }
}
