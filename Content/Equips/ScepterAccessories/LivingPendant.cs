
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class LivingPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 54;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 9)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class LivingPendantPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                
            }
        }
    }
}