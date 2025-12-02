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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class GreatFlayerDash : ModProjectile
    {
        SoundStyle EnemySlice = new SoundStyle($"DestroyerTest/Assets/Audio/TenebrousKatana/GoreSlice", 2) with {
            Volume = 1.0f, 
            Pitch = 0.0f, 
            PitchVariance = 0.5f, 
        }; 
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40; // persistent
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
        }
        public override string Texture => "DestroyerTest/Content/MeleeWeapons/GreatFlayer";
       
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Check if the player is holding the item and channeled
            if (player.HeldItem.type == ModContent.ItemType<GreatFlayer>())
            {

                // Lock the projectile's position relative to the player
                float holdDistance = 100f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;

                
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver4;

                // Shoot dust particles in a line from the tip
                Vector2 dustDirection = toCursor;
                Vector2 dustSpawn = Projectile.Center + dustDirection * Projectile.width * 0.5f;

                Vector2 randomSpawn = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
                int dustIndex = Dust.NewDust(randomSpawn, 0, 0, DustID.TintableDustLighted, dustDirection.X * 4f, dustDirection.Y * 4f, 100, ColorLib.Ichor, 1.2f);
                Main.dust[dustIndex].noGravity = true;

            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }
		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.LocalPlayer;
            target.AddBuff(BuffID.Bleeding, 300);
            target.AddBuff(BuffID.Ichor, 300);
			SoundEngine.PlaySound(EnemySlice, Projectile.position);
		}

    }
}