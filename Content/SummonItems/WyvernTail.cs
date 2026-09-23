

using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems
{
	public class WyvernTail : ModItem
	{
        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GreatFlayer>();
            }
        }
		public override void SetDefaults() {
			// This method quickly sets the whip's properties.
			// Mouse over to see its parameters.
			Item.DefaultToWhip(ModContent.ProjectileType<WyvernTailProjectile>(), 140, 2, 4);
            Item.width = 36;
            Item.height = 32;
            Item.autoReuse = true;

			Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
		}

		// Makes the whip receive melee prefixes
		public override bool MeleePrefix() {
			return false;
		}

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<WyvernCorpseArtifact>(3)
                    .Register();
            }
        }
    }
}