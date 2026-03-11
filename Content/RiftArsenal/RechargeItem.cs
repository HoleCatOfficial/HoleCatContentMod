using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Tools;
using DestroyerTest.Rarity;
using Fargowiltas.Items.Summons.SwarmSummons.Energizers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;



namespace DestroyerTest.Content.RiftArsenal
{
    public interface IRechargeFunctionality
    {
        bool Energized { get; }
    }
    public class RechargeItem : ModItem
    {
        //Husk class to prevent reference errors, since 20 files reference this class

        public override void UpdateInventory(Player player)
        {
            
        }
	}

    public class Recharge : ModPlayer, IRechargeFunctionality
    {
        public bool Energized { get; private set; }

        public override void PreUpdate()
        {
            var shadow = Player.GetModPlayer<LivingShadowPlayer>();
            Energized = shadow.LivingShadowCurrent > 0;
        }
    }
}
