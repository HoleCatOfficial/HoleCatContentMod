using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class VeiledSpearProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 108;
            MinExtension = 0.6f;
            MaxExtension = 70f;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            JabSound = SoundID.Item1;

            Glowmask = ModContent.Request<Texture2D>("DestroyerTest/Content/Projectiles/Weapon/Melee/VeiledSpearProjectile_Glow");
        }

        public override void ExtraEffects()
        {
            MaxExtension = 70f * Projectile.scale;
            if (progress > 0.2f && progress < 0.8f)
            {
                if (Projectile.ai[0] % 3 == 0)
                {
                    SoundEngine.PlaySound(DTAssetLib.ChargeBreak with { PitchVariance = 0.3f }, Projectile.Center);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 12, ModContent.ProjectileType<RiftSpark>(), Projectile.damage / 5, 5, Owner.whoAmI);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Zap with { PitchVariance = 0.5f }, target.Center);
            target.AddBuff(ModContent.BuffType<DaylightOverload>(), 600);
        }

        public override void AtFullExtension()
        {
            StarParticle Star = new();
            Star.Initialize(Tip, Main.rand.NextVector2Circular(0.01f, 0.01f), Color.White, Main.rand.NextFloat(1f, 2f));
            ParticleEngine.Particles.Add(Star);

            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Tip);
            SoundEngine.PlaySound(SoundID.Item60, Tip);
        }
    }
}