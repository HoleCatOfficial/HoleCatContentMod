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
    public class RibChainsawHoldout : ModProjectile
    {
        SoundStyle EnemySlice = new SoundStyle($"DestroyerTest/Assets/Audio/TenebrousKatana/GoreSlice", 2) with {
					Volume = 1.0f, 
					Pitch = 0.0f, 
					PitchVariance = 0.5f, 
				}; 
        
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;

            DrawOffsetX = 25;
            DrawOriginOffsetY = -2;
        }

        private void AnimateProjectile() {
            if (++Projectile.frameCounter >= 4) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }
        
        public int SoundInterval = 20;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<RibChainsaw>() && player.controlUseItem)
            {

                SoundInterval--;
                if (SoundInterval <= 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Chainsaw"), Projectile.Center);
                    SoundInterval = 20;
                }

                AnimateProjectile();

                Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                Projectile.spriteDirection = Projectile.direction;
                player.ChangeDir(Projectile.direction);
                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
                Projectile.Center = playerCenter;
                float rotationOffset = Projectile.spriteDirection == -1 ? MathHelper.Pi : 0;
                Projectile.rotation = Projectile.velocity.ToRotation() + rotationOffset;
                player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
                Projectile.timeLeft = 2;
            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            target.AddBuff(BuffID.Bleeding, 120);
		}

    }
}