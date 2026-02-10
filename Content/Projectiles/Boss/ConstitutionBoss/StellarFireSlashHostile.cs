using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
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

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class StellarFireSlashHostile : SpinningSlash
    {
        public int UpDown => (int)Projectile.ai[0];
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 170;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Blending = true;
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 0.5f;
        }

        public float stellarFireTime;
        public override bool PreAI()
        {
            stellarFireTime += 0.025f;
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);
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
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }
    }
}