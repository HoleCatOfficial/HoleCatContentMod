using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class HolyMarker : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<BlessedNodeMB>();
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];

            Spark spark = new();
            spark.PrepareSpark(Projectile.Center, new Vector2(6, 0).RotatedBy(Projectile.rotation), Projectile.rotation + MathHelper.PiOver2, Main.DiscoColor, 0.5f, false, 60, SparkDrawMode.Additive, 2f);
            ParticleEngine.Particles.Add(spark);

            Spark spark2 = new();
            spark2.PrepareSpark(Projectile.Center, new Vector2(-6, 0).RotatedBy(Projectile.rotation), Projectile.rotation + MathHelper.PiOver2, Main.DiscoColor, 0.5f, false, 60, SparkDrawMode.Additive, 2f);
            ParticleEngine.Particles.Add(spark2);
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasers"), Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + new Vector2(1500, 0).RotatedBy(Projectile.rotation), (Projectile.Center + new Vector2(1500, 0).RotatedBy(Projectile.rotation)).DirectionTo(Projectile.Center) * 0.001f, ModContent.ProjectileType<BlessedLaser2>(), Projectile.damage, Projectile.knockBack);
        }
    }
}
