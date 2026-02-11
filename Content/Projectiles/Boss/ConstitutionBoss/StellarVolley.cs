using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
	public class StellarVolley : ModProjectile
	{
		private NPC NPCTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		private Player PLRTarget
		{
			get => Projectile.ai[1] == 0 ? null : Main.player[(int)Projectile.ai[1] - 1];
			set
			{
				Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 72;
			Projectile.height = 72;

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

        public Color MainColor = Color.White;
        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);
		public override bool PreDraw(ref Color lightColor)
        {
            lightColor =  MainColor;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int k = TrailPositions.Count - 1; k > 0; k--)
            {
                Vector2 drawPos = TrailPositions[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((TrailPositions.Count - k) / (float)TrailPositions.Count);
                Main.EntitySpriteDraw(
                    ProjTex.Value,
                    drawPos,
                    null,
                    color,
                    Projectile.rotation,
                    ProjTex.Size() / 2f,  // proper origin
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Opus.DrawTextureOnProj(ProjTex, Projectile, Color.White, true, Projectile.rotation, 1f, 1f);

            return false;
        }


		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;

        public int Lifetime = 580;
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
		public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);


            UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion * 8f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center,  MainColor.ToVector3() * 0.2f);

            Projectile.velocity.Y += 0.1f;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			
			target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
			
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);
        }

    }
}