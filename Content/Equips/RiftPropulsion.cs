
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Wings)]
	public class RiftPropulsion : ModItem
	{

		public override void SetStaticDefaults() {
			// These wings use the same values as the solar wings
			// Fly time: 180 ticks = 3 seconds
			// Fly speed: 9
			// Acceleration multiplier: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(150, 5f, 1.5f, false);
            
		}

		public override void SetDefaults() {
			Item.width = 60;
			Item.height = 26;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<RiftRarity1>(); // The rarity of the item
			Item.accessory = true;
            Item.UseSound = SoundID.Item24;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 0.85f; // Falling glide speed
			ascentWhenRising = 0.15f; // Rising speed
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

        

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(16)
				.AddIngredient<Item_Riftplate>(8)
                .AddIngredient(ItemID.Jetpack)
                .AddTile<Tile_RiftConfigurator>()
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
				.Register();
		}
	}

    public class RiftPropulsionDash : ModPlayer
    {
        bool HasRiftPropulsionEquipped()
        {
            for (int i = 3; i < 8; i++) // Accessory slots (vanilla)
            {
                if (Player.armor[i].type == ModContent.ItemType<RiftPropulsion>())
                    return true;
            }
            return false;
        }
        // These indicate what direction is what in the timer arrays used
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int DashCooldown = 60; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 40; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public float DashVelocity = 20f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public bool DashAccessoryEquipped;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 6; // frames remaining in the dash

        public override void ResetEffects()
        {
            DashAccessoryEquipped = true;

            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15 && HasRiftPropulsionEquipped())
            {
                DashDir = DashDown;
            }
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15 && HasRiftPropulsionEquipped())
            {
                DashDir = DashUp;
            }
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && Player.doubleTapCardinalTimer[DashLeft] == 0 && HasRiftPropulsionEquipped())
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && Player.doubleTapCardinalTimer[DashRight] == 0 && HasRiftPropulsionEquipped())
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
        }

        public override void PreUpdateMovement()
        {
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashUp when Player.velocity.Y > -DashVelocity:
                    case DashDown when Player.velocity.Y < DashVelocity:
                        {
                            float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * DashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                Dust.NewDust(newVelocity, 15, 15, ModContent.DustType<ColorableNeonDust>(), 6f, 0f, 0, ColorLib.Rift, 10f);
                SoundEngine.PlaySound(new SoundStyle($"DestroyerTest/Assets/Audio/RiftDash"));

            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            {
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;

                DashTimer--;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == DashID.None
                && HasRiftPropulsionEquipped()
                && !Player.setSolar
                && !Player.mount.Active;
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return DashTimer <= 0;
        }
    }
}