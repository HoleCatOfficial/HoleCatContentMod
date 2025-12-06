using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class BigSoul : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28; // The width of projectile hitbox
            Projectile.height = 56; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type];
            DTUtils Utility = new DTUtils();

            // Calculate source rectangle for current frame
            int frameHeight = texture.Value.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, Projectile.frame * frameHeight, texture.Value.Width, frameHeight);

            Vector2 origin = new Vector2(texture.Value.Width / 2f, frameHeight / 2f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, Color.Purple, false, 0f);
            sb.Draw(texture.Value, drawPos, sourceRect, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(sb);

            return false;
        }

        public override void AI()
        {
            AnimateProjectile();
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.DemonTorch, 0, 0, 70, default, 1.0f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            int numProjectiles = 20;
            float rotationStep = MathHelper.TwoPi / numProjectiles;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TPKill") with { Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = new Vector2(8f, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<TormentedSoul2>(),
                    Projectile.damage,
                    Projectile.knockBack
                );
            }

            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = new Vector2(7f, 0f).RotatedBy(rotationStep * (i + 0.5f));
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<TormentedSoul2>(), 
                    Projectile.damage / 2,
                    Projectile.knockBack
                );
            }

            /*
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = new Vector2(8f, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<TormentedSoul2>(),
                    Projectile.damage / 2,
                    Projectile.knockBack
                );
            }
            */

            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.DemonTorch, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, default, 1);
        }


    }
}