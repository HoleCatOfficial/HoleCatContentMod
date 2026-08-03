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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class TwistedFaithProj : SpinningSlash
    {
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Blending = true;
            themeColor = new Color(184, 45, 117);
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 600);
        }
    }
}