using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
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
    public class NeglectedRegards : ModItem
    {

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
