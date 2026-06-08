
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
using DestroyerTest.Content.Tiles;
using Terraria.Localization;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
	public class ShineShadeDeck : ModItem
	{
        public Shield ShineShadeShield = new Shield("HollowShield2", 330, 130, ColorLib.Rift, DTAssetLib.ScholarShieldSounds.Activate, DTAssetLib.Impacts.Deflect, DTAssetLib.Impacts.HeatseekerSilohSlam,
            new List<NetworkText>()
            {
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} felt a little hollow inside."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} gave a little too much in return for too little."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} fell victim to the eclipse."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} didnt have it in them to sustain their shield.")
            },
            20, 10
        );

        public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity2>();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.GetDamage(DamageClass.Ranged) *= 1.05f;
            player.whipRangeMultiplier *= 1.08f;
            player.maxMinions += 1;
            player.GetAttackSpeed(DamageClass.Ranged) *= 0.95f;
            player.GetArmorPenetration(DamageClass.Generic) += 6;

            ShieldManager.ActivateShield(ShineShadeShield, player);
            player.statDefense += 10;

            foreach (Item item in Main.item)
            {
                int tm = player.GetItemGrabRange(item);
                tm = (int)(tm * 1.10f);
            }
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var shieldText = Language.GetText("Mods.DestroyerTest.ShieldPlayer.ShieldLine");
			tooltips.Add(new TooltipLine(Mod, "ShieldInfo", shieldText.Value));
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<Scourge>() || incomingItem.type != ModContent.ItemType<Hollow>() || incomingItem.type != ModContent.ItemType<Vortex>() || incomingItem.type != Type;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_HeliciteCrystal>(30)
                .AddIngredient<Scourge>()
                .AddIngredient<Hollow>()
                .AddIngredient<Vortex>()
            .Register();
        }
    }
}