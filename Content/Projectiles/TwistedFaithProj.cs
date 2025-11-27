using System;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DestroyerTest.Content.Buffs;
using Microsoft.Build.Evaluation;

namespace DestroyerTest.Content.Projectiles
{
    public class TwistedFaithProj : ModProjectile
    {
        public override void SetStaticDefaults() {
			Main.projFrames[Type] = 4;
		}
        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 170;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

		public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(184, 45, 117);

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

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor * 0.5f,
                Projectile.rotation,
                origin,
                Projectile.scale * 1.15f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor * 0.35f,
                Projectile.rotation + 0.5f,
                origin,
                Projectile.scale * 1.05f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor * 0.35f,
                Projectile.rotation - 0.5f,
                origin,
                Projectile.scale * 1.05f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale * 0.35f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 1f + player.GetAttackSpeed(DamageClass.Melee);
        }

        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 60) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            if (Projectile.timeLeft > 80)
            {
                Projectile.alpha -= (int)(255f / 40f);
            }
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += (int)(255f / 40f);;
            }

            Projectile.rotation += 0.8f * Projectile.direction;

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 8; i++) //Fuck off about lag. I know what I'm doing.
                {
                    Vector2 rand = Projectile.Center + Main.rand.NextVector2Circular(Projectile.Hitbox.Width / 2, Projectile.Hitbox.Height / 2);

                    // Direction straight outward from the projectile
                    Vector2 outward = Vector2.Normalize(rand - Projectile.Center);

                    // Then rotate it by the projectile's rotation to give that tangential offset
                    Vector2 spiralDir = outward.RotatedBy(Projectile.rotation);

                    // Scale to taste
                    Vector2 velocity = spiralDir * 2f;

                    Dust.NewDustPerfect(rand, DustID.FireworksRGB, velocity, 0, new Color(184, 45, 117), 0.5f);

                }
            }

            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2) {
				Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 240);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 240);
        }

    }
}