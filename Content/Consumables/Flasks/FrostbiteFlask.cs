using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Buffs.Imbues;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class FrostbiteFlask : BaseFlask
    {
        public override Color[] DrinkColors => new Color[2] { Color.SkyBlue, Color.DeepSkyBlue };

        public override int BuffType => ModContent.BuffType<WeaponImbueFrostbite>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.Blue;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn, 2)
                .AddIngredient(ItemID.SoulofNight, 1)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}
