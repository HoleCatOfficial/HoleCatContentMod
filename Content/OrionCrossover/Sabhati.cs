using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.OrionCrossover;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.OrionCrossover
{
    public class Sabhati : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 160;
            Item.height = 160;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<StellarRarity>();
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.autoReuse = false;
            Item.damage = 28;
            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SabhatiSwing>();
            Item.channel = true;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (DestroyerTestMod.EternityIsActive)
            {
                damage += 1.15f;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string v = Language.GetTextValue("Mods.DestroyerTest.Items.Sabhati.EternityTooltip");
            TooltipLine L = new TooltipLine(Mod, "EternityDamageMod", v);
            L.OverrideColor = ColorLib.StellarFireGradientLooping();

            if (DestroyerTestMod.EternityIsActive)
            {
                tooltips.Add(L);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                
                .AddIngredient<Constitution>(1)
                .AddIngredient(ItemID.Starfury, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}