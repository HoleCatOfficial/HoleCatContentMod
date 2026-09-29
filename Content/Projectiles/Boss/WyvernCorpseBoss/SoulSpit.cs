using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulSpit : ModProjectile
    {

        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 12; 
            Projectile.friendly = false;
            Projectile.hostile = true; 
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }



        public override void OnSpawn(IEntitySource source)
        {
            
        }

        public override void AI()
        {
            PointGlowPreMultiplied glow = new();
            glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(2, 2), ColorLib.Soul, 2f);
            ParticleEngine.Particles.Add(glow);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}