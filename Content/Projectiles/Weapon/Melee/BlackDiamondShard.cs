using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class BlackDiamondShard : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Weapon/Melee/BlackDiamondProjectile";
        int StickIndex => (int)Projectile.ai[0];

        NPC StuckNPC => Main.npc[StickIndex];

        float OffsetRotation => Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.timeLeft = 180;
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisBlue with { A = 0 }, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.03f, 3.5f), SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.02f, 3f), SpriteEffects.None, 0f);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = OffsetRotation;
        }

        public override void AI()
        {
            if (StuckNPC.active)
            {


                Vector2 D = (Projectile.rotation - MathHelper.PiOver2 + Main.rand.NextFloat(0.1f)).ToRotationVector2() * 6;
                Fire F = new Fire();
                F.PrepareFire(Projectile.Center, D, DTUtils.RandomDirection(2), 0.1f, ColorLib.TenebrisBlue, Main.rand.NextFloat(0.3f, 0.5f), 60, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.BehindProjectiles.Add(F);

                TenebrousCloudParticle FX = new();
                FX.Initialize(Projectile.Center, D, ColorLib.TenebrisBlue, 0.6f, Main.rand.NextFloat(0.1f, 0.2f));
                ParticleEngine.BehindProjectiles.Add(FX);

                Projectile.Center += StuckNPC.velocity;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { PitchVariance = 0.5f, Volume = 0.7f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlackDiamondShardExplosion>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
    }

    internal class BlackDiamondShardExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 5;
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                Fire F = new Fire();
                F.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(3, 3), DTUtils.RandomDirection(2), 0.1f, ColorLib.TenebrisBlue, Main.rand.NextFloat(0.3f, 0.5f), 60, FireDrawMode.Additive, PixelLayer.AboveTiles);
                ParticleEngine.BehindProjectiles.Add(F);

                TenebrousCloudParticle FX = new();
                FX.Initialize(Projectile.Center, Main.rand.NextVector2Circular(3, 3), ColorLib.TenebrisBlue, 0.6f, Main.rand.NextFloat(0.1f, 0.2f));
                ParticleEngine.BehindProjectiles.Add(FX);
            }
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 10, Projectile.Center, 0, ColorLib.TenebrisBlue, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 10, Projectile.Center, 0, OpusColorUtils.Pastel(ColorLib.TenebrisBlue, 0.4f), 1f, 8);
            //Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 6, Projectile.Center, 0, Color.White, 0.75f, 6);
        }
    }
}