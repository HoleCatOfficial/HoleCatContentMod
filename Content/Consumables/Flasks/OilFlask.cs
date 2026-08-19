using DestroyerTest.Common;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Buffs.Imbues;

namespace DestroyerTest.Content.Consumables.Flasks
{
	public class OilFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.Black];

        public override int BuffType => BuffID.WeaponImbueFire;

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.White;
    }
}