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
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots
{
	public class LightShot2 : ElementalScepterShot
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

			TrailColor = new Color(220, 29, 183);
			DustColor = new Color(220, 29, 183);
			TravelDust = ModContent.DustType<ColorableNeonDust>();
			KillDust = ModContent.DustType<ColorableNeonDust>();
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;

            Debuff = ModContent.BuffType<LightInferno>();
            DebuffTime = 300;
            DetectionRad = 1200;
        }
    }
}