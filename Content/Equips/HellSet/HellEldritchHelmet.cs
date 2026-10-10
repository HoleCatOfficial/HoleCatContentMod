using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.HellSet
{
    [AutoloadEquip(EquipType.Head)]
    public class HellEldritchHelmet : ModItem
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
            Item.defense = 22;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HellPlatemail>() && legs.type == ModContent.ItemType<HellBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(Item);
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetModPlayer<HellEldritchPlayer>().Active = true;

            Lighting.AddLight(player.Center, Color.Orange.ToVector3());
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.03f;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<HellArmorScrap>(5)
               .AddIngredient(ItemID.Ectoplasm, 7)
               .AddTile(TileID.MythrilAnvil)
               .Register();
        }
    }

    public class HellEldritchPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (Active && Main.rand.NextBool(10))
            {
                Projectile.NewProjectile(Player.GetSource_OnHit(victim), new Vector2(x, y), Vector2.Zero, ModContent.ProjectileType<HellExplosion>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(300), 2f, Player.whoAmI);
            }
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            mult = 0.9f;
        }
    }
}
