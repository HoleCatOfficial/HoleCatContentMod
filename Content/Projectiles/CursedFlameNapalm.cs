using System;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
{
	public class CursedFlameNapalm : ModProjectile
	{
		public override string Texture => "DestroyerTest/Content/Particles/ParticleDrawEntity";

        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
            Projectile.height = 40; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = true;
            Projectile.alpha = 160;
        }
        public bool DrawTrail = true;
        public bool fading = false;
        public bool justSpawned = true;
        public float velocityLength;
        public Vector2 scale;

        public override bool PreDraw(ref Color lightColor)
        {
            DTUtils Utility = new DTUtils();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D trail = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Trail3").Value;
            Texture2D MainTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/GlowCircle").Value;

            // Calculate a scale based on velocity

             // X-axis is stretched, Y-axis stays normal

            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(
                trail,
                Projectile.Center - Main.screenPosition, // draw at projectile's center
                null,
                ColorLib.CursedFlames,
                Projectile.velocity.ToRotation() + MathHelper.PiOver2,
                new Vector2(trail.Width / 2, trail.Height / 2), // origin is left-center so it stretches correctly
                scale,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                MainTexture,
                Projectile.Center - Main.screenPosition,
                null,
                ColorLib.CursedFlames,
                Projectile.velocity.ToRotation(),
                new Vector2(MainTexture.Width / 2, MainTexture.Height / 2),
                0.2f,
                SpriteEffects.None,
                0
            );

            Utility.ReturnToDefaultDrawing(spriteBatch);



            return true;
        }


        public float smoothedVelocity;

        public override void OnSpawn(IEntitySource source)
        {
            float targetY = smoothedVelocity * 0.4f;
            scale.Y = MathHelper.Lerp(scale.Y, targetY, 0.1f);
        }

        public override void AI()
        {
            float currentSpeed = Projectile.velocity.Length();

            smoothedVelocity = MathHelper.Lerp(smoothedVelocity, currentSpeed, 0.2f);

            scale = new Vector2(0.55f, smoothedVelocity * 0.4f);


            // Default target scale based on speed
            float targetY = smoothedVelocity * 0.4f;
            scale = new Vector2(0.45f, targetY);

            if (justSpawned)
            {
                // Start small and grow into target scale
                scale.Y = MathHelper.Lerp(scale.Y, targetY, 0.1f);

                // Once it's close enough, stop the fade-in
                if (Math.Abs(scale.Y - targetY) < 0.01f)
                    justSpawned = false;
            }

            if (fading)
            {
                scale.Y = MathHelper.Lerp(scale.Y, 0f, 0.1f);
                if (scale.Y < 0.01f) fading = false;
            }

            int[] types = new int[]
            {
                PRTLoader.GetParticleID<ColoredFire1>(),
                PRTLoader.GetParticleID<ColoredFire2>(),
                PRTLoader.GetParticleID<ColoredFire3>(),
                PRTLoader.GetParticleID<ColoredFire4>(),
                PRTLoader.GetParticleID<ColoredFire5>(),
                PRTLoader.GetParticleID<ColoredFire6>(),
                PRTLoader.GetParticleID<ColoredFire7>()
            };

            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Projectile.Center, Vector2.Zero, ColorLib.CursedFlames, 1.5f);
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.ai[0] = 5f;
                Projectile.velocity.Y += 0.15f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            DrawTrail = false;
            fading = true;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 240);
        }
	}

	
}