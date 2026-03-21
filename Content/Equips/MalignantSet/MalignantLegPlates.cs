using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using System.Drawing;
using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.MalignantSet
{
    [AutoloadEquip(EquipType.Legs)]
    [AutoloadGlowmask]
    public class MalignantLegPlates : ModItem
    {
        public override void Load()
        {
            GlowmaskLoader.QueueGlowmaskRegistration($"{Texture}_Legs_Glow");
        }

        public override void SetStaticDefaults()
        {
            GlowmaskLoader.AssignGlowmaskTexture_Equip(Item.glowMask, EquipType.Legs, EquipLoader.GetEquipSlot(Mod, "MalignantLegPlates_Legs", EquipType.Legs));
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WretchedShards>(5)
                .AddIngredient(ItemID.SpectreBar, 8)
                .AddIngredient(ItemID.EbonstoneBlock, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}