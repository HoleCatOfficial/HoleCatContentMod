using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    
	public class SoulEdge : ModItem
	{
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults() {

            Item.UseSound = SoundID.Item71;
			Item.width = 40; // The item texture's width.
			Item.height = 40; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 15; // The damage your item deals.
			Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 6; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<LifeEchoRarity>(); // Give this item our custom rarity.
			Item.shoot = ModContent.ProjectileType<SoulEdgeProjectile>();
            Item.shootSpeed = 0.02f;
		}

        

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }


		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) 
			{
                //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(hitbox), Main.rand.NextVector2Circular(3, 3), new Color(184, 228, 242), 0.5f);
            }
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient(ItemID.WoodenSword, 1)
                .AddIngredient(ItemID.FlinxFur, 2)
                .AddIngredient<LifeEcho>(20)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}