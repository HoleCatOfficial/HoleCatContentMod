using DestroyerTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Expedition : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 1;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.findTreasure = true;

            player.tileSpeed += 0.15f;
            player.wallSpeed += 0.15f;

            player.pickSpeed += 0.08f;
        }


    }

    public class ExpeditionGlobal : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if ((item.type == ItemID.IronCrate || item.type == ItemID.WoodenCrate))
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Expedition>(), 20, 1, 1));
            }
        }
    }
}
 