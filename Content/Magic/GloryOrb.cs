using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Italian;

namespace DestroyerTest.Content.Magic
{
    public class GloryOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Purity>();
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.value = Item.sellPrice(gold: 25, silver: 70);
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = true;
            Item.damage = 700;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.mana = 0;
            Item.crit = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;

            Item.shoot = ModContent.ProjectileType<GloryOrbHoldout>();
            Item.shootSpeed = 1;
        }

        public override float UseTimeMultiplier(Player player)
        {
            return 0.1f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                return true;
            }
            else
            {
                return false;
            }    
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}