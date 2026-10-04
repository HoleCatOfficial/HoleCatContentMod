using DestroyerTest;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.AncientSword
{
    [AutoloadGlowmask]
    public class Tibel : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 128;
            Item.height = 128;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 1000;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                if (DTCrossMod.CallOfVoidMod.TryFind("VoidPurple", out ModRarity voidPurple))
                {
                    Item.rare = voidPurple.Type;
                }
            }
            Item.shoot = ModContent.ProjectileType<TibelSwing>();
            Item.noUseGraphic = true;
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
            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                if (DTCrossMod.CallOfVoidMod.TryFind("VoidBar", out ModItem VoidBar) && DTCrossMod.CallOfVoidMod.TryFind("StarlessNight", out ModItem StarlessNight) && DTCrossMod.CallOfVoidMod.TryFind("VoidWellTile", out ModTile voidWell))
                {
                    CreateRecipe()
                        .AddIngredient<Moongeist>()
                        .AddIngredient(StarlessNight.Type, 1)
                        .AddIngredient(VoidBar.Type, 6)
                        .AddTile(voidWell.Type)
                        .Register();
                }
            }

        }
    }
}