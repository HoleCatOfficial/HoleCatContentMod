using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
 
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Numerics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.AuraThiefSet
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Legs)]
	public class AuraThiefCuisses : ModItem
	{
		public override void SetDefaults() {
			Item.width = 22; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<LifeEchoRarity>(); // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) 
		{
            float h = player.height - 2;
            Rectangle below = new Rectangle((int)player.position.X, (int)(player.position.Y + h), player.width, 2);
            if (Math.Abs(player.velocity.X) > 3.75f)
			{
				//Dust.NewDustDirect(player.Bottom, 2, 1, ModContent.DustType<SoulDust>(), 0, 0.02f, 100, new Microsoft.Xna.Framework.Color(184, 228, 242), 1);

				PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
				Glow.Initialize(Main.rand.NextVector2FromRectangle(below), Main.rand.NextVector2Circular(3, 3), new Color(184, 228, 242), 0.5f);
				ParticleEngine.ShaderParticles.Add(Glow);
			}
        }


		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<LifeEcho>(15)
                .AddIngredient<BlackCloth>(15)
                .AddIngredient(ItemID.Wood, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

    public class AuraThiefCuissPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
               
            }
        }

        public override void PostUpdateRunSpeeds()
        {
			if (Active)
			{
				Player.maxRunSpeed *= 1.35f;
			}
        }
    }
}