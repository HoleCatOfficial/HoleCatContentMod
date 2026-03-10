using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.Tools;

using System.Collections.Generic;



namespace DestroyerTest.Content.RiftArsenal
{
    public class RechargeItem : ModItem
    {
        public static bool Energized = false;

        public override void UpdateInventory(Player player)
        {
            var modPlayer = player.GetModPlayer<LivingShadowPlayer>();
            if (modPlayer.LivingShadowCurrent > 0)
            {
                Energized = true;
            }
            if (modPlayer.LivingShadowCurrent <= 0)
            {
                Energized = false;
            }
        }
	}



}
