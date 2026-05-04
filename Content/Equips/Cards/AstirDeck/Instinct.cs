using DestroyerTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Instinct : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<InstinctPlayer>(out var instinct))
            {
                instinct.Active = true;
            }
        }
    }

    public class  InstinctPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        int dodgeCooldown = 0;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (dodgeCooldown > 0)
                {
                    dodgeCooldown--;
                }
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            SoundEngine.PlaySound(DTAssetLib.ChargeBreak, Player.Center);
            dodgeCooldown = 1200;
            return dodgeCooldown <= 0 && Active;
        }
    }
}
