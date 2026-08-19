using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class WitheringFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.Magenta, Color.DeepPink];

        public override int BuffType => ModContent.BuffType<WeaponImbueWithering>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.Purple;
    }
}
