using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{

    [AutoloadEquip(EquipType.Head)]
    public class CrimsonHeadgear : ModItem
    {

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;

        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.CrimsonScalemail && legs.type == ItemID.CrimsonGreaves;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.03f;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(player.armor[0]);
            player.crimsonRegen = true;
            player.GetDamage<ScepterClass>() += 0.19f;
            player.ScepterClass().ThrowSpeedModifier *= 1.30f;
            player.ScepterClass().Range += 40;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 15)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}