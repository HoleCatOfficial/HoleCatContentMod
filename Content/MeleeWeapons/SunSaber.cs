using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class SunSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1f;
        }
        public override void SetDefaults()
        {
            Item.width = 118;
            Item.height = 118;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 200;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 70);
            Item.rare = ItemRarityID.Master;
            Item.shoot = ModContent.ProjectileType<SunSaberSwing>();
            Item.noUseGraphic = true;
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
                .AddIngredient(ItemID.OrangePhasesaber, 1)
                .AddIngredient(ItemID.FragmentSolar, 24)
                .Register();
        }
    }
}