using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class LightFireball : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public override void AI()
        {
            Projectile.rotation += 0.1f * Projectile.direction;

            for (int f = 0; f < 3; f++)
            {
                Dust Trail = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableNeonDust>(), newColor: ColorLib.SoulOfLightColor, Scale: 1.5f);
                Trail.noGravity = true;
                Trail.velocity = Vector2.Zero;
            }

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, Projectile.direction, 0.14f, ColorLib.SoulOfLightColor, 1f, 100, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<LightInferno>(), 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);
        }
    }
}