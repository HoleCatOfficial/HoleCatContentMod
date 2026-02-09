using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles.Stellar;
using InnoVault.PRT;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    [AutoloadGlowmask]
    public class GalantineKnifeThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];
        private NPC StickT;
        private Vector2 stuckOffset;
        private bool Flag1;
        private bool Stick;

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Stick)
            {
                // Run once when the projectile first sticks
                if (!Flag1)
                {
                    Projectile.timeLeft = 240; // Stays stuck for ~2 seconds
                    Projectile.velocity = Vector2.Zero; // Stop movement
                    Projectile.tileCollide = false; // Don’t unstick from walls
                    Flag1 = true;
                    Projectile.netUpdate = true;
                }

                if (StickT != null && StickT.active)
                {
                    StickT.AddBuff(ModContent.BuffType<GalantineBurn>(), 120);

                    // Keep the projectile glued to the target
                    Projectile.Center = StickT.Center + stuckOffset;

                    // Optional: match target rotation if you want it to “move” with the enemy animation
                    // Projectile.rotation = StickT.rotation;

                    if (StickT.life <= 0)
                        Projectile.Kill();

                    return;
                }
                else
                {
                    // Target despawned or died
                    Stick = false;
                    Projectile.tileCollide = true;
                }
            }
            else
            {
                // Regular flying / gravity behavior
                if (LifeTime < 60)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
                else
                {
                    if (Main.GameUpdateCount % 6 == 0)
                        SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.75f, MaxInstances = 0 }, Projectile.Center);

                    Projectile.velocity.Y += 0.2f;
                    Projectile.rotation += 0.5f * Projectile.direction;
                }
            }

            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(StellarParticleIndex.ConstitutionParticle, Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.15f, default, 0.5f);
            }
        }

        public void Sticking(NPC target)
        {
            if (!Stick)
            {
                StickT = target;
                stuckOffset = Projectile.Center - target.Center;
                Stick = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Only stick if it’s early in its flight (so it doesn't embed after bouncing)
            if (LifeTime < 60)
            {
                Sticking(target);
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StellarBow/StellarBowEmpoweredShoot", 3)
            {
                PitchVariance = 0.4f,
                MaxInstances = 0
            }, Projectile.Center);

            Opus.RadialSpreadParticle(StellarParticleIndex.ConstitutionParticle, 12, Projectile.Center, 1f, default, 1f, 2f, RandomOffset: true);

            DTUtils.ConstitutionStarExplosionEffects(Projectile);

            if (!Stick)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<ConstitutionStarFriendly>(), 3, Projectile.Center, 8, 4, 8, AI2: 1, RandomOffset: true);
            }
        }
    }
}
