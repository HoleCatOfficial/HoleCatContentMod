using System.IO;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;

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
			TileKill = false;
			Homing = true;
			MaxTileHitCount = 4;
			Projectile.Resize(16, 16);
			TrailAmplitude = 20f;
			Debuff = BuffID.Poisoned;
			DebuffTime = 600;
		}
	}
}