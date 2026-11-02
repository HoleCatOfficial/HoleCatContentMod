using Microsoft.Xna.Framework;
using Terraria.ID;

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