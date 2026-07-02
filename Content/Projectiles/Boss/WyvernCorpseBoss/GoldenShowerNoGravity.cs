using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Linq;
using System.Xml;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class GoldenShowerNoGravity : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GoldenShowerHostile);
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            for (int num122 = 0; num122 < 3; num122++)
            {
                float num123 = Projectile.velocity.X / 3f * (float)num122;
                float num124 = Projectile.velocity.Y / 3f * (float)num122;
                int num125 = 14;
                int num126 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num125, Projectile.position.Y + (float)num125), Projectile.width - num125 * 2, Projectile.height - num125 * 2, 170, 0f, 0f, 100);
                Main.dust[num126].noGravity = true;
                Dust dust2 = Main.dust[num126];
                Dust dust63 = dust2;
                dust63.velocity *= 0.1f;
                dust2 = Main.dust[num126];
                dust63 = dust2;
                dust63.velocity += Projectile.velocity * 0.5f;
                Main.dust[num126].position.X -= num123;
                Main.dust[num126].position.Y -= num124;
            }
            if (Main.rand.Next(8) == 0)
            {
                int num127 = 16;
                int num128 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num127, Projectile.position.Y + (float)num127), Projectile.width - num127 * 2, Projectile.height - num127 * 2, 170, 0f, 0f, 100, default(Color), 0.5f);
                Dust dust2 = Main.dust[num128];
                Dust dust63 = dust2;
                dust63.velocity *= 0.25f;
                dust2 = Main.dust[num128];
                dust63 = dust2;
                dust63.velocity += Projectile.velocity * 0.5f;
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 900);
        }
    }
}
