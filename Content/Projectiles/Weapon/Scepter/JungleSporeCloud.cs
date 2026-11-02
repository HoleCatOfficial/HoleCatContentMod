
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class JungleSporeCloud : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override bool PreDraw(ref Color lightColor)
        {


            return false;
        }

        public override void AI()
        {
            TintableSmoke Smoke = new();
            Smoke.CreateWithBlending(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(1.3f, 1.3f), Color.GreenYellow, 0.5f, 1.4f, 60, PixelLayer.AboveTiles, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Smoke);


            Projectile.velocity *= 0.93f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 480);
        }

        public override void OnKill(int timeLeft)
        {

        }


    }

}