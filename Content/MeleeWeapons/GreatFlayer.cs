
using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class GreatFlayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.TooltipScaleMult[Type] = 1.1f;
            DTUtils.isSpecialSwingSword[Type] = true;

            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<FleshVolley>();
            }
        }
        public override void SetDefaults()
        {

            Item.width = 160;
            Item.height = 160;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 17;
            Item.autoReuse = true;
            Item.damage = 1100;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.SetSpecialMeleeStats();

            Item.shoot = ModContent.ProjectileType<GreatFlayerProjectile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<GreatFlayerDash>()] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.WGIIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<WyvernCorpseArtifact>(3)
                    .Register();
            }
        }
    }
}