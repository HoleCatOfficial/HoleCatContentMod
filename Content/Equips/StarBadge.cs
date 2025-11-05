using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	public class StarBadge : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToVanitypet(ModContent.ProjectileType<ConstitutionPet>(), ModContent.BuffType<ConstitutionPetBuff>());

			Item.width = 22;
			Item.height = 26;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.sellPrice(0, 5);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            return true;
        }
        
        public override void UpdateEquip(Player player)
        {
            player.AddBuff(Item.buffType, 2);
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                EntitySource_ItemUse source = new EntitySource_ItemUse(player, Item);
                Projectile.NewProjectile(source, player.Center, Vector2.One, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
            }
        }
	}
}