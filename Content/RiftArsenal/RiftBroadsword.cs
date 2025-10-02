using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.Tools;

using System.Collections.Generic;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources.Blueprints;
using DestroyerTest.Content.Tiles.RiftConfigurator;



namespace DestroyerTest.Content.RiftArsenal
{
	public class RiftBroadsword : RechargeItem
	{
		public override void SetDefaults() {
			Item.width = 80; // The item texture's width.
			Item.height = 80; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 40; // The damage your item deals.
			Item.knockBack = 12; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 16; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.UseSound = SoundID.Item71; // The sound when the weapon is being used.
		}

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.RiftDust>());
            }

            if (Energized)
            {
                int[] types = new int[]
                {
                    PRTLoader.GetParticleID<Arc1>(),
                    PRTLoader.GetParticleID<Arc2>(),
                    PRTLoader.GetParticleID<Arc3>()
                };

				if (Main.rand.NextBool(3))
				{
					PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Main.rand.NextVector2FromRectangle(hitbox), Vector2.Zero, ColorLib.Rift, 0.3f);
				}
            }
		}

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Energized)
            {
                target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
            }
        }
	
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BroadswordData>()
                .AddIngredient<ShadowCircuitry>(2)
                .AddIngredient<Item_Riftplate>(12)
                .AddTile<Tile_RiftConfiguratorWeaponry>()
			.Register();
        }
	}
}
