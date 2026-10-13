using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.EntitiesProjectiles
{
    public class GazerShadow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.Red));
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.EnergyBounce with { Pitch = -0.7f, PitchVariance = 0.1f }, Projectile.Center);
            for (int i = 0; i < 9; i++)
            {
                Dust spawn = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(10f, 10f), 0, Color.Red, Main.rand.NextFloat(1f, 4f));
                spawn.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 30)
            {

            }
            else
            {
                if (Projectile.ai[0] < 300)
                {
                    Projectile.velocity *= 1.04f;
                }

            }
        }
    }
}
