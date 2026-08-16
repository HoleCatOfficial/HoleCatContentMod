
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
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
            UsesDefaultSweepFX = true;
            SweepScale = 2.1f;
            Projectile.extraUpdates = 3;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.MetalSwing with { MaxInstances = 0, PitchVariance = 0.6f };
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
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
           
        }
        public override void DrawUnderBlade()
        {
           
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


            StellarPointGlow Glow = new StellarPointGlow();
            Glow.Initialize(ppt[Main.rand.Next(20)], SwordLine.GetLineRotation.ToRotationVector2() * 2, default, 2f);
            ParticleEngine.BehindProjectiles.Add(Glow);
            

            ScaleMult = 2f;

            SweepColor = SweepHighlightColor = ColorLib.StellarFireGradient(SlashProgress);

            SparkEdge(Main.player[Projectile.owner], 1f, SweepColor);
        }
    }
}
