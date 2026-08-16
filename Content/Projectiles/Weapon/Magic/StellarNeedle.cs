using System.Collections.Generic;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles.Stellar;
using BreadLibrary.Core.Graphics.Particles;
using System.Linq;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class StellarNeedle : ModProjectile, IStickyProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(13, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 10f, ColorLib.StellarFire3, trailOffset, 10);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public float GlowMult = 1f;

        bool IStickyProjectile.IsStickingToTarget { get; set; }

        bool IStickyProjectile.CanStickToTargets => true;

        bool IStickyProjectile.CanBeUnstuck => false;

        int IStickyProjectile.MaxStuckProjectiles => 1;

        bool IStickyProjectile.DealsDamageWhileStuck => true;

        NPC.HitInfo IStickyProjectile.StuckDamageInfo => new NPC.HitInfo() { Damage = Projectile.damage, HitDirection = Projectile.direction };

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            StellarPointGlow particle = new();
            particle.Prepare(Projectile.Center, Projectile.velocity * 0.1f);
            ParticleEngine.BehindProjectiles.Add(particle);

            Lighting.AddLight(Projectile.Center, ColorLib.StellarFire2.ToVector3());

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnKill(int timeLeft)
        {
           
        }

        void IStickyProjectile.OnStickToTarget(NPC target)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit, Projectile.Center);
            Projectile.timeLeft = 120;
        }

        void IStickyProjectile.DuringStick(NPC target)
        {

        }

        void IStickyProjectile.OnUnstick(NPC target, Projectile Replacing)
        {

        }
    }
}