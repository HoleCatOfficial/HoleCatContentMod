using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class SunspotSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 9f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 18f;

        float IHomingProjectile.DetectRadius => 900f;

        bool IHomingProjectile.CanHome => Timer >= 90;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int TurnDir = 0;
        public override void OnSpawn(IEntitySource source)
        {
            TurnDir = Main.rand.Next(-1, 2);
        }


        float Scl = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10f, color, 0f);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl * 1.4f, 0.5f) * 0.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl * 1.4f, 0.5f) * 0.07f, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.07f, SpriteEffects.None);


            return false;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;

            Projectile.ResetExcessTrailPoints();

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            Scl = Opus.Sine(0.8f, 0.4f, 0.6f);

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;

                if (TurnDir == -1)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-0.05f);
                }
                if (TurnDir == 1)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
                }

            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);
        }
    }

    public class HeliosSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 9f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.02f;

        float IHomingProjectile.HomingMaxAccel => 9f;

        float IHomingProjectile.DetectRadius => 900f;

        bool IHomingProjectile.CanHome => Timer >= 90;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int TurnDir = 0;
        public override void OnSpawn(IEntitySource source)
        {
            TurnDir = Main.rand.Next(-1, 2);
        }

        float Scl = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10f, color, 0f);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.07f, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, color with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(Scl, 0.5f) * 0.07f, SpriteEffects.None);

            return false;
        }

        int Timer = 0;

        
        public override void AI()
        {
            Timer++;

            Projectile.ResetExcessTrailPoints();

            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);

            Scl = Opus.Sine(0.8f, 0.4f, 0.6f);

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
                if(TurnDir == -1)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-0.05f);
                }
                if (TurnDir == 1)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);
            
        }
    }
}
