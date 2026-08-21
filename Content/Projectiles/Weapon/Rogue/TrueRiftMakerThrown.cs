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
 
using DestroyerTest.Content.Particles;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class TrueRiftMakerThrown : ModProjectile
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
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public void DustSpawn1()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, -16).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, 16).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.75f);

            Dust trail1 = Dust.NewDustPerfect(DustPos, DustID.FireworksRGB, Projectile.velocity * 0.05f, 0, ColorLib.Rift, 0.75f);
            trail1.noGravity = true;
        }

        public void DustSpawn2()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, 16).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, -16).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.75f);

            Dust trail2 = Dust.NewDustPerfect(DustPos, DustID.FireworksRGB, Projectile.velocity * 0.05f, 0, ColorLib.Rift, 0.75f);
            trail2.noGravity = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            DustSpawn1();
            DustSpawn2();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Opus.DrawProjectileShadowsRotating(Projectile, 3, Color.Black, 0.2f);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BigRiftExplosion>(), Projectile.damage / 2, 10f, Projectile.owner);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 bottomLeft = new Vector2(projHitbox.X, projHitbox.Y + projHitbox.Height);
            Vector2 topRight = new Vector2(projHitbox.X + projHitbox.Width, projHitbox.Y);
            return targetHitbox.Intersects(new Rectangle((int)bottomLeft.X, (int)bottomLeft.Y, (int)(topRight.X - bottomLeft.X), (int)(bottomLeft.Y - topRight.Y)));
        }

    }
}