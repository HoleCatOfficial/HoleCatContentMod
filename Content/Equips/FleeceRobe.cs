using DestroyerTest.Common;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    public class FleeceRobe : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 6;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public float DMGBonus => 1.04f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"));

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= DMGBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<BrownCloth>()
            .AddIngredient<CyanCloth>()
            .AddIngredient<LimeCloth>()
            .Register();
        }
	}
}