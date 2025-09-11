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
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisLance : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 26; // The width of projectile hitbox
            Projectile.height = 102; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 120; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, newColor: ColorLib.TenebrisGradient, Scale: 1.8f, Velocity: Vector2.Zero);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 FlankLeft = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 FlankRight = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            if (Main.GameUpdateCount % 10 == 0 && Projectile.velocity.Length() > 2)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.45f }, Projectile.Center);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankLeft * 0.02f, ModContent.ProjectileType<TenebrisDart>(), Projectile.damage / 2, 3);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankRight * 0.02f, ModContent.ProjectileType<TenebrisDart>(), Projectile.damage / 2, 3);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.TintableDustLighted, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, ColorLib.TenebrisGradient, 1);
            int numProjectiles = 12;
            float rotationStep = MathHelper.TwoPi / numProjectiles;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = new Vector2(8f, 0f).RotatedBy(rotationStep * i);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<TenebrisDart>(),
                    Projectile.damage / 2,
                    Projectile.knockBack
                );
            }
        }

    }
}