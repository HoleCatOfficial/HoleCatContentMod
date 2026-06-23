using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class CrystalBladeSparkPink : ModProjectile, IHomingProjectile
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

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;

            Dust D = Dust.NewDustPerfect(Projectile.Center, DustID.UndergroundHallowedEnemies, Vector2.Zero);
            D.noGravity = true;

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
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

    public class CrystalBladeSparkPurple: ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 9f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 18f;

        float IHomingProjectile.DetectRadius => 900f;

        bool IHomingProjectile.CanHome => Timer >= 30;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;

            Dust D = Dust.NewDustPerfect(Projectile.Center, DustID.CorruptSpray, Vector2.Zero);
            D.noGravity = true;

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
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

    public class CrystalBladeSparkBlue : ModProjectile, IHomingProjectile
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

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;

            Dust D = Dust.NewDustPerfect(Projectile.Center, DustID.HallowSpray, Vector2.Zero);
            D.noGravity = true;

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
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
