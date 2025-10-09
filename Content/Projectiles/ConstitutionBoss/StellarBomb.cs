using System;
using System.Xml;
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

namespace DestroyerTest.Content.Projectiles.ConstitutionBoss
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

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center);
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                ColorLib.StellarColor,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                0.2f,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
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
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            DTUtils Utility = new DTUtils();

            Utility.RadialSpreadProjectile(ModContent.ProjectileType<HollowStar>(), 5, Projectile.Center, Projectile.damage, (int)Projectile.knockBack, 20);

            Vector2 Outward = new Vector2(0, -1).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 6);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, Outward.X, Outward.Y, 100, ColorLib.StellarColor, 1.5f).noGravity = true;
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