using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class MoonlordProjectileFX : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[ProjectileID.PhantasmalBolt] = 2;
            ProjectileID.Sets.TrailCacheLength[ProjectileID.PhantasmalBolt] = 80;
        }

        float offset = 0f;
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!DTVanillaChangesConfig.instance.BossRework)
            {
                return base.PreDraw(projectile, ref lightColor);
            }   
            if (projectile.type == ProjectileID.PhantasmalBolt)
            {
                offset -= 0.02f;
                DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2).Value, projectile.OldCenter().ToList(), projectile.oldRot.ToList(), 30f, Color.Aquamarine, offset);

                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, projectile.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, projectile.rotation, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 2f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, projectile.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, projectile.rotation + MathHelper.PiOver2, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.6f, 0.2f), SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, projectile.rotation + MathHelper.PiOver2, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.6f, 0.2f) * 0.5f, SpriteEffects.None);

                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void AI(Projectile projectile)
        {
            if (!DTVanillaChangesConfig.instance.BossRework)
            {
                return;
            }

            if (projectile.type == ProjectileID.PhantasmalBolt)
            {
                projectile.ResetExcessTrailPoints();

                /*
                Spark spark2 = new();
                spark2.PrepareSpark(projectile.Center + new Vector2(0, Opus.Sine(-6f, 6f, 0.3f)).RotatedBy(projectile.velocity.ToRotation()), projectile.velocity * 0.1f, projectile.velocity.ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.1f, false, 9, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(spark2);

                Spark spark3 = new();
                spark3.PrepareSpark(projectile.Center + new Vector2(0, Opus.Sine(6f, -6f, 0.3f)).RotatedBy(projectile.velocity.ToRotation()), projectile.velocity * 0.1f, projectile.velocity.ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.1f, false, 9, SparkDrawMode.Additive, 3f);
                ParticleEngine.Particles.Add(spark3);
                */

                return;
            }
        }
    }
}
