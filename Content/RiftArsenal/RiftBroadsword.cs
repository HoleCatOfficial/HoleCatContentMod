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
	public class RiftBroadsword : ModItem, IRechargeFunctionality
	{
		public override void SetDefaults() 
        {
			Item.width = 80;
			Item.height = 80;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 40;
			Item.knockBack = 12;
			Item.crit = 16;

			Item.value = Item.buyPrice(gold: 16);
			Item.rare = ModContent.RarityType<RiftRarity1>();
			Item.UseSound = SoundID.Item71;
		}

        public bool Energized
        {
            get
            {
                return Main.LocalPlayer.GetModPlayer<Recharge>().Energized;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
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
