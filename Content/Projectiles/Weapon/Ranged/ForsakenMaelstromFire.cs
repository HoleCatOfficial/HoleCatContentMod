using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class ForsakenMaelstromFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 9)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override void AI()
        {
            AnimateProjectile();

            TintableSmoke Smoke = new();
            Smoke.CreateWithBlending(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f), Color.MediumPurple, 0.5f, Main.rand.NextFloat(0.1f, 1f), 100, PixelLayer.AbovePlayer, BlendState.Additive, true);
            ParticleEngine.BehindProjectiles.Add(Smoke);
        }




        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(ModContent.BuffType<SoulErosion>(), 300);


        }
    }

}