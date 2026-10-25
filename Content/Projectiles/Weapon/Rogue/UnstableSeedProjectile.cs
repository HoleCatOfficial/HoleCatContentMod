using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using GlowmaskHelper.Content;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class UnstableSeedProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
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
                if (!Flag1)
                {
                    Projectile.timeLeft = 120;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;
                    SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/SeedStick") { PitchVariance = 0.6f, MaxInstances = 0}, Projectile.Center);
                    Flag1 = true;
                    Projectile.netUpdate = true;
                }

                if (StickT != null && StickT.active)
                {
                    StickT.AddBuff(ModContent.BuffType<SoulErosion>(), 120);

                    Projectile.Center = StickT.Center + stuckOffset;

                    if (StickT.life <= 0)
                    {
                        Projectile.Kill();
                    }

                    return;
                }
                else
                {
                    Stick = false;
                    SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { Pitch = 1.2f, PitchVariance = 0.3f, MaxInstances = 0 }, Projectile.Center);
                    SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { Pitch = 2f, PitchVariance = 0.3f, MaxInstances = 0 }, Projectile.Center);
                    SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { Pitch = -1.9f, PitchVariance = 0.3f, MaxInstances = 0, Volume = 0.4f }, Projectile.Center);

                    SimpleExplosionParticle Explosion = new();
                    Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Yellow, 0.2f, 0.02f, 1.7f, Utilities.SubtractiveBlending);
                    ParticleEngine.Particles.Add(Explosion);

                    Main.player[Projectile.owner].Heal(3);
                    SimpleExplosionParticle Explosion2 = new();
                    Explosion2.Prepare(Main.player[Projectile.owner].MountedCenter, Vector2.Zero, Color.Yellow, 0.2f, 0.02f, 2f, Utilities.SubtractiveBlending);
                    ParticleEngine.Particles.Add(Explosion2);
                    Projectile.Kill();
                }
            }
            else
            {
                // Regular flying / gravity behavior
                if (LifeTime < 30)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
                else
                {
                    Projectile.velocity.Y += 1f;
                    Projectile.rotation += 0.5f * Projectile.direction;
                }
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
            if (!target.friendly && !NPCID.Sets.ProjectileNPC[target.type])
            {
                Sticking(target);
                Projectile.netUpdate = true;
            }
            
        }

        public override void OnKill(int timeLeft)
        {
            if (!Stick)
            {
                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { Pitch = 1.2f, PitchVariance = 0.3f, MaxInstances = 0 }, Projectile.Center);

                SimpleExplosionParticle Explosion2 = new();
                Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Yellow, 0.2f, 0.02f, 1f, Utilities.SubtractiveBlending);
                ParticleEngine.Particles.Add(Explosion2);
            }
            else
            {
                if (StickT != null && StickT.active)
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { Pitch = 1.2f, PitchVariance = 0.3f, MaxInstances = 0 }, Projectile.Center);

                    SimpleExplosionParticle Explosion2 = new();
                    Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Yellow, 0.2f, 0.02f, 1f, Utilities.SubtractiveBlending);
                    ParticleEngine.Particles.Add(Explosion2);
                }
            }
        }
    }
}
