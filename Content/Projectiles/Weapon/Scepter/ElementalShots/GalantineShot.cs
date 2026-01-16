using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots
{
	public class GalantineShot : ElementalScepterShot
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

            float T = Projectile.timeLeft / 600;
			TrailColor = ColorLib.StellarFireGradient(T);
			DustColor = ColorLib.StellarFireGradient(T);
			TravelDust = DustID.TintableDustLighted;
			KillDust = DustID.TintableDustLighted;
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;

            Debuff = ModContent.BuffType<GalantineBurn>();
            DebuffTime = 300;
            DetectionRad = 1200;
        }

        public override void PostAI()
        {
            float T = Projectile.timeLeft / 600;
            TrailColor = ColorLib.StellarFireGradient(T);
            DustColor = ColorLib.StellarFireGradient(T);
        }
    }
}