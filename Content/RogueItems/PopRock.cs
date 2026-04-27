using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.DamageType = ModContent.GetInstance<DTRogueClass>();
        }
    }
}