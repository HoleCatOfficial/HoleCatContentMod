using DestroyerTest;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.RangedItems;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class Purity : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PureBow>();
        }
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 94;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 180;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<HallowedSpecialRarity>();
            Item.shoot = ModContent.ProjectileType<PuritySwing>();
            Item.noUseGraphic = true;
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