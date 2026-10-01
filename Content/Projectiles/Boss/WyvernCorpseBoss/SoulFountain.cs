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
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulFountain : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<WyvernCorpseHead>();
        }
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        float Mult = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Mult = Opus.Sine(1f, 1.1f, 0.3f);


            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Bottom - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(3f, 6f) * Mult, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Bottom - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(3f, 5f) * Mult, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.ThinGlowCone.Value, Projectile.Bottom - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, -MathHelper.PiOver2, new Vector2(0f, DTAssetLib.ThinGlowCone.Value.Height / 2), new Vector2(3f, 6f) * Mult, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0]++;

            Spark spark = new();
            spark.PrepareSpark(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-100, 100), 0), new Vector2(Main.rand.NextFloat(-2, 2), -30), 0f, ColorLib.Soul, 2f, false, 60, SparkDrawMode.Additive, 2f);
            ParticleEngine.BehindProjectiles.Add(spark);

            Spark spark2 = new();
            spark2.PrepareSpark(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-100, 100), 0), new Vector2(Main.rand.NextFloat(-2, 2), -30), 0f, Color.White, 2f, false, 60, SparkDrawMode.Additive, 2f);
            ParticleEngine.BehindProjectiles.Add(spark2);


            if (Projectile.ai[0] % 20 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Bottom + new Vector2(Main.rand.NextFloat(-100, 100), 0), new Vector2(Main.rand.NextFloat(-12, 12), -20), ModContent.ProjectileType<SoulVolley>(), (int)(Projectile.damage * 0.1f), 4);
            }

            Lighting.AddLight(Projectile.Center, ColorLib.Soul3.ToVector3() * Mult);
        }

    }
}
