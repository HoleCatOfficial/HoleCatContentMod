using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GlowmaskHelper.Content;
using Terraria.Audio;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Summon;

namespace DestroyerTest.Content.SummonItems
{
    public class StarConstruct : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 0.5f;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<StarConstructInactive>();
            Item.width = 26;
            Item.height = 28;
            Item.UseSound = new SoundStyle("Destroyertest/Assets/Audio/Chroma_Throw") with { PitchVariance = 0.4f, MaxInstances = 0 };
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 22;
            Item.autoReuse = true;
            Item.buffTime = 120;
            Item.buffType = ModContent.BuffType<StarConstructMinionBuff>();
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<StellarMatter>(), 16)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
		
	}
}