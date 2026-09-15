using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class ZyplonRing : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 320;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<VesperRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ZyplonRingDodge>().Active = true;
        }
    }

    public class ZyplonRingDodge : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (Active)
            {
                return Main.rand.NextBool(6, 100);
            }
            return false;
        }
    }
}
