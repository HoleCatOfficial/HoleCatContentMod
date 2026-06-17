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
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.ParentClasses;

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
            TrailColor = MainColor;
            DustColor = MainColor;
			TravelDust = ModContent.DustType<ColorableNeonDust>();
			KillDust = ModContent.DustType<ColorableNeonDust>();
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;
            inittime = Projectile.timeLeft;

            Debuff = ModContent.BuffType<GalantineBurn>();
            DebuffTime = 300;
            DetectionRad = 1200;
        }

        public int Lifetime = 120;
		public int Time = 0;

        public Color MainColor = Color.White;

		public bool StartKill = false;
		public void UpdateLerpTime()
		{
			Time++;

			if (Time > Lifetime)
			{
				StartKill = true;
			}
		}
		public float LifetimeCompletion
		{
			get
			{
				if (Lifetime <= 0)
				{
					return 0f;
				}

				return (float)Time / (float)Lifetime;
			}
		}


        public override void PostAI()
        {
            UpdateLerpTime();
			MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            TrailColor = MainColor;
            DustColor = MainColor;
        }
    }
}