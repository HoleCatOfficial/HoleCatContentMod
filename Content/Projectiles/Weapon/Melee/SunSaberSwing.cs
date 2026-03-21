using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class SunSaberSwing : BaseBroadswordProjectile
    {
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 46;
            SweepColor = Color.DarkOrange;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.SpiritOfJusticeSwing;

        public override void ExtraEffects()
        {
            SparkEdge(Main.player[Projectile.owner], 1f, Color.PaleGoldenrod);
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot);
            npc.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }
    }
}