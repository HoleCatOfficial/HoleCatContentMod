using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class MageClone : ModProjectile
    {
        public int Variant;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Variant = Main.rand.Next(3);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Variant;

            SmallShine shine = new SmallShine();
            shine.Prepare(Projectile.Center, Vector2.Zero, Color.White, 1f);
            ParticleEngine.ShaderParticles.Add(shine);
        }

        public void ColorAffectedFX(Color color)
        {
            Lighting.AddLight(Projectile.Center, color.ToVector3() * Opus.Sine(0.3f, 0f));
            if (Main.rand.NextBool(10))
            {
                
            }
            //Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, 0, 0, 0, color, 1.5f);
            if (Projectile.ai[0] < 300)
            {
                Opus.RingSpreadDustRandom(DustID.TintableDustLighted, 3, ProjPos, 60, 0, color, 0.01f, 1f);
                
            }
        }
        
        public Vector2 ProjPos;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Variant == 0)
            {
                ColorAffectedFX(ColorLib.TenebrisMagenta);
            }
            if (Variant == 1)
            {
                ColorAffectedFX(ColorLib.TenebrisBlue);
            }
            if (Variant == 2)
            {
                ColorAffectedFX(ColorLib.TenebrisBeige);
            }

            Vector2 directionToMouse = Main.MouseWorld - Projectile.Center;
            Projectile.spriteDirection = directionToMouse.X > 0 ? 1 : -1;

            ProjPos = Projectile.Center + new Vector2(0, -40);
            Vector2 ShotDir = Main.MouseWorld - ProjPos;
            ShotDir = ShotDir.ToRotation().ToRotationVector2() * 20;

            

            Projectile.ai[0]++;

            
            if (Projectile.ai[0] >= 300)
            {
                if (Projectile.ai[0] % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, ProjPos);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, ShotDir, ModContent.ProjectileType<TenebrisFlamesFriendly>(), (int)(2f * player.GetDamage(DamageClass.Magic).Additive), 4, Projectile.owner);
                }
            }

        }
    }
}