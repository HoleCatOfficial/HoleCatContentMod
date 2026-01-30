using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Input;
using Terraria.DataStructures;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using ReLogic.Reflection;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class DreampoonHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60; // persistent
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public Vector2 ShotPos;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<Dreampoon>() && player.channel)
            {
                //player.heldProj = Projectile.type;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                ShotPos = mountedCenter + toCursor * 40;
                bool canshoot = player.ownedProjectileCounts[ModContent.ProjectileType<DreampoonProjectile>()] < 1;
                
                if(canshoot)
                {
                    Projectile.ai[0]++;
                }
                Projectile.timeLeft = 60;
                HoldAnim(player);

                

                if (Projectile.ai[0] == 30)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ClickMetal"), player.Center);
                }

                if (Projectile.ai[0] > 60 && canshoot)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/VesperHarpoonShoot") { MaxInstances = 0, PitchVariance = 0.2f }, Projectile.Center);
                    Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), ShotPos, toCursor * 15, ModContent.ProjectileType<DreampoonProjectile>(), Projectile.damage, 10, Projectile.owner);
                    ShotFX(ShotPos);
                    Projectile.ai[0] = 0;
                }

                Reload();
            }
            else
            {
                Projectile.Kill();
            }
        }

        public void ShotFX(Vector2 CTR)
        {
            for (int u = 0; u < 6; u++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), CTR, new Vector2(16, 0).RotatedByRandom(MathHelper.Pi), Color.White, 1f, 2);
            }
            for (int k = 0; k < 2; k++) {
				float speedMulti = 0.4f;
				if (k == 1) {
					speedMulti = 0.8f;
				}

				Gore smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), CTR, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity += Vector2.One;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), CTR, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity.X -= 1f;
				smokeGore.velocity.Y += 1f;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), CTR, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity.X += 1f;
				smokeGore.velocity.Y -= 1f;
				smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), CTR, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
				smokeGore.velocity *= speedMulti;
				smokeGore.velocity -= Vector2.One;
			}
        }
        public void Reload()
        {
        }

        public void HoldAnim(Player player)
        {
            // Lock the projectile's position relative to the player
            float holdDistance = 10f;
            Vector2 mountedCenter = player.MountedCenter;
            Vector2 toCursor = Main.MouseWorld - mountedCenter;
            toCursor.Normalize();
            Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

            Projectile.Center = desiredPos;

            // Rotate to face the cursor
            Projectile.rotation = toCursor.ToRotation();

            // Constantly face the direction it's pointing
            Projectile.spriteDirection = toCursor.X > 0 ? 1 : -1;
            Projectile.direction = toCursor.X > 0 ? 1 : -1;

            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

    }
}