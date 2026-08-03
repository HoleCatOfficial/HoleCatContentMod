using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System;
using Terraria.DataStructures;
using System.IO;
 
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class TitanScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Glass;
            base.SetDefaults();
        }

        public int AreaTimer = 600;
        public bool TriggeredArea = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            

            Projectile.NewProjectile(Projectile.GetSource_OnHit(null), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AreaParticle>(), 0, 0, Projectile.owner);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
           

            Projectile.NewProjectile(Projectile.GetSource_OnHit(null), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AreaParticle>(), 0, 0, Projectile.owner);

            base.OnTileCollide(oldVelocity);
            return false; // Prevents the projectile from being destroyed on collision
        }

    }
    public class AreaParticle : ModProjectile
        {
            public override string Texture => DTUtils.NoTexture;

            private int auraTimer = 600;
          

            public override void SetDefaults()
            {
                Projectile.width = 2;
                Projectile.height = 2;
                Projectile.friendly = false;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 1200;
                Projectile.tileCollide = false;
                Projectile.scale = 0.1f;
                Projectile.hide = true; // Optional: hide if just visual
            }

            private float radius = 250f;
            public override void AI()
            {
                auraTimer--;

             

                // Apply buffs to players inside the radius
                foreach (Player player in Main.player)
                {
                    if (player.active && !player.dead && Vector2.Distance(player.Center, Projectile.Center) <= radius)
                    {
                        player.AddBuff(BuffID.TitaniumStorm, 300);
                    }
                }

                if (auraTimer <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

}

