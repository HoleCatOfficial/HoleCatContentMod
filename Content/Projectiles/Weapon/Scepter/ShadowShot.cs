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
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
	public class ShadowShot : ScepterShot
	{
		public override void SetStaticDefaults()
		{
			TrailType = 4;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

			TrailColor = new Color(179, 54, 201);
			DustColor = new Color(179, 54, 201);
			BounceDust = KillDust;
			KillDust = DustID.FireworksRGB;
			TileBounce = true;
			TileKill = false;
			Homing = false;
			MaxTileHitCount = 6;
			Projectile.Resize(16, 16);
			TrailAmplitude = 20f;
			Debuff = BuffID.ShadowFlame;
			DebuffTime = 600;
			BounceSound = new SoundStyle("DestroyerTest/Assets/Audio/Charge/Anvil") { PitchVariance = 0.3f, MaxInstances = 0 };
		}
	}
}