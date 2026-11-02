using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class NeglectedRegards : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Unrest>();
        }

        public override void SetDefaults()
        {
            Item.width = 78;
            Item.height = 78;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 13;
            Item.knockBack = 1f;
            Item.crit = 7;

            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<NeglectedRegardsSwing>();
            Item.channel = true;


        }


        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NeglectedRegards>()
                .AddIngredient<Dyrn>(16)
                .Register();
        }
    }
}
