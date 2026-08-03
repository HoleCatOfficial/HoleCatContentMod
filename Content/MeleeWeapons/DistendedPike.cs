using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class DistendedPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Scorn>();
        }

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 94;

            Item.rare = ModContent.RarityType<PrimalRarity>();
            Item.value = Item.sellPrice(silver: 10);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.autoReuse = true;

            Item.damage = 150;
            Item.knockBack = 3f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = false;

            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<DistendedPikeProjectile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}

		