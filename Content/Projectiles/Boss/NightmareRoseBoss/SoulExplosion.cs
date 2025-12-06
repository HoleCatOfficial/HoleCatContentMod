using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Common;
using Microsoft.Build.Execution;
using DestroyerTest.Content.Buffs;
using Terraria.ID;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class SoulExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 2000;
            Projectile.height = 2000;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int c = 0; c < 4; c++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, ColorLib.Soul, 16f);
            }
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, ModContent.DustType<SoulDust>(), 0, 0, 0, ColorLib.Soul, 5f);
            
            
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //Vector2 mag = Projectile.Center + new Vector2(35, 0);
            //Vector2 KickVel = mag - Projectile.Center;
            //target.velocity += KickVel;
            //target.AddBuff(ModContent.BuffType<SoulInferno>(), 600);
        }
    }
}
