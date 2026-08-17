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
    public class FrigidFenzim : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<FrigidFenzimThrown>();
            Item.width = 58;
            Item.height = 58;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.damage = 20;
            Item.autoReuse = true;
            Item.crit = 15;
        }


    }
}