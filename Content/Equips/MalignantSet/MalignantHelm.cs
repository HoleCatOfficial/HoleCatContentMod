using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using GlowmaskHelper.Content;

namespace DestroyerTest.Content.Equips.MalignantSet
{
    [AutoloadEquip(EquipType.Head)]
    [AutoloadGlowmask]
    public class MalignantHelm : ModItem
    {
        public override void Load()
        {
            GlowmaskLoader.QueueGlowmaskRegistration($"{Texture}_Head_Glow");
        }
        public int ParticleSpawnTimer = 0;


        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
            GlowmaskLoader.AssignGlowmaskTexture_Equip(Item.glowMask, EquipType.Head, EquipLoader.GetEquipSlot(Mod, "MalignantHelm_Head", EquipType.Head));
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.defense = 12;
        }


       
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MalignantBodyArmor>() && legs.type == ModContent.ItemType<MalignantLegPlates>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.08f;
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetDamage(DamageClass.Summon) += 0.16f;
            player.GetDamage(DamageClass.Throwing) += 0.1f;
            player.GetDamage<ScepterClass>() += 0.14f;

            if (DTCrossMod.CalamityIsLoaded)
            {
                DTCrossMod.CalamityMod.Call("AddMaxStealth", player, 60f);

                DTCrossMod.CalamityMod.Call("SetWearingRogueArmor", player, true);
            }

            player.GetModPlayer<MalignantPlayer>().Active = true;
            player.DefaultSetBonusText(player.armor[0]);
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WretchedShards>(12)
                .AddIngredient(ItemID.SpectreBar, 4)
                .AddIngredient(ItemID.EbonstoneBlock, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class MalignantPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (Active)
            {
                if (item.DamageType == DamageClass.Melee)
                {
                    return 1.1f;
                }
                if (item.DamageType == DamageClass.Ranged)
                {
                    return 1.05f;
                }
                if (item.DamageType == DamageClass.Magic)
                {
                    return 1.2f;
                }
            }
            return 1f;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Active)
            {
                if (item.DamageType == DamageClass.Magic)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1f), ModContent.ProjectileType<InfectedCrystalCF>(), damage / 2, 8, Player.whoAmI);
                    }
                }
                if (item.DamageType == DamageClass.Ranged)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.05f), ModContent.ProjectileType<CursedNodeCrystalFriendly>(), damage / 2, 8, Player.whoAmI);
                    }
                }
            }
            return true;
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (Active)
            {
                mult = 0.85f;
            }
        }
        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (Active)
            {
                if (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.MeleeNoSpeed)
                {
                    scale = 1.425f;
                }
            }
        }
    }
}