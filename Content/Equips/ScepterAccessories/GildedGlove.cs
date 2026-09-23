
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class GildedGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.PirateShip, ItemDropRule.Common(Type, 21));
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.vanity = false;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ScepterClass>() += 0.06f;
            player.ScepterClass().Range += 60;
            player.ScepterClass().ThrowSpeedModifier += 0.3f;
        }
    }
}
