using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Fargos.EternityDrops
{
    public class UnyieldingSkull : OneTimeConsumable
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 22;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 180;
            Item.useTime = 180;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle(DTAssetLib.AudioPath + "/UnyieldingSkullConsume");
            Item.maxStack = 1;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player)
        {
            if (player.TryGetModPlayer<UnyieldingSkullUpgrade>(out var upgrade))
            {
                upgrade.PermaBuff = true;
            }
            return true;
        }

        public override bool Consumed()
        {
            if (Main.LocalPlayer.TryGetModPlayer<UnyieldingSkullUpgrade>(out var upgrade))
            {
                return upgrade.PermaBuff;
            }
            return true;
        }

        public override void OnConsume(Player player)
        {
            if (player.TryGetModPlayer<UnyieldingSkullUpgrade>(out var upgrade))
            {
                upgrade.PermaBuff = true;
            }
        }
    }

    public class UnyieldingSkullUpgrade : ModPlayer
    {
        public bool PermaBuff = false;

        public override void PostUpdateMiscEffects()
        {
            if (PermaBuff)
            {
                Player.GetCritChance(DamageClass.Summon) += 8f;
                Player.GetKnockback(DamageClass.Summon) += 0.22f;
                Player.maxTurrets += 1;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("US_consumed", PermaBuff);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("US_consumed"))
            {
                PermaBuff = tag.GetBool("US_consumed");
            }
        }
    }
}