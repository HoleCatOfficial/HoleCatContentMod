using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.CorpseBoss;
using DestroyerTest.Content.Projectiles.VampireBoss;
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

namespace DestroyerTest.Content.Projectiles
{
    public class SoulCrystalBomb : ModProjectile
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
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = true;
            Projectile.alpha = 0;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
        }

        public override bool PreDrawExtras()
        {
            SpriteBatch sb = Main.spriteBatch;

            sb.End(); // End vanilla drawing
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            TelegraphLine(sb);

            sb.End(); // End additive
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            Texture2D telegraphTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/FlowerBombTelegraphLine").Value;
            Vector2 start = Projectile.Center;

            if (Projectile.active)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    float angle = MathHelper.TwoPi * dir / 8f;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    Vector2 drawPos = start - Main.screenPosition;
                    float length = 3600f;
                    Vector2 scale = new Vector2(1f, length / telegraphTexture.Height);

                    SB.Draw(telegraphTexture, drawPos, null, ColorLib.Ichor, angle + MathHelper.PiOver2, new Vector2(telegraphTexture.Width / 2f, 0), scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center);
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Texture2D texture1 = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/GlowCircle").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Cyclone2").Value;

            Main.spriteBatch.Draw(
                texture2,
                Center - Main.screenPosition,
                null,
                ColorLib.Soul,
                TextureRotationOffset,
                new Vector2(texture2.Width / 2f, texture2.Height / 2f),
                0.2f,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                texture1,
                Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                new Vector2(texture1.Width / 2f, texture1.Height / 2f),
                0.4f,
                SpriteEffects.None,
                1f
            );

            Utility.ReturnToDefaultDrawing(spriteBatch);
        }

        public float TextureRotationOffset = 0f;
        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            TextureRotationOffset -= 0.5f;
            Projectile.velocity *= 0.999f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.moveSpeed *= 0.6f;
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = -oldVelocity.X;
				}
			if (Projectile.velocity.Y != oldVelocity.Y) {
				Projectile.velocity.Y = -oldVelocity.Y;
			}
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            var launchVelocity = new Vector2(-8, 0);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ModContent.ProjectileType<SoulCrystal>(), 35, 1);
            }
        }
    }
}