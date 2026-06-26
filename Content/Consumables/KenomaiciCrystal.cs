
using DestroyerTest.Common;
using DestroyerTest.Content.Tiles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Consumables
{
    internal class KenomaiciCrystal : ModItem
    {
        public static readonly int MaxKCrystals = 10;
        public static readonly int ManaPerCrystal = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerCrystal, MaxKCrystals);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ManaCrystal);
            
            Item.UseSound = DTAssetLib.Impacts.KCrystalConsume;
        }

        public override bool CanUseItem(Player player)
        {
            // This check prevents this item from being used before vanilla mana upgrades are maxed out.
            return player.ConsumedManaCrystals == Player.ManaCrystalMax;
        }

        public override bool? UseItem(Player player)
        {
            // Moving the exampleManaCrystals check from CanUseItem to here allows this example crystal to still "be used" like Mana Crystals can be
            // when at the max allowed, but it will just play the animation and not affect the player's max mana
            if (player.GetModPlayer<HeliciteManaPlayer>().KCrystals >= MaxKCrystals)
            {
                // Returning null will make the item not be consumed
                return null;
            }

            // This method handles permanently increasing the player's max mana and displaying the blue mana text
            player.UseManaMaxIncreasingItem(ManaPerCrystal);

            // This field tracks how many of the example crystals have been consumed
            player.GetModPlayer<HeliciteManaPlayer>().KCrystals++;

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_HeliciteCrystal>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }


    }

    public class HeliciteManaPlayer : ModPlayer
    {
        public int KCrystals = 0;
        byte HeliciteManaSyncMSG = (byte)DestroyerTestMod.MessageType.HeliciteManaSync;

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;
            mana.Base = KCrystals * KenomaiciCrystal.ManaPerCrystal;
            // Alternatively:  mana = StatModifier.Default with { Base = exampleManaCrystals * ExampleManaCrystal.ManaPerCrystal };
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)HeliciteManaSyncMSG);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)KCrystals);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            KCrystals = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            HeliciteManaPlayer clone = (HeliciteManaPlayer)targetCopy;
            clone.KCrystals = KCrystals;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            HeliciteManaPlayer clone = (HeliciteManaPlayer)clientPlayer;

            if (KCrystals != clone.KCrystals)
            {
                // This example calls SyncPlayer to send all the data for this ModPlayer when any change is detected, but if you are dealing with a large amount of data you should try to be more efficient and use custom packets to selectively send only specific data that has changed.
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }

        // NOTE: The tag instance provided here is always empty by default.
        // Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
        public override void SaveData(TagCompound tag)
        {
            tag["KCrystals"] = KCrystals;
        }

        public override void LoadData(TagCompound tag)
        {
            KCrystals = tag.GetInt("KCrystals");
        }
    }
}
