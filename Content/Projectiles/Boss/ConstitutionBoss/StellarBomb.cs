using System;
using System.Xml;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class StellarBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public Color MainColor = Color.White;
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.FlatStar.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFireGradientLooping(), 0f, DTAssetLib.FlatStar.Value.Size() / 2, WarnScale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, MainColor, Color.White, TextureRotationOffset, 1f);
        }

        public float TextureRotationOffset = 0f;

        public int Lifetime = 120;
		public int Time = 0;

		public bool StartKill = false;
		public void UpdateLerpTime()
		{
			Time++;

			if (Time > Lifetime)
			{
				StartKill = true;
			}
		}
		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime <= 0)
				{
					return 0f;
				}

				return (float)Time / (float)Lifetime;
			}
		}

        float WarnScale = 0f;

        public override void AI()
        {
            UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion * 8f);

            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            TextureRotationOffset -= 0.5f;
            Projectile.velocity *= 0.999f;

            if (StartKill)
            {
                WarnScale += 0.1f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            DTUtils Utility = new DTUtils();

            Opus.RadialSpreadProjectile(ModContent.ProjectileType<HollowStar>(), 5, Projectile.Center, Projectile.damage, (int)Projectile.knockBack, 20, offset: 0);

            Vector2 Outward = new Vector2(0, -1).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 6);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, Outward.X, Outward.Y, 100,  ColorLib.StellarFireGradientLooping(), 1.5f).noGravity = true;
            }

            DTUtils.ConstitutionStarExplosionEffects(Projectile);

            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && Vector2.Distance(player.Center, Projectile.Center) < 150)
                {
                    player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), (int)(Projectile.damage * 1.75f), 0);
                    player.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
                }
            }
        }
    }
}