using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using rail;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.MalignantSet
{
    [AutoloadEquip(EquipType.Body)]
    public class MalignantBodyArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34; 
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<MalignantBodyAmmoReserve>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WretchedShards>(7)
                .AddIngredient(ItemID.SpectreBar, 9)
                .AddIngredient(ItemID.EbonstoneBlock, 9)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class MalignantBodyAmmoReserve : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (Active && weapon.DamageType == DamageClass.Ranged)
            {
                if (Main.rand.NextBool(10))
                {
                    return false;
                }
            }
            return true;
        }
    }
}