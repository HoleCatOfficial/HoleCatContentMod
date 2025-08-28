using System.Linq;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{

    public class TreasonScrollBomb : ModProjectile
    {


        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 22; // The width of projectile hitbox
            Projectile.height = 22; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        int BlastRadius = 270;

        private bool alreadyExploded = false;

        public override void OnKill(int timeLeft)
        {
            if (alreadyExploded)
                return;
            alreadyExploded = true;

            Player player = Main.player[Projectile.owner];
            int damage = player.GetWeaponDamage(player.HeldItem);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom1>(), Projectile.Center, Vector2.Zero, Color.LavenderBlush, 1f);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Distance(Projectile.Center) < BlastRadius)
                {
                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                    {
                        Damage = Projectile.damage / 12,
                        Knockback = 5f,
                        HitDirection = npc.Center.X < Projectile.Center.X ? -1 : 1
                    };
                    npc.StrikeNPC(hitInfo);
                }
            }

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active &&
                    proj.type == ModContent.ProjectileType<TreasonScrollBomb>() &&
                    proj.whoAmI != Projectile.whoAmI &&
                    proj.Center.Distance(Projectile.Center) <= BlastRadius)
                {
                    proj.Kill();
                }
            }
        }

        int Radius = 180;
        int DustAmount = 35;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
            for (int i = 0; i < DustAmount; i++)
            {
                Vector2 dustPos = Projectile.Center + Main.rand.NextVector2CircularEdge(Radius, Radius);
                Dust.NewDustPerfect(dustPos, DustID.TintableDustLighted, Projectile.velocity, 100, Color.LavenderBlush, 1.2f);
            }
            Projectile.velocity *= 0.995f;
        }   
    }
}