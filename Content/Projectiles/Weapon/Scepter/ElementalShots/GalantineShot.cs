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

        public int inittime;
        public override void SetDefaults()
        {
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

            /*
            float T = (inittime - Projectile.timeLeft) / (float)inittime * 4f;
			TrailColor = ColorLib.StellarFireGradient(T);
			DustColor = ColorLib.StellarFireGradient(T);
            */
            TrailColor = ColorLib.StellarFireGradientLooping();
            DustColor = ColorLib.StellarFireGradientLooping();
			TravelDust = DustID.TintableDustLighted;
			KillDust = DustID.TintableDustLighted;
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;
            inittime = Projectile.timeLeft;

            Debuff = ModContent.BuffType<GalantineBurn>();
            DebuffTime = 300;
            DetectionRad = 1200;
        }

        public override void PostAI()
        {
            /*
            float T = (inittime - Projectile.timeLeft) / (float)inittime * 4f;
            TrailColor = ColorLib.StellarFireGradient(T);
            DustColor = ColorLib.StellarFireGradient(T);
            */

            TrailColor = ColorLib.StellarFireGradientLooping();
            DustColor = ColorLib.StellarFireGradientLooping();
        }
    }
}