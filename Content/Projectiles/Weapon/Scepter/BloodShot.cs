using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class BloodShot : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

      

        public override void SetStaticDefaults()
        {
           
        }

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>(); // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Color.Red, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            return false;
        }


        int t = 0;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.6f);
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Projectile.velocity * 0.4f, 0, Color.Red, 1f);
            d.noGravity = true;

            t++;

            if (t < 120)
            {

            }
            else
            {
                if (Projectile.velocity.Length() < 30)
                {
                    Projectile.velocity *= 1.05f;
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            

        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 }, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.05f, 1.7f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);
            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Red * 0.8f, 0.1f, 0.05f, 1f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion2);

            foreach (Dust dust in Opus.RadialSpreadDust(DustID.FireworksRGB, 24, Projectile.Center, 0, Color.Red, 2f, 5f, Main.rand.NextFloat(MathHelper.TwoPi)))
            {
                dust.noGravity = true;
            }
        }
    }
}
