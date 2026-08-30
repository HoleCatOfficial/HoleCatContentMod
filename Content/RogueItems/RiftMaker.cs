using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
	public class RiftMaker : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DTUtils.LegendaryWeapon[Type] = true;
			OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.Mothron, ItemDropRule.Common(Type, 10));
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 17.5f;
			Item.shoot = ModContent.ProjectileType<RiftMaker_Thrown>();
			Item.width = 92;
			Item.height = 92;
			Item.maxStack = 1;
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/ZenithSound") { PitchVariance = 0.4f, MaxInstances = 0 };
            Item.useAnimation = 120;
			Item.useTime = 120;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.damage = 80;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Throwing;
        }

		
	}


}