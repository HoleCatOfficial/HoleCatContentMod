using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class RainbowSlash : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.3f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.15f, SpriteEffects.None);
            return true;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
