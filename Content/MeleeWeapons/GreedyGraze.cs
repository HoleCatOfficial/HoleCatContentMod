
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Finnish;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class GreedyGraze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.TooltipScaleMult[Type] = 1.6f;
            DTUtils.isSpecialSwingSword[Type] = true;
        }
        public override void SetDefaults()
        {

            Item.width = 88;
            Item.height = 92;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.autoReuse = true;
            Item.damage = 600;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.SetSpecialMeleeStats();

            Item.shoot = ModContent.ProjectileType<GreedyGrazeSwing>();
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