using DestroyerTest.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.ConstitutionBoss;
using OpusLib;

namespace DestroyerTest.Content.MeleeWeapons
{

	public class Horizon : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.height = 39;
			Item.width = 39;
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.shoot = ModContent.ProjectileType<RGBSlash>();
			Item.damage = 20;
			Item.shootSpeed = 20;
			Item.channel = true;
			Item.noUseGraphic = true;
		}
	}
}