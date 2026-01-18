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
using InnoVault.PRT;
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
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class InfernalTiara : ModItem
    {
        
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            // By passing this (the ModItem) into the item parameter we can reference it later in GetEquipSlot with just the item's name
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}_Highlight", EquipType.Head, null, $"{Name}_Head_Highlight");

            /* Here is example code for supporting a female-specifig legs equip texture. See SetMatch as well.
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Female", EquipType.Legs, this, Name + "_Female");
			*/
        }

        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            //ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 10; // Height of the item
            Item.value = Item.sellPrice(gold: 8); // How many coins the item is worth
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>(); // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<InfernalDress>();
        }

        public override void UpdateArmorSet(Player player)
        {
            if (player.TryGetModPlayer<InfernalShieldPlayer>(out InfernalShieldPlayer Shield))
            {
                Shield.Active = true;
            }
            ScepterClassStats.Range += 2;
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

    public class InfernalShieldPlayer : ShieldPlayer
    {
        public override int MaxDurability => 136;
        private int _durability = 136;
		public override int Durability
		{
			get => _durability;
			set => _durability = Math.Clamp(value, 0, MaxDurability);
		}
        public override int Radius => 120;
        public override Color themeColor => ColorLib.HellFire;
        public override SoundStyle Regen => SoundID.Research;
        public override SoundStyle Break => new SoundStyle("DestroyerTest/Assets/Audio/TO_Break") with { PitchVariance = 0.3f };
        public override SoundStyle Hit => new SoundStyle("DestroyerTest/Assets/Audio/Impacts/IceImpact", 3);
        public override NetworkText[] DeathMSGs => new NetworkText[]
        {
            NetworkText.FromLiteral($"{Player.name} sacrificed themselves to the inferno."),
            NetworkText.FromLiteral($"{Player.name} gave a little too much in return for too little."),
            NetworkText.FromLiteral($"{Player.name} succumbed under the burden of the inferno."),
            NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
        };
        public override int RechargeHealthTax => 2;
    }
}