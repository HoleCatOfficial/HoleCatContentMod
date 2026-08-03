
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
 
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
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
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class Endemy2Swing : BaseBroadswordProjectile
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 142;
            SweepColor = Color.Goldenrod;
            SweepHighlightColor = Color.Bisque;
            UsesDefaultSweepFX = true;
            SweepScale = 2.6f;

            WaitTimeMultiplier = 1.3f;
            SwingSpeed = 0.15f;
            

            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Highlight");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(BuffID.BrokenArmor, 300);
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { MaxInstances = 0, PitchVariance = 0.4f, Pitch = -0.7f }, npc.Center);
            BloomRingSharp Ring = new();
            Ring.Prepare(npc.Center, Vector2.Zero, Color.Goldenrod, 0.07f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);
        }

       
        public override void DrawUnderBlade()
        {

        }
        public override void DrawOverBlade()
        {

        }

        public override void ExtraEffects()
        {



        }
    }
}
