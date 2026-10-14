
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.MeleeWeapons;

using Terraria.Localization;
using DestroyerTest.Content.Entities;
using System.Linq;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Common.Systems;

namespace DestroyerTest.Content.SummonItems
{
    public class Malachite  : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
            DTUtils.LegendaryWeapon[Type] = true;
        }

        public static int Level = 0;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.knockBack = 9f;
            Item.mana = 10;
            Item.width = 84;
            Item.height = 84;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 180;
            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<MalachiteBuff>();
            Item.shoot = ModContent.ProjectileType<MalachiteProjectile>();
        }



        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            switch(Level)
            {
                case 0:
                    {
                        damage += 0.0f;
                        break;
                    }
                    //Skeletron
                case 1:
                    {
                        damage += 0.2f;
                        break;
                    }
                    //WoF
                case 2:
                    {
                        damage += 0.8f;
                        break;
                    }
                    //Plantera
                case 3:
                    {
                        damage += 1.2f;
                        break;
                    }
                    //Cultist
                case 4:
                    {
                        damage += 1.8f;
                        break;
                    }
            }
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<MalachiteOrganizer>().Level = Level;
            if (DownedBossSystem.downedSkeletronBoss)
            {
                Level = 1;
            }
            if (DownedBossSystem.downedWallBoss)
            {
                Level = 2;
            }
            if (DownedBossSystem.downedPlanteraBoss)
            {
                Level = 3;
            }
            if (DownedBossSystem.downedCultistBoss)
            {
                Level = 4;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string Key = "Mods.DestroyerTest.Items.Malachite";
            TooltipLine Level1Tooltip = new TooltipLine(Mod, "Level1Tooltip", Language.GetTextValue(Key + ".Level1Tooltip")) { OverrideColor = Level >= 1 ? Color.Goldenrod : Color.Gray};
            TooltipLine Level2Tooltip = new TooltipLine(Mod, "Level2Tooltip", Language.GetTextValue(Key + ".Level2Tooltip")) { OverrideColor = Level >= 2 ? Color.Goldenrod : Color.Gray };
            TooltipLine Level3Tooltip = new TooltipLine(Mod, "Level3Tooltip", Language.GetTextValue(Key + ".Level3Tooltip")) { OverrideColor = Level >= 3 ? Color.Goldenrod : Color.Gray };
            TooltipLine Level4Tooltip = new TooltipLine(Mod, "Level4Tooltip", Language.GetTextValue(Key + ".Level4Tooltip")) { OverrideColor = Level >= 4 ? Color.Goldenrod : Color.Gray };



            tooltips.Add(Level1Tooltip);
            tooltips.Add(Level2Tooltip);
            tooltips.Add(Level3Tooltip);
            tooltips.Add(Level4Tooltip);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}
