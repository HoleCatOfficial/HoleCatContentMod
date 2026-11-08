using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class DarkWaterThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            Projectile.rotation += 0.5f * Projectile.direction;
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());

            if (LifeTime < 30)
            {

            }
            else
            {
                if (Main.GameUpdateCount % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { }, Projectile.Center);
                }

                Projectile.velocity.Y += 0.4f;



            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { PitchVariance = 0.3f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Shatter with { PitchVariance = 0.3f, Pitch = -1 }, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkWaterDust>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}