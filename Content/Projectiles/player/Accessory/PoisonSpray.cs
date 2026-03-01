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
using Microsoft.Build.Evaluation;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class PoisonSpray : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 6;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust trail = Dust.NewDustPerfect(Projectile.position, DustID.Poisoned, null, 0, Color.White, 1f);
            trail.noGravity = true;
            
            if (LifeTime < 15)
            {
                
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
            }
        }
    }
}
