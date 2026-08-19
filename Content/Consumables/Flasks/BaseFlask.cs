using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public abstract class BaseFlask : ModItem
    {
        public abstract Color[] DrinkColors { get; }
        public abstract int BuffType { get; }
        public abstract Vector2 Dimensions { get; }
        public abstract int Rarity { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = DrinkColors;
        }
        public override void SetDefaults()
        {
            Item.width = (int)Dimensions.X;
            Item.height = (int)Dimensions.Y;
            Item.DefaultToFlask(BuffType, Rarity, Terraria.Item.sellPrice(0, 0, 5));
        }
    }
}
