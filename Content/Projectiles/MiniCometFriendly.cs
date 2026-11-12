using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class MiniCometFriendly : ModProjectile, IHomingProjectile
    {

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        private Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture);

        public bool TracksNPCs => true;

        public bool TracksPlayers => false;

        public float HomingTurnSpeed => 10;

        public bool UsesHomingAcceleration => false;

        public float HomingAccelAmount => 1f;

        public float HomingMaxAccel => 1f;

        public float DetectRadius => 1200;

        public bool CanHome => DelayTimer > 20;

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.StellarFireGradientLooping();
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, i / (float)Projectile.oldPos.Length);
                Color color = OpusColorUtils.MultiLerp(i / (float)Projectile.oldPos.Length, ColorLib.StellarFireColormap);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (color with { A = 0 } * 0.3f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
            }


            Opus.DrawTextureOnProj(DTAssetLib.SparkSmoothThin, Projectile, Color.White with { A = 0 }, false, Projectile.rotation + MathHelper.PiOver2, 0.2f, 0.1f);

            return false;
        }


        public override void AI()
        {

            DelayTimer++;


            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, ColorLib.StellarFire1.ToVector3() * 0.2f);

        }
     
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 300);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        }

    }
}