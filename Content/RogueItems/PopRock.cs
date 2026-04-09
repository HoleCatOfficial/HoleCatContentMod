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
using DestroyerTest.Content.Projectiles.Weapon.Rogue;

namespace DestroyerTest.Content.RogueItems
{
    public class PopRock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<PopRockThrown>();
            Item.width = 32;
            Item.height = 32;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<RiftRarity1>();
            Item.damage = 120;
            Item.autoReuse = true;
        }
    }
}