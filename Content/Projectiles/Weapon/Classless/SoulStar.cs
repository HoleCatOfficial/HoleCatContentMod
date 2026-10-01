using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Classless
{
    public class SoulStar  : ModProjectile, IHomingProjectile
    {

        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 15;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 2800;

        bool IHomingProjectile.CanHome => DelayTimer >= 10;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;


            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, ColorLib.Soul, true, 0f, 1f, 1f);

            spriteBatch.UseBlendState(BlendState.AlphaBlend);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White, true, 0f, 0.6f, 0.6f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            DelayTimer++;

            Projectile.rotation += Projectile.direction * 0.07f;

            Lighting.AddLight(Projectile.Center, ColorLib.Soul.ToVector3() * 0.2f);

            if (DelayTimer < 20 || DelayTimer > 180)
            {
                return;
            }

            float maxDetectRadius = 2800f;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 300);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                PointGlowPreMultiplied glow = new();
                glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(8f, 8f), ColorLib.Soul, 2f);
                ParticleEngine.Particles.Add(glow);
            }
        }

    }
}
