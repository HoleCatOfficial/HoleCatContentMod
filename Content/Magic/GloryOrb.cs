using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using System.Linq;
using UtfUnknown.Core.Models.SingleByte.Italian;
using DestroyerTest.Content.Projectiles.Weapon.Magic;

namespace DestroyerTest.Content.Magic
{
    public class GloryOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
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
            Item.damage = 550;
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HekatesMystique>()
                .AddIngredient(ItemID.MagnetSphere)
                .AddIngredient(ItemID.Ectoplasm, 6)
                .AddIngredient(ItemID.PixieDust, 6)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}