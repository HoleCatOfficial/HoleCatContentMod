using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.GameContent.Drawing;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using OpusLib;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class DarkRaptureExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void SetStaticDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            ShockwaveExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisMagenta, 0.04f, 0.001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(Explosion);

            foreach (Dust dust in Opus.RadialSpreadDust(DustID.FireworksRGB, 40, Projectile.Center, 0, ColorLib.TenebrisMagenta, 2f, 8))
            {
                dust.noGravity = true;
            }

           // Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 4, Projectile.Center, 0, ColorLib.TenebrisMagenta, 1f, 5);
            //Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 4, Projectile.Center, 0, ColorLib.TenebrisMagenta, 1f, 7);
            //Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 6, Projectile.Center, 0, ColorLib.TenebrisMagenta, 1f, 9);
        }

        public override void AI()
        {

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }

}

