
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class BroochOfBalance : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 44;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<NightInferno>()] = true;
            player.buffImmune[ModContent.BuffType<LightInferno>()] = true;
            player.noKnockback = true;
        }

        public static List<int> Shields = new List<int>
        {
            ItemID.EoCShield,
            ModContent.ItemType<NobleBulwarkGold>(),
            ModContent.ItemType<NobleBulwarkPlatinum>(),
            ItemID.CobaltShield,
            ItemID.SquireShield,
            ItemID.ObsidianShield,
            ItemID.PaladinsShield,
            ItemID.HeroShield,
            ItemID.FrozenShield,
            ItemID.AnkhShield,
        };


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BroochOfLight>()
                .AddIngredient<BroochOfNight>()
                .AddIngredient(ItemID.SoulofMight, 8)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddIngredient(ItemID.SoulofSight, 8)
            .Register();
        }
    }
}