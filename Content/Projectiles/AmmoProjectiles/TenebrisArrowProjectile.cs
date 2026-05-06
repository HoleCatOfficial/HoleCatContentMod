using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class TenebrisArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.WindPhysicsImmunity[Type] = true;
            Main.projFrames[Type] = 3;
        }

        float Mode
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.frame = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(1, 4);
        }
        
        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        public Color colorofLight = Color.White;
        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > 30)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > 30)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            if (Mode > 2)
            {
                Mode = 2;
            }
            if (Mode < 1)
            {
                Mode = 1;
            }

            if (Mode == 1)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }
            if (Mode == 2)
            {
                Projectile.friendly = false;
                Projectile.hostile = true;
            }

            if (Projectile.frame == 1)
            {
                colorofLight = ColorLib.TenebrisMagenta;
            }
            if (Projectile.frame == 2)
            {
                colorofLight = ColorLib.TenebrisBlue;
            }
            if (Projectile.frame == 3)
            {
                colorofLight = ColorLib.TenebrisBeige;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, colorofLight.ToVector3());
            if (Projectile.timeLeft <= 120)
            {
                Projectile.velocity.Y += 0.015f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue; // skip tiny wiggle segments

                float rotation = diff.ToRotation();

                float width = MathHelper.Lerp(0.01f, 0.000007f, i / 30);
                float alpha = MathHelper.Lerp(1f, 0f, i / 30);
                Color color = colorofLight * alpha;

                Main.spriteBatch.Draw(
                    DTAssetLib.Square.Value,
                    start,
                    null,
                    color,
                    rotation,
                    new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisSlinger/TenebrisSlingerArrowImpact", 4) with { PitchVariance = 0.4f });
          
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisSlinger/TenebrisSlingerArrowImpact", 4) with { PitchVariance = 0.4f });
          
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<TenebrisDarkmatterDust>());
                dust.noGravity = true;
                dust.velocity *= 1.5f;
                dust.scale *= 0.9f;
            }
            return true;
        }
    }
}