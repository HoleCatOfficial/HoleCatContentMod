using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using System.Numerics;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.Equips.DistendedSet
{
    [AutoloadEquip(EquipType.Legs)]
    public class DistendedCuisses : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = Item.sellPrice(gold: 1); 
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.velocity.Length() > 1f && !player.mount.Active)
            {
                Dust.NewDustDirect(player.Bottom, 2, 1, DustID.Wraith, 0, 0.02f, 100, new Color(184, 228, 242), 1);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonGreaves, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}