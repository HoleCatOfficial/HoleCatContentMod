using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class HoleCatHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem[Type] = true;
            DTUtils.isSpecialSwingSword[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 102;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.SetSpecialMeleeStats();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 70;
            Item.autoReuse = false;
            Item.damage = 550;
            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<HoleCatHookSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

    }
}