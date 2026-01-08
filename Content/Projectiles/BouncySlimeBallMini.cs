using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using System.Collections.Generic;

namespace DestroyerTest.Content.Projectiles
{
    public class BouncySlimeBallMini : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (LifeTime < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
                Projectile.rotation += 0.5f * Projectile.direction;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item85, Projectile.Center);
            Opus.RadialDustRandomDir(DustID.PinkSlime, 14, Projectile.Center, 100, default, 4f, 2.5f);
        }
    }
}
