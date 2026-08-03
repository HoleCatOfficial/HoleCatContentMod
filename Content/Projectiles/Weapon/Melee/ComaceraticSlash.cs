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
    public class ComaceraticSlash : SpinningSlash
    {
        public int UpDown => (int)Projectile.ai[0];
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Blending = true;
            themeColor = Color.DarkOrange;
            DustType = DustID.SolarFlare;
            DustUsesColor = false;
            DustScale = 0.5f;
            Projectile.scale = 1.3f;
        }

        public float Time;
        public override bool PreAI()
        {
            Time++;
            themeColor = Color.Lerp(Color.Yellow, Color.DarkOrange, Time / 240);
            return true;
        }

        public override void Rotation()
        {
            if (UpDown == 0)
            {
                Projectile.rotation += 0.8f * Projectile.direction;
            }
            else
            {
                Projectile.rotation -= 0.8f * Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }
    }
}