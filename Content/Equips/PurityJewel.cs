using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class PurityJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 8));
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<DPlayer>(out DPlayer Defile))
            {
                Defile.HasPurityJewel = true;
            }

            player.panic = true;
            player.sunflower = true;
            player.accRunSpeed += 0.25f;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient(ItemID.ManaRegenerationBand)
                .AddIngredient(ItemID.LifeCrystal)
                .AddIngredient(ItemID.LifeFruit)
                .AddIngredient(ItemID.PurificationPowder, 3)
                .AddIngredient(ItemID.Sunflower)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
            
        }
    }

    public class PurityJewelPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void UpdateLifeRegen()
        {
            if (Active)
            {
                Player.lifeRegen += 22;
                Player.manaRegen += 18;
            }
        }


    }
}
