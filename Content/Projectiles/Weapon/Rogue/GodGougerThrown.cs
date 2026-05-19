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

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class GodGougerThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Projectile.velocity * 0.2f, 100, Color.PaleTurquoise, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + new Vector2(0, 3f) - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + new Vector2(0, -3f) - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + new Vector2(3f, 0) - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + new Vector2(-3f, 0) - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 600);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TPHit") with { MaxInstances = 0, PitchVariance = 0.2f, Pitch = -0.8f }, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, -Projectile.velocity.RotatedBy(0.5f), ModContent.ProjectileType<GodGougerMiniPink>(), 6, 10, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, -Projectile.velocity.RotatedBy(-0.5f), ModContent.ProjectileType<GodGougerMiniTeal>(), 6, 10, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.Pink, 1f, 2.4f);
            Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.PaleTurquoise, 1f, 2.4f);
            return true;
        }
    }
}