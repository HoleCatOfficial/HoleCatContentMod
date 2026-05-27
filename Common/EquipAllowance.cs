using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using DestroyerTest.Content.Equips;

namespace DestroyerTest.Common
{
    public class EquipAllowance : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (incomingItem.ModItem is ShimmeringGauntlet G)
            {
                if (equippedItem.type == ItemID.FireGauntlet && incomingItem.type == G.Item.type)
                {
                    return false;
                }
            }


            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }
    }
}
