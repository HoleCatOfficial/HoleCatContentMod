using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class LavaScrollLava : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 19f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.08f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Projectile.ai[0] >= 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

      
        public override bool PreDraw(ref Color lightColor)
        {
          

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, new Vector2(1 + (0.2f * Projectile.velocity.Length()), 1f), SpriteEffects.None);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Lava, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 0, default, 1f).noGravity = true;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 30)
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] >= 30 ? null : false;
        }


    }
}
