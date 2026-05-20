using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
    public class CursedHammer : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BlossomBeater>();
        }
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6; 
            Item.autoReuse = true;
            Item.damage = 140;
            Item.DamageType = ModContent.GetInstance<DTRogueClass>();
            Item.crit = 10;
            Item.shoot = ModContent.ProjectileType<CursedHammerThrown>();
            Item.shootSpeed = 40f;
            Item.noUseGraphic = true;
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse == 2) // Throwing mode
            {
                float animationSpeed = 8.0f; // You can modify this to change the animation speed.

                // Calculate the progress, but limit it to a max of 1.0
                float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
                progress = Math.Min(progress * animationSpeed, 1.0f); // Clamps progress to a max of 1

                // Start angle at 180 degrees (upwards)
                float startAngle = MathHelper.ToRadians(180f);

                // Declare endAngle here to ensure it's accessible outside of the if blocks
                float endAngle;

                // Set the end angle based on player direction
                if (player.direction == 1)
                {
                    endAngle = MathHelper.ToRadians(270f); // Right side, end angle 270
                }
                else if (player.direction == -1)
                {
                    endAngle = MathHelper.ToRadians(90f); // Left side, end angle 90
                }
                else
                {
                    endAngle = startAngle; // Default case (shouldn't happen unless player.direction is unexpected)
                }

                // Interpolate between start and end angle
                float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);

                // If the progress has reached the end, stop the arm from rotating further
                if (progress == 1.0f)
                {
                    // Ensure the arm stays at the final angle and doesn't continue animating
                    armRotation = endAngle;
                }

                // Apply the final rotation to the player's arm
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            }
        }
    }
}