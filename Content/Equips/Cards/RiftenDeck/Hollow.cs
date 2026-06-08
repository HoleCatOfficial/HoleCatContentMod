
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.Localization;
using System;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Graphics.Pixelation;
using OpusLib;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
	public class Hollow : ModItem
	{
        public Shield HollowShield = new Shield("HollowShield", 200, 70, ColorLib.Rift, DTAssetLib.ScholarShieldSounds.Activate, DTAssetLib.Impacts.Deflect, DTAssetLib.Impacts.HeatseekerSilohSlam,
            new List<NetworkText>()
            {
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} felt a little hollow inside."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} gave a little too much in return for too little."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} fell victim to the eclipse."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} didnt have it in them to sustain their shield.")
            },
            10, 4
        );
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 11));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity1>();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            ShieldManager.ActivateShield(HollowShield, player);

            player.statDefense += 10;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var shieldText = Language.GetText("Mods.DestroyerTest.ShieldPlayer.ShieldLine");
			tooltips.Add(new TooltipLine(Mod, "ShieldInfo", shieldText.Value));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Living_Shadow>(45)
                .AddIngredient<Item_RiftClay>(25)
            .Register();
        }
    }
}