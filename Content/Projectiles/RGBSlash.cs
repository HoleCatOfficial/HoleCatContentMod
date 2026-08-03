using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DestroyerTest.Content.Buffs;
using Microsoft.Build.Evaluation;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles
{
    public class RGBSlash : LinearSlash
    {
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Blending = true;
            themeColor = Main.DiscoColor;
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 600);
        }
    }
}