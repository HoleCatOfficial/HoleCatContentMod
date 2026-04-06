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
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Common.Systems;

namespace DestroyerTest.Content.Equips.DivineServantSet
{
    [AutoloadEquip(EquipType.Head)]
    public class DivineServantHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ModContent.RarityType<SoulRarity>();
            Item.defense = 16;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DivineServantRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            DivineServantSystem.IsServant[player.whoAmI] = true;
            player.DefaultSetBonusText(player.armor[0]);
        }
    }
}