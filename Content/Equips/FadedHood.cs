using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Head)]
    public class FadedHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FadedRobes>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ScepterClass>() += 0.15f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.DefaultSetBonusText(player.armor[0]);
            player.ScepterClass().ThrowSpeedModifier += 0.35f;
            if (player.TryGetModPlayer<CultScepterPlayer>(out CultScepterPlayer Scptr))
			{
				Scptr.Active = true;
			}
        }
    }
    
    public class CultScepterPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Active)
            {
                if (item.DamageType.CountsAsClass(ModContent.GetInstance<ScepterClass>()))
                {
                    if (Player.altFunctionUse == 2)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            Projectile.NewProjectile(Entity.GetSource_ItemUse(item), position, velocity.RotatedByRandom(13), ModContent.ProjectileType<FakeAncientLight>(), damage / 2, knockback / 2, Player.whoAmI);
                        }
                    }
                }
            }
            base.ModifyShootStats(item, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
    }
}