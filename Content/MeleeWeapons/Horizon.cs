using DestroyerTest.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using System;
using OpusLib;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;

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
			Item.useTime = 2;
			Item.useAnimation = 2;
			Item.useStyle = ItemUseStyleID.Shoot;

			Item.shoot = ModContent.ProjectileType<TrackingFireSlash>();
			Item.damage = 4;
			Item.channel = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shootSpeed = 1f;
		}
	}
}