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
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class FadedHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            //ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
                                                                  //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
                                                                  // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26; // Width of the item
            Item.height = 24; // Height of the item
            Item.value = Item.sellPrice(gold: 8); // How many coins the item is worth
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>(); // The rarity of the item
            Item.defense = 6; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FadedRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
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

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                for (int y = 0; y < 4; y++)
                {
                    Vector2 ofst = new Vector2(100, 0);
                    Vector2 velocity = Player.Center + ofst.RotatedByRandom(MathHelper.Pi);
                    Projectile.NewProjectile(Entity.GetSource_OnHurt(hurtInfo.DamageSource), Player.Center, velocity, ModContent.ProjectileType<FakeAncientLight>(), 16, 2, Player.whoAmI);
                }
            }
            base.OnHitByNPC(npc, hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                for (int y = 0; y < 4; y++)
                {
                    Vector2 ofst = new Vector2(100, 0);
                    Vector2 velocity = Player.Center + ofst.RotatedByRandom(MathHelper.Pi);
                    Projectile.NewProjectile(Entity.GetSource_OnHurt(hurtInfo.DamageSource), Player.Center, velocity, ModContent.ProjectileType<FakeAncientLight>(), 16, 2, Player.whoAmI);
                }
            }
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Active)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>())
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