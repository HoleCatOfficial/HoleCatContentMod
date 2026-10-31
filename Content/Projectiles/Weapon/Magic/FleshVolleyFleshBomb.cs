using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class FleshVolleyFleshBomb : ModProjectile, IHomingProjectile
    {
        public SoundStyle BombPlant = SoundID.Item50;
        public SoundStyle BombBlow = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 };

        public bool Homing = false;
        float TSpeed = 1f;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => TSpeed;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.03f;

        float IHomingProjectile.HomingMaxAccel => 30f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Homing && Projectile.ai[0] >= 30;

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
           
            Homing = Main.rand.NextBool();
            Projectile.tileCollide = !Homing;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        float GlowOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/FleshBombOutline").Value;
            if (Homing)
            {
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Red * GlowOpacity, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor));

            return false;
        }


        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30)
            {
                if (!Homing)
                {
                    Projectile.velocity.Y++;
                }
                else
                {
                    if (Projectile.ai[0] == 31)
                    {
                        SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item104, Projectile.Center);
                    }

                    Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * GlowOpacity);

                    if (Projectile.ai[0] % 8 == 0)
                    {
                        TSpeed++;
                    }

                    if (GlowOpacity < 1f)
                    {
                        GlowOpacity += 0.05f;
                    }

                    
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * -0.5f, Projectile.velocity.Y * -0.5f, 100 - ((int)GlowOpacity * 10), Color.Red, 1.0f);
                    d.noGravity = true;
                }
            }
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default, 1.0f);


            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(BombBlow, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.05f, 1.7f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);
            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Red * 0.5f, 0.1f, 0.05f, 1f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion2);

            Opus.RadialSpreadDustRandom(DustID.Blood, 10, Projectile.Center, 0, default, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 10, Projectile.Center, 0, Color.Red, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.Blood, 6, Projectile.Center, 0, default, 0.75f, 6);
        }
    }
}
