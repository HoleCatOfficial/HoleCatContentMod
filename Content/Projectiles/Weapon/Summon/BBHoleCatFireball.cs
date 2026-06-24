using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BBHoleCatFireball : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 7f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 3f;

        float IHomingProjectile.DetectRadius => 1000;

        bool IHomingProjectile.CanHome => willHome && DelayTimer >= 30;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 360;
            Projectile.frame = 0;
            Projectile.ArmorPenetration = 12;
            Projectile.extraUpdates = 3;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            
            return false;
        }

        private bool willHome;

        public override void OnSpawn(IEntitySource source)
        {
            willHome = Main.rand.NextBool(4);
        }

        int DelayTimer = 0;
        public override void AI()
        {

            LerpingFire F = new();
            F.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, ColorLib.HoleCatFireColormap, 0.75f, 60, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(F);

            Fire F2 = new();
            F2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, Color.White, 0.5f, 60, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(F2);


            if (DelayTimer < 30)
            {
                DelayTimer += 1;
                return;
            }


           


        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            
            for (int i = 0; i < 8; i++)
            {
                LerpingFire F = new();
                F.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(1.3f, 1.3f), DTUtils.RandomDirection(2), 0.02f, ColorLib.HoleCatFireColormap, 0.5f, 60, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(F);
            }
        }

    }
}
