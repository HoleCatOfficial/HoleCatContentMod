using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
 
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;
using DestroyerTest.Content.RiftArsenal;
using Steamworks;
using System.Collections.Generic;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Head)]
    public class InfernalTiara : ModItem
    {
        public static List<NetworkText> DMSG = new()
        {
            NetworkText.FromLiteral($"{Main.LocalPlayer.name} sacrificed themselves to the inferno."),
            NetworkText.FromLiteral($"{Main.LocalPlayer.name} gave a little too much in return for too little."),
            NetworkText.FromLiteral($"{Main.LocalPlayer.name} succumbed under the burden of the inferno."),
            NetworkText.FromLiteral($"{Main.LocalPlayer.name} didnt have it in them to sustain their shield.")
        };

        public Shield InfernalShield = new Shield("InfernalShield", 136, 120, ColorLib.HellFire, SoundID.Research, SoundID.Item51, new SoundStyle("DestroyerTest/Assets/Audio/TO_Break") with { PitchVariance = 0.3f }, DMSG, 2, 2);
        public override void Load()
        {
           
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}_Highlight", EquipType.Head, null, $"{Name}_Head_Highlight");
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 10;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 8;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<InfernalDress>();
        }

        public override void UpdateArmorSet(Player player)
        {
            
            player.ScepterClass().Range += 2;
            player.lavaImmune = true;
            player.DefaultSetBonusText(player.armor[0]);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Item body = new Item();
			body.SetDefaults(ModContent.ItemType<InfernalDress>());
			Item legs = new Item();
			legs.SetDefaults(0);
			if (IsArmorSet(Item, body, legs))
			{
				var pityText = Language.GetText("Mods.DestroyerTest.ShieldPlayer.ShieldLine");
				tooltips.Add(new TooltipLine(Mod, "ShieldInfo", pityText.Value));
			}
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}