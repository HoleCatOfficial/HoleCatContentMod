using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	public class AmalgamatedFragments : ModItem
	{
		public override void SetDefaults() {
            //Item.DefaultToVanitypet(ModContent.ProjectileType<ConstitutionPet>(), ModContent.BuffType<ConstitutionPetBuff>());

            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<CursedNodePet>();
            Item.buffType = ModContent.BuffType<NodesPetBuff>();

			Item.width = 22;
			Item.height = 26;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.sellPrice(0, 5);
		}

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
            return true;
        }
	}
}