using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Terraria.Audio;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class SpearOfAspirationThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Projectile.velocity * 0.2f, 100, Color.White, 1.2f);
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsRotating(Projectile, 8, Color.White);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DTUtils.GenericSparkleEffect(target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

    }
}