using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class CrystalBlade : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 70;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 200;
            Item.knockBack = 4f;
            Item.crit = 16;

            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ModContent.RarityType<HallowedSpecialRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<CrystalBladeSwing>();
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
                .AddIngredient(ItemID.CrystalShard, 24)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
