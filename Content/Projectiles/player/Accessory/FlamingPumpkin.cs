using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class FlamingPumpkin : ModProjectile, IHomingProjectile
    {

        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.02f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 1500;

        bool IHomingProjectile.CanHome => DelayTimer > 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }



            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Scale = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, Color.Orange with { A = 0} * 0.3f, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, Scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White) with { effect = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically});
            return false;
        }




        public override void AI()
        {
            DelayTimer++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 40);
            d.noGravity = true;

            if (DelayTimer < 30)
            {
                Projectile.velocity *= 0.97f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer > 30;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            


        }
    }
}