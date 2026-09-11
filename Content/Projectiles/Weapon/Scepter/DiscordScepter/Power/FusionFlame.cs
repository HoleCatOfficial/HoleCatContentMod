using System.IO;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter.Power
{
    public class FusionFlame : ModProjectile, IHomingProjectile
    {
        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        private void AnimateProjectile()
        {

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.DrawTextureOnProj(DTAssetLib.PointGlowPreMultiplied, Projectile, ColorLib.CelestialGradient with { A = 0 }, false, ScaleX: 2f, ScaleY: 2f);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, ColorLib.CelestialGradient with { A = 0 }) with { scale = new Vector2(1.2f, 1.2f) });

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 15f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 30f;

        float IHomingProjectile.DetectRadius => 400f;

        bool IHomingProjectile.CanHome => DelayTimer > 60;

        public override void AI()
        {
            DelayTimer++;

            if (Projectile.TryGetGlobalProjectile<HomingGlobal>(out var Homing) && Homing.TrackingNPC != null && Homing.TrackingNPC.whoAmI == -1)
            {
                Projectile.velocity *= 0.85f;
            }

            AnimateProjectile();
            Projectile.rotation = (Projectile.velocity.X) * 0.1f;


            if (Main.rand.NextBool(4))
            {
                PointGlowPreMultiplied glow = new();
                glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.5f, ColorLib.CelestialGradient, 1f);
                ParticleEngine.BehindProjectiles.Add(glow);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.Impacts.Malevolence with { Pitch = -0.5f}, Projectile.Center);

            BloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.CelestialGradient, 0.3f, 0.01f, 1f, BlendState.Additive);
            ParticleEngine.Particles.Add(Ring);

            for (int i = 0; i < 14; i++)
            {
                StarParticle star = new();
                star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), ColorLib.CelestialGradient, 1f);
                ParticleEngine.Particles.Add(star);
            }

            if (DTCrossMod.CalamityIsLoaded && DTCrossMod.CalamityMod != null)
            {
                if (DTCrossMod.CalamityMod.TryFind("ElementalMix", out ModBuff ElementalMix))
                {
                    target.AddBuff(ElementalMix.Type, 300);
                }
            }
        }
    }
}