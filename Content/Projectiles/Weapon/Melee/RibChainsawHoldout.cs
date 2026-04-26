using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            /*
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            */
            Projectile.netImportant = true;
            Projectile.hide = true;
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


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;
            int frameHeight = T.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                T.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(T.Width / 2f, frameHeight / 2f);
            
            Main.EntitySpriteDraw(T, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            SoundInterval--;
            if (SoundInterval <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Chainsaw"), Projectile.Center);
                SoundInterval = 20;
            }

            AnimateProjectile();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);


            if (player.channel && !player.noItems && !player.CCed)
            {
                float holdoutDistance = RibChainsaw.HoldoutDistance * Projectile.scale;
                Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
                if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }

                // Set the projectile velocity, which is actually the holdout offset for held projectiles.
                Projectile.velocity = holdoutOffset;
            }
            else
            {
                Projectile.Kill();
            }


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

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();
            ScreenShake.screenshakeMagnitude = 4;
            ScreenShake.screenshakeTimer = 10;

            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { MaxInstances = 0 }, Projectile.position);
            int splatterdir = target.position.X > player.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, ColorLib.Ichor, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }
        }

    }
}