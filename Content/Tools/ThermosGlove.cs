using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.Tools;

using System.Collections.Generic;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Projectiles;


namespace DestroyerTest.Content.Tools
{

    public class ThermosGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 60;
            Item.useAnimation = 60;

            Item.DamageType = DamageClass.Generic;
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ThermosGloveProjectile>();
            Item.useTurn = true;
        }

        public override void UpdateInventory(Player player)
        {
            
        }

      
    }
}
