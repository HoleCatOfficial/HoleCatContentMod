using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{

    [AutoloadEquip(EquipType.Head)]
    public class ShadowMonarchHeadgear : ModItem
    {

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.ShadowScalemail && legs.type == ItemID.ShadowGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 0.05f;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(player.armor[0]);
            player.shadowArmor = true;
            player.GetKnockback(DamageClass.Generic) += 0.07f;
            player.ScepterClass().ThrowSpeedModifier += 0.30f;
            player.ScepterClass().Range += 40;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 15)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}