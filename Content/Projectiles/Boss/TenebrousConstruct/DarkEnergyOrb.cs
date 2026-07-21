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
using System.Linq;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class DarkEnergyOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;

            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
         }

        float ro = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            ro += 0.08f;
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, ColorLib.TenebrisGradient * Projectile.Opacity, ro);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, ColorLib.TenebrisGradient));

            return false;
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return Projectile.ai[0] >= 180;
        }
        public override void AI()
        {
            AnimateProjectile();

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 180)
            {

            }
            else
            {
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, Projectile.ai[0] / 180f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Main.rand.NextVector2Circular(3, 3), 0, ColorLib.TenebrisGradient);
            }
        }
    }
}