using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Consumables
{
    public class LunacyParasite : OneTimeConsumable
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override bool Consumed()
        {
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                if (Main.LocalPlayer.TryGetModPlayer<LunacySlotPlayer>(out var slot))
                {
                    return slot.HasSlot;
                }
            }
            return false;
        }

        public override void SetDefaults()
        {
            Item.UseSound = new SoundStyle(DTAssetLib.AudioPath + "/LunacyParasiteConsume");
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
            Item.width = 54;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 8, 50);
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.maxStack = 1;
        }

        public override void OnConsume(Player player)
        {
            if (player.TryGetModPlayer<LunacySlotPlayer>(out var slot))
            {
                slot.HasSlot = true;
            }
        }
    }

    public class LunacySlotPlayer : ModPlayer
    {
        public bool HasSlot = false;
        public override void PostUpdateMiscEffects()
        {

        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("LP_consumed", HasSlot);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("LP_consumed"))
            {
                HasSlot = tag.GetBool("LP_consumed");
            }
        }
    }


    public class LunacySlot : ModAccessorySlot
    {
        public override string FunctionalTexture => DTAssetLib.ExtrasPath + "/LunacySlotIcon";

        public override bool IsEnabled()
        {
            
            if (!Main.gameMenu)
            {
                return Main.LocalPlayer.GetModPlayer<LunacySlotPlayer>().HasSlot;
            }

            return false;
        }

    }
}