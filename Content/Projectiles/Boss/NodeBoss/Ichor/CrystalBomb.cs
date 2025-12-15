using System;
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

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor
{
    public class CrystalBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22; // The width of projectile hitbox
            Projectile.height = 22; // The height of projectile hitbox

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
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            TelegraphLine(sb);
            Opus.ReturnToDefaultDrawing(sb);
            return false;
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            var LineTex = DTAssetLib.Line(1).Value;
            Vector2 start = Projectile.Center;

            if (Projectile.active)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    float angle = MathHelper.TwoPi * dir / 8f;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    Vector2 drawPos = start - Main.screenPosition;
                    float length = 3600f;
                    Vector2 scale = new Vector2(1f, length / LineTex.Height);

                    SB.Draw(LineTex, drawPos, null, ColorLib.Ichor, angle + MathHelper.PiOver2, new Vector2(LineTex.Width / 2f, 0), scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            Projectile.velocity *= 0.99f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
            if (Projectile.timeLeft % 60 == 0)
            {
                if (Projectile.timeLeft >= 60)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
                    Opus.RadialSpreadDust(DustID.Ichor, 10, Projectile.Center, 0, default, 1, 8, true);
                }

                if (Projectile.timeLeft <= 60)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, Projectile.Center);
                    Opus.RadialSpreadDust(DustID.Ichor, 10, Projectile.Center, 0, default, 2, 10, true);
                }
            }
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
            var launchVelocity = new Vector2(-30, 0);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ModContent.ProjectileType<IchorNodeCrystal>(), 35, 1);
            }
        }
    }
}