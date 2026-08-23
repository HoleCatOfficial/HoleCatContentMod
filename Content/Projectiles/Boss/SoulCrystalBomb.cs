using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
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

namespace DestroyerTest.Content.Projectiles.Boss
{
    public class SoulCrystalBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }
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
        }

        public override bool PreDrawExtras()
        {
            SpriteBatch sb = Main.spriteBatch;

            TelegraphLine(sb);

            return false;
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            var LineTex = DTAssetLib.Streak(13, true).Value;

            Vector2[] V = Opus.GetEquidistantVectors(8, Projectile.Center, 10, 0f);

            for (int i = 0; i < V.Length; i++)
            {
                Main.EntitySpriteDraw(LineTex, Projectile.Center - Main.screenPosition, null, ColorLib.Soul2 with { A = 0 }, Projectile.Center.DirectionTo(V[i]).ToRotation(), new Vector2(0, LineTex.Height / 2), new Vector2(50f, 0.85f), SpriteEffects.None);
                Main.EntitySpriteDraw(LineTex, Projectile.Center - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, Projectile.Center.DirectionTo(V[i]).ToRotation(), new Vector2(0, LineTex.Height / 2), new Vector2(50f, 0.2f), SpriteEffects.None);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, Color.White, ColorLib.Soul, TextureRotationOffset, 2f);
        }
        

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }


        public float TextureRotationOffset = 0f;
        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            TextureRotationOffset -= 0.5f;
            Projectile.velocity *= 0.99f;                       
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
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
            var launchVelocity = new Vector2(-20, 0);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ModContent.ProjectileType<SoulCrystal>(), 15, 1);
            }
        }
    }
}