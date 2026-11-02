using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class NatureShot : ScepterShot
    {
        public override void SetStaticDefaults()
        {
            TrailType = 10;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

            TrailColor = Color.LimeGreen;
            DustColor = Color.LimeGreen;
            BounceDust = KillDust;
            KillDust = DustID.FireworksRGB;
            TileBounce = false;
            TileKill = true;
            Homing = true;
            MaxTileHitCount = 4;
            Projectile.Resize(16, 16);
            TrailAmplitude = 20f;
            Debuff = BuffID.Poisoned;
            DebuffTime = 600;
        }
    }
}