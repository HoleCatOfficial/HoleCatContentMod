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
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{

    public class Zwei : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 17f;
            Item.shoot = ModContent.ProjectileType<LightKnifeThrown>();
            Item.width = 34;
            Item.height = 72;
            Item.UseSound = new SoundStyle("Destroyertest/Assets/Audio/SwordSounds/QuickSwing", 4) with { PitchVariance = 0.4f, MaxInstances = 0 };
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Expert;
            Item.damage = 80;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Throwing;
        }

        int usage = 0;

        public override bool? UseItem(Player player)
        {
            usage++;
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = usage % 2 == 0 ? ModContent.ProjectileType<LightKnifeThrown>() : ModContent.ProjectileType<NightKnifeThrown>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddIngredient(ItemID.PalladiumBar, 4)
                .AddIngredient(ItemID.GoldBar, 4)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddIngredient(ItemID.CobaltBar, 4)
                .AddIngredient(ItemID.GoldBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}