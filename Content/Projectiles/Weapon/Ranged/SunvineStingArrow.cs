using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.RangedItems;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class SunvineStingArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public SoundStyle kill = SoundID.Item127;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(25))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(1, 1), ModContent.ProjectileType<SolarTrail>(), Projectile.damage / 2, 1f, Projectile.owner);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, (Projectile.Center + new Vector2(-10, 0).RotatedBy(Projectile.rotation)) - Main.screenPosition, null, ColorLib.Rift, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, Projectile.scale * 0.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        Player Owner => Main.player[Projectile.owner];
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HeldItem.ModItem is SunvineSting Sting)
            {
                SoundEngine.PlaySound(SoundID.Item112 with { Pitch = MathHelper.Lerp(-0.7f, 0f, (float)Sting.HitCount / 20f)}, Projectile.Center);
                Sting.HitCount++;
                Sting.ResetTime = 120;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(kill, Projectile.Center);

            Vector2[] Ds = Opus.RadialVectorOutward(5, Projectile.Center, 0.2f, 0f);

            for (int i = 0; i < Ds.Length; i++)
            {
                Spark S = new();
                S.PrepareSpark(Projectile.Center, Ds[i], Ds[i].ToRotation(), ColorLib.LightRift2, 0.2f, false, 20, SparkDrawMode.Additive, 5f);
                ParticleEngine.Particles.Add(S);
            }
        }
    }
}