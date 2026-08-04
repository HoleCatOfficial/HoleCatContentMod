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
    public class HematoidBlob : ModProjectile, IHomingProjectile
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
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Ranged;
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
            Texture2D Back = ModContent.Request<Texture2D>($"{Texture}_Back").Value;
            Texture2D Front = ModContent.Request<Texture2D>($"{Texture}").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Scale = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(Back, Projectile.OldCenter()[i] - Main.screenPosition, null, Color.White, 0f, Back.Size() / 2, Scale, SpriteEffects.None);
            }

            for (int j = 0; j < Projectile.oldPos.Length; j++)
            {
                float Scale = MathHelper.Lerp(1f, 0f, (float)j / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(Front, Projectile.OldCenter()[j] - Main.screenPosition, null, Color.White with { A = 0 }, 0f, Front.Size() / 2, Scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }

    

      
        public override void AI()
        {
            DelayTimer++;

            Projectile.rotation = 0f;
            
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
            List<Vector2> Star1 = Polar.GenerateCurvedStar(8, 3, 10, Projectile.Center, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            foreach (Vector2 p1 in Star1)
            {
                Vector2 Vel = p1 - Projectile.Center;
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Vel, 0, Color.Red, 1f);
            }
            
           
        }
    }
}