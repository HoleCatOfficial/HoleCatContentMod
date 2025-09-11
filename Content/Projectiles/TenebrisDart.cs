using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
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
    public class TenebrisDart : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;
            DTUtils Utility = new DTUtils();
            SpriteBatch sb = Main.spriteBatch;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Utility.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            if (WaitTimer < 20)
            {
                Main.EntitySpriteDraw(DTAssetLib.FadeLine.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, Projectile.rotation + MathHelper.PiOver2, new Vector2(DTAssetLib.FadeLine.Value.Width / 2, DTAssetLib.FadeLine.Value.Height / 2), 2, SpriteEffects.None, 0);
            }
            Utility.ReturnToDefaultDrawing(sb);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public int WaitTimer = 0;
        public bool SoundFlag = false;
        public override void AI()
        {
            if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, newColor: ColorLib.TenebrisGradient, Scale: 1.8f, Velocity: Vector2.Zero);
                }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (WaitTimer < 20)
            {
                WaitTimer++;
            }

            if (WaitTimer >= 20)
            {
                if (Projectile.velocity.Length() < 16)
                {
                    if (!SoundFlag)
                    {
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ManaBurst") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.45f }, Projectile.Center);
                        SoundFlag = true;
                    }
                    Projectile.velocity *= 1.2f;
                }
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarBurst2") with { MaxInstances = 0, PitchVariance = 0.3f }, Projectile.Center);
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.TintableDustLighted, Projectile.velocity.X * 0.7f, Projectile.velocity.Y * 0.7f, 0, ColorLib.TenebrisGradient, 1);
        }

    }
}