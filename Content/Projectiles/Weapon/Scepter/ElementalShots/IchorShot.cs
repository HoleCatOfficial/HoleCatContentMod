using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots
{
    public class IchorShot : ElementalScepterShot
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            TrailType = 10;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            TrailColor = ColorLib.Ichor;
            DustColor = Color.White;
            TravelDust = DustID.IchorTorch;
            KillDust = DustID.IchorTorch;
            Projectile.Resize(16, 16);
            TrailAmplitude = 10f;

            Debuff = BuffID.Ichor;
            DebuffTime = 300;
            DetectionRad = 1200;
        }
    }
}