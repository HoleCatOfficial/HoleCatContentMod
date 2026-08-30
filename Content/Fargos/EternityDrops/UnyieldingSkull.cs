using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Fargos.EternityDrops
{
    public class UnyieldingSkull : ModItem
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
            Item.useAnimation = 15;
            Item.useTime = 90;
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
    }
}