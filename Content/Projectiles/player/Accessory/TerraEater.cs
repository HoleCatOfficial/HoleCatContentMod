using System.Collections.Generic;
using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class TerraEater : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 12f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 10f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => Projectile.timeLeft > 30 && Projectile.timeLeft < 210;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Color col = Color.Lerp(Color.DarkCyan, Color.DarkSeaGreen, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (col with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 15)
            {
                Projectile.frame++;

                if (Projectile.frame > Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
          
        }
    }
}