using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles.Stellar;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class BalanceBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        private void AnimateProjectile()
		{
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
		}

        public override void AI()
        {
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Projectile.direction, 0.14f, ColorLib.SoulOfLightColor, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire);
            Fire fire2 = new Fire();
            fire2.PrepareFire(Projectile.Center, Vector2.Zero, Projectile.direction, 0.14f, ColorLib.SoulOfNightColor, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );
            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(ModContent.BuffType<LightInferno>(), 300);
            target.AddBuff(ModContent.BuffType<NightInferno>(), 300);
            
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.CrystalBreak, Projectile.Center);
        }
    }
}