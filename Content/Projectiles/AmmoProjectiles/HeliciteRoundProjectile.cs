using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class HeliciteRoundProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        float Scl = 1f;
        float off = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            off -= 0.08f;
            //Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 8f, color, off);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl * 1.4f, 0.5f) * 0.10f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl * 1.4f, 0.5f) * 0.05f, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.10f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.05f, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {

            Projectile.ResetExcessTrailPoints();

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);
            Lighting.AddLight(Projectile.Center, color.ToVector3());

            Scl = Opus.Sine(0.8f, 0.4f, 0.6f);
          
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}