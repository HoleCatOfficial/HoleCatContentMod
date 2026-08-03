using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class SpectralStyngerBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero, 0, default, 1f);
            d.noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14);
            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<SpectralStyngerShard>(), Main.rand.Next(2, 6), Projectile.Center, Projectile.damage, Projectile.knockBack, 17f);
        }
    }

    public class SpectralStyngerShard : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 1600;

        bool IHomingProjectile.CanHome => Timer > 90;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero);
            d.noGravity = true;
            d.scale = 1.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff);
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
        }
    }
}
