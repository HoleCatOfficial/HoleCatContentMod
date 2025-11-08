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
    public class SpookySickle : ModProjectile
    {

        public int TileCollisions = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.tileCollide = false;
        }

        public int trailLength = 10;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(252, 121, 2);

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

            Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public override void AI()
        {
            Projectile.velocity *= 1.01f;

            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha -= 5;
            }
            
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.2f * Projectile.direction;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, Projectile.height / 2).RotatedBy(Projectile.rotation), DustID.Torch, null, 100, default, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 240);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<SpookySickleExplosion>(), Projectile.damage, 2);
        }

    }
}