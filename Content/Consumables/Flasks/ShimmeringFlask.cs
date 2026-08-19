using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class ShimmeringFlask : BaseFlask
    {
        public override Color[] DrinkColors => [ColorLib.TenebrisMagenta, ColorLib.TenebrisBeige, ColorLib.TenebrisBlue];

        public override int BuffType => ModContent.BuffType<WeaponImbueShimmeringFlames>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ModContent.RarityType<ShimmeringRarity>();
    }
}
