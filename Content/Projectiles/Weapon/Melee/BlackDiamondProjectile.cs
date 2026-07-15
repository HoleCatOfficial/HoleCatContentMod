using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using Terraria.Utilities.Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using BreadLibrary.Core.Utilities;
using OpusLib.Content.Helpers;
using DestroyerTest.Content.Dusts;
using OpusLib.Content.Particles;
using System;
using BreadLibrary.Core.Graphics.Pixelation;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class BlackDiamondProjectile : BaseSpearProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 124;
            MinExtension = 0.6f;
            MaxExtension = 185f;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            ExtraLength = 110f;
            JabSound = DTAssetLib.SwordSounds.SpinWave with { PitchVariance = 0.6f };
        }

        public float ShineOpacity = 0f;
        Vector2 DPos;
        Color SC;
        public override void DrawUnder()
        {
            DPos = Projectile.Center + (new Vector2(110, -110) * Projectile.scale).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, DPos - Main.screenPosition, null, SC with { A = 0 } * ShineOpacity * 0.5f, Projectile.rotation + MathHelper.PiOver4, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.05f, 3.15f), SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, DPos - Main.screenPosition, null, SC with { A = 0 } * ShineOpacity, Projectile.rotation + MathHelper.PiOver4, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.03f, 3f), SpriteEffects.None, 0f);
        }

        public override void ExtraEffects()
        {
            MaxExtension = 185f * Projectile.scale;
            ShineOpacity = MathHelper.Lerp(0, 1, Utilities.Convert01To010(progress));
            SC = Color.Lerp(ColorLib.TenebrisBlue, OpusColorUtils.Pastel(ColorLib.TenebrisBlue, 0.3f), Utilities.Convert01To010(progress));

            if (!Main.dedServ)
            {

                Vector2 D = Projectile.rotation.ToRotationVector2() * 1f;
                Fire F = new Fire();
                F.PrepareFire(DPos, D, Math.Sign(D.X), 0.1f, SC, Main.rand.NextFloat(0.3f, 0.5f), 60, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.BehindProjectiles.Add(F);

                if (!DTOptimizationsConfig.instance.DisableExcessParticles)
                {
                    TenebrousCloudParticle FX = new();
                    FX.Initialize(DPos, D, SC, ShineOpacity, Main.rand.NextFloat(0.1f, 0.2f));
                    ParticleEngine.BehindProjectiles.Add(FX);
                }

            }

            if (LodgeCooldown > 0)
            {
                LodgeCooldown--;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            /*
            Rectangle ExtHitbox = Utils.CenteredRectangle(DPos, new Vector2(120, 120));
            if (ExtHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            */
            return base.Colliding(projHitbox, targetHitbox);
        }

        int LodgeCooldown = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { PitchVariance = 0.2f });
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine with { PitchVariance = 0.2f });

            BlackDiamondParticle FX = new();
            FX.Initiate(target.Center, Projectile.rotation);
            ParticleEngine.BehindProjectiles.Add(FX);

            if (LodgeCooldown <= 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Tip, Vector2.Zero, ModContent.ProjectileType<BlackDiamondShard>(), Projectile.damage, 0f, Owner.whoAmI, target.whoAmI, Projectile.rotation + MathHelper.PiOver4);
                LodgeCooldown = 5;
            }

        }
    }
}