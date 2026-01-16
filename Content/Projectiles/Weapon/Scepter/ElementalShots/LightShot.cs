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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots
{
	public class LightShot : ElementalScepterShot
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

			TrailColor = Main.DiscoColor;
			DustColor = Color.White;
			TravelDust = DustID.RainbowRod;
			KillDust = DustID.RainbowRod;
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;

            DetectionRad = 1200;
        }
    }
}