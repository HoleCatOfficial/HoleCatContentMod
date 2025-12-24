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

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class EnchantedShot : ScepterShot
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

			TrailColor = Color.SkyBlue;
			DustColor = Color.White;
			BounceDust = DustID.FireworksRGB;
			KillDust = BounceDust;
			TileBounce = true;
			Homing = false;
			MaxTileHitCount = 4;
			Projectile.Resize(16, 16);
			TrailAmplitude = 20f;
        }
    }
}