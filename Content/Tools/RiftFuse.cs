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
using DestroyerTest.Content.Tiles.RiftConfigurator;


namespace DestroyerTest.Content.Tools
{
		
		
	public class RiftFuse : ModItem
	{
		public override void SetDefaults() {
			Item.width = 80; // The item texture's width.
			Item.height = 80; // The item texture's height.
			Item.value = Item.buyPrice(gold: 16); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<RiftRarity1>();
		}

		public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<ShadowCircuitry>(24)
                .AddIngredient<Item_Riftplate>(20)
                .AddIngredient<RiftData>(3)
                .AddTile<Tile_RiftConfiguratorTools>()
				.Register();
		}

        public override void UpdateInventory(Player player)
        {
            NoChargeLeft(player);
        }

		public override bool CanRightClick() {
                return true;
            }

        private const int CrucibleProximityRange = 3;


        public override void RightClick(Player player)
        {
            SoundStyle zapSound = new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") with { Volume = 0.7f, PitchVariance = 0.2f };

            if (player.HeldItem != null && player.HeldItem.type == ModContent.ItemType<RiftElectrifier>())
            {
                if (IsNearRiftCrucible(player))
                {
                    ReplenishLivingShadow(player, zapSound, consumeBatteries: false);
                }
                else if (player.CountItem(ModContent.ItemType<RiftBattery>(), 4) >= 4)
                {
                    ConsumeBatteries(player, 4);
                    ReplenishLivingShadow(player, zapSound, consumeBatteries: true);
                }
                else
                {
                    CombatText.NewText(player.Hitbox, ColorLib.Rift, $"4 Rift Batteries needed, or, plug the Electrifier into a rift crucible.", true);
                }
            }
            else
			{
				return;
			}
        }

        public override bool ConsumeItem(Player player)
        {
            return false; // Prevents the item from being consumed on use
        }

        private bool IsNearRiftCrucible(Player player)
        {
            Point playerTilePosition = player.Center.ToTileCoordinates();
            int riftCrucibleTileType = ModContent.TileType<Tile_RiftCrucible>();

            for (int x = -CrucibleProximityRange; x <= CrucibleProximityRange; x++)
            {
                for (int y = -CrucibleProximityRange; y <= CrucibleProximityRange; y++)
                {
                    Point checkPosition = new Point(playerTilePosition.X + x, playerTilePosition.Y + y);
                    if (Main.tile[checkPosition.X, checkPosition.Y].TileType == riftCrucibleTileType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ConsumeBatteries(Player player, int count)
        {
            int riftBatteryType = ModContent.ItemType<RiftBattery>();
            for (int i = 0; i < count; i++)
            {
                player.ConsumeItem(riftBatteryType);
            }
        }

        private void ReplenishLivingShadow(Player player, SoundStyle zapSound, bool consumeBatteries)
        {
            SoundEngine.PlaySound(zapSound, player.position);
            ScreenFlashSystem.FlashIntensity = 0.9f;

            var modPlayer = player.GetModPlayer<LivingShadowPlayer>();
            modPlayer.LivingShadowCurrent = modPlayer.LivingShadowMax2;
        }

        private bool Sound = false;
        private void NoChargeLeft(Player player)
        {
            var modPlayer = player.GetModPlayer<LivingShadowPlayer>();
            if (modPlayer.LivingShadowCurrent == 0 && Sound == false)
			{
            Sound = true;
            
            }
        }
	}
}
