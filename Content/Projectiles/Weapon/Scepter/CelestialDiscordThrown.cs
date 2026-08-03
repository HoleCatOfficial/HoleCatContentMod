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
using System.Collections.Generic;
using System.IO;
 
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class CelestialDiscordThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.CelestialGradient;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.LunarOre;
            base.SetDefaults();
        }




        public int AreaTimer = 600;
        public bool TriggeredArea = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            target.AddBuff(BuffID.Confused, 120);
            //Projectile.NewProjectile(Projectile.GetSource_OnHit(null), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CDAreaParticle>(), 0, 0, Projectile.owner);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            
            //Projectile.NewProjectile(Projectile.GetSource_OnHit(null), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CDAreaParticle>(), 0, 0, Projectile.owner);

            base.OnTileCollide(oldVelocity);
            return false; // Prevents the projectile from being destroyed on collision
        }

    }
    public class CDAreaParticle : ModProjectile
        {
            public override string Texture => DTUtils.NoTexture;

            private int auraTimer = 600;
            private float radius = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
            Projectile.hide = true; // Optional: hide if just visual
            }

            public override void AI()
            {
                auraTimer--;

                // Interpolate radius from 0 to 300 over the first 30 ticks (auraTimer 600 -> 570)
                if (auraTimer > 570)
                {
                    float t = 1f - (auraTimer - 570) / 30f; // t goes from 0 to 1 as auraTimer goes from 600 to 570
                    radius = MathHelper.SmoothStep(0f, 300f, MathHelper.Clamp(t, 0f, 1f));
                }
                else
                {
                    radius = 300f;
                }

                // Create the dust ring
                int dustAmount = 8; // Number of dust particles in the ring
            for (int i = 0; i < dustAmount; i++)
            {
                float angle = MathHelper.TwoPi * i / dustAmount;
                // Offset the angle each tick so the dust rotates around the circle over time
                float timeOffset = Main.GameUpdateCount * 0.03f; // Adjust speed as needed
                float dynamicAngle = angle + timeOffset;
                Vector2 dustPos = Projectile.Center + radius * new Vector2((float)Math.Cos(dynamicAngle), (float)Math.Sin(dynamicAngle));
           
            }
                    if (auraTimer <= 30)
                {
                    float t = (float)auraTimer / 30f; // t goes from 1 to 0 as auraTimer goes from 30 to 0
                    radius = MathHelper.SmoothStep(0f, 300f, t); // Smoothly shrink from 300 to 0
                }
                
               

                Vector2 damagebonusspawnpoint = Main.rand.NextVector2Circular(radius, radius) + Projectile.Center;
          

                // Apply buffs to players inside the radius
                foreach (Player player in Main.player)
                {
                    if (player.active && !player.dead && Vector2.Distance(player.Center, Projectile.Center) <= radius)
                    {
                        player.GetDamage(DamageClass.Generic) += 0.25f;
                    }
                }

                if (auraTimer <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

}

