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
using InnoVault.PRT;
using DestroyerTest.Common;
using Terraria.GameContent.Creative;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Projectiles;
using InnoVault;
using DestroyerTest.Content.Equips.AuraThiefSet;
using DestroyerTest.Content.Projectiles.player.ArmorSet;

namespace DestroyerTest.Content.Equips.SpectralThiefSet
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class SpectralThiefHeadgear : ModItem
    {
        public int ParticleSpawnTimer = 0;
        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            //ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            //ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

        }

        public override void SetDefaults()
        {
            Item.width = 36; // Width of the item
            Item.height = 28; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<LifeEchoRarity>(); // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SpectralThiefBreastplate>() && legs.type == ModContent.ItemType<SpectralThiefCuisses>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<SpectralThiefScepter>(out SpectralThiefScepter Scptr))
                {
                    Scptr.Active = true;
                }
            }

            if (player.TryGetModPlayer<SpectralThief>(out SpectralThief Th))
            {
                Th.Active = true;
            }
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ectoplasm, 16)
                .AddIngredient<AuraThiefHeadgear>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SpectralThief : ModPlayer
    {
        public bool Active = false;
        public bool MoonlordBoost = ModLoader.HasMod("CalamityMod") && DownedBossSystem.downedLunarBoss;

        public override void ResetEffects()
        {
            Active = false;
        }
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (!MoonlordBoost)
                {
                    Player.GetDamage(DamageClass.Melee) *= 1.18f;
                    Player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.18f;
                }
                if (MoonlordBoost)
                {
                    Player.GetDamage(DamageClass.Melee) *= 1.30f;
                    Player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.30f;
                }
            }
        }
    }
    
    public class SpectralThiefScepter : GlobalProjectile
    {
        public bool Active = false;
        public bool MoonlordBoost = ModLoader.HasMod("CalamityMod") && DownedBossSystem.downedLunarBoss;
        public override bool InstancePerEntity => true;
        public bool IsAThrownScepter = false;
        public bool IsScepterClassButNotThrown = false;

        public override void SetDefaults(Projectile entity)
        {
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && entity.Name.Contains("Thrown"))
            {
                IsAThrownScepter = true;
            }
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && !entity.Name.Contains("Thrown"))
            {
                IsScepterClassButNotThrown = true;
            }
        }

        public override void AI(Projectile projectile)
        {
            base.AI(projectile);

            if (Active && IsAThrownScepter && !MoonlordBoost)
            {
                if (Main.GameUpdateCount % 20 == 0)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, projectile.velocity * 0.001f, ModContent.ProjectileType<PhantomScepter>(), projectile.damage / 3, 3, projectile.owner, ai2: 1);
                }
            }
            if (Active && IsAThrownScepter && MoonlordBoost)
            {
                if (Main.GameUpdateCount % 20 == 0)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, projectile.velocity * 0.5f, ModContent.ProjectileType<PhantomScepter2>(), projectile.damage / 3, 3, projectile.owner, ai2: 1);
                }
            }
        }
    }
}