using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
    public class FleshVolley : ModItem
    {

        public override void SetStaticDefaults()
        {
            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RibChainsaw>();
            }
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 56;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item69;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.damage = 200;
            Item.mana = 80;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<FleshVolleyFleshBomb>();
            Item.shootSpeed = 9f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectile(source, position, new Vector2(Main.rand.NextFloat(0.4f, 1.1f) * Item.shootSpeed * player.direction, Main.rand.NextFloat(-1.1f, -0.7f) * Item.shootSpeed), type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.WGIIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<WyvernCorpseArtifact>(3)
                    .Register();
            }
        }
    }
}
