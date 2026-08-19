using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Consumables.Flasks;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class BrineFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.SkyBlue, Color.White];

        public override int BuffType => ModContent.BuffType<WeaponImbueBrine>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.White;

    }
}