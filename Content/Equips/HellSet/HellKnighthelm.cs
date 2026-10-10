using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.HellSet
{
    [AutoloadEquip(EquipType.Head)]
    public class HellKnighthelm : ModItem
    {


        public override void SetStaticDefaults()
        {

            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 14;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HellPlatemail>() && legs.type == ModContent.ItemType<HellBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(Item);
            player.GetDamage(DamageClass.Melee) += 0.12f;
            player.endurance += 0.2f;
            player.GetModPlayer<HellKnightPlayer>().Active = true;

            Lighting.AddLight(player.Center, Color.Orange.ToVector3());
        }

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration<DTTrueMeleeClass>() += 20;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<HellArmorScrap>(3)
               .AddIngredient(ItemID.Ectoplasm, 6)
               .AddTile(TileID.MythrilAnvil)
               .Register();
        }
    }

    public class HellKnightPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (Active)
            {
                scale += 0.35f;
            }
        }
    }
}
