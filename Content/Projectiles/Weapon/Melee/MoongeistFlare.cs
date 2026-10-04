using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class MoongeistFlare : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = 1;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.MediumAquamarine with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 0.7f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.Aquamarine with { A = 0 } * 0.3f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.06f, SpriteEffects.None);
            }


            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.3f, 0.15f), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(0.15f, 0.05f), SpriteEffects.None);

            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoongeistFlareExplosion>(), Projectile.damage, Projectile.knockBack);
        }
    }

    public class MoongeistFlareExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.timeLeft = 15;
        }
        public override bool PreDraw(ref Color lightColor)
        {
         
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Aquamarine, 0.3f, 0.01f, 2f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);

            BloomRingSharp Ring1 = new();
            Ring1.Prepare(Projectile.Center, Vector2.Zero, Color.Aquamarine, 0.1f, 0.01f, 0.3f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring1);
        }
        public override void AI()
        {

        }
    }
}
