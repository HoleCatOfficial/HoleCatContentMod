using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class MoongeistBolt : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.07f;

        float IHomingProjectile.HomingMaxAccel => 40f;

        float IHomingProjectile.DetectRadius => 4800;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public float DelayTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Mult = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.MediumAquamarine with { A = 0 } * 0.2f * Mult) * Projectile.Opacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 1f, SpriteEffects.None);

                Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.OldCenter()[i] - Main.screenPosition, null, (Color.Aquamarine with { A = 0 } * 0.3f * Mult) * Projectile.Opacity, i == 0 ? Projectile.velocity.ToRotation() : Projectile.oldRot[i], DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.1f, SpriteEffects.None);
            }


            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, Projectile.rotation, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, new Vector2(XScale, 1f), SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, Color.Aquamarine with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(XScale, 0.25f), SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(1).Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, DTAssetLib.Star(1).Value.Size() / 2, new Vector2(XScale * 0.5f, 0.125f), SpriteEffects.None);
            return false;
        }

        float XScale = 0f;

        public override void AI()
        {

            DelayTimer++;

            XScale = 0.01f * Projectile.velocity.Length();

            XScale = Utils.Clamp(XScale, 0.5f, 8f);
            Projectile.rotation = Projectile.velocity.ToRotation();



            Lighting.AddLight(Projectile.Center, Color.Aquamarine.ToVector3());


            StarParticle star = new();
            star.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.05f, Color.Aquamarine, 0.4f, Main.rand.NextFloat(-0.1f, 0.1f), 90);
            ParticleEngine.Particles.Add(star);



            if (DelayTimer < 30)
            {
                Projectile.velocity *= 0.94f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item109 with { Pitch = 0.5f }, target.Center);
           

            //target.AddBuff(BuffID.Electrified, 300);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                StarParticle star = new();
                star.Initialize(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), Color.Aquamarine, 1f, Main.rand.NextFloat(-0.1f, 0.1f), 90);
                ParticleEngine.Particles.Add(star);
            }

            for (int i = 0; i < 10; i++)
            {
                float spd = 0.3f * i;
                Vector2 Vel = new Vector2(spd, 0).RotatedBy(Projectile.rotation);
                PixelParticle pixel1 = new();
                pixel1.Initialize(Projectile.Center, Vel, Color.Aquamarine, 2f, 20);
                ParticleEngine.Particles.Add(pixel1);

                PixelParticle pixel2 = new();
                pixel2.Initialize(Projectile.Center, -Vel, Color.Aquamarine, 2f, 20);
                ParticleEngine.Particles.Add(pixel2);
            }

            for (int i = 0; i < 6; i++)
            {
                float spd = 0.2f * i;
                Vector2 Vel = new Vector2(spd, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
                PixelParticle pixel1 = new();
                pixel1.Initialize(Projectile.Center, Vel, Color.Aquamarine, 2f, 20);
                ParticleEngine.Particles.Add(pixel1);

                PixelParticle pixel2 = new();
                pixel2.Initialize(Projectile.Center, -Vel, Color.Aquamarine, 2f, 20);
                ParticleEngine.Particles.Add(pixel2);
            }
        }
    }
}
