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

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class RiftMaker_Thrown : ModProjectile
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
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.2f, 100, ColorLib.Rift, 1.2f);
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsRotating(Projectile, 8, ColorLib.Rift);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/RiftMaker_Boom") with { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);
			PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Projectile.Center, Vector2.Zero, ColorLib.Rift, 1);
            Opus.RadialDustRandomDir(DustID.FireworksRGB, 13, Projectile.Center, 0, ColorLib.Rift, 1f, 2.4f);
        }

    }
}