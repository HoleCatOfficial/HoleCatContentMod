using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.DiscordScepter.Power;
using OpusLib.Content.Particles;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Graphics.Particles;

namespace DestroyerTest.Content.Scepter
{
	public class CelestialDiscord : ScepterItem
	{
		public override int Width => 52;
        public override int Height => 52;

		public int Charge = 0;
		public int ChargeIncrementInterval = 0;
		public bool CanCharge = true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			base.SetDefaults();

			Charge = 0;
			ChargeIncrementInterval = 0;

			ShootDMG = 180;
			ShootCrit = 10;
			ThrowCrit = 18;
			KB = 15;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<IncarnadineRarity>();

			ShootID = ModContent.ProjectileType<SolarDart>();
			ThrowID = ModContent.ProjectileType<CelestialDiscordThrown>();

			ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.4f, MaxInstances = 0};
			ThrowSound = new SoundStyle("DestroyerTest/Assets/Audio/HellWeaponDash", 3) with { PitchVariance = 0.4f, MaxInstances = 0};

			base.SetDefaults();
		}
		
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if (player.altFunctionUse == 2) // Throwing mode
            {
                type = ModContent.ProjectileType<CelestialDiscordThrown>();
            }
			if (player.altFunctionUse != 2)
			{
				if (CanCharge)
				{
					type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<VortexDart>(), ModContent.ProjectileType<StardustDartBig>(), ModContent.ProjectileType<NebulaFlameSpawner>() });
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Item82 with { Pitch = MathHelper.Lerp(0f, 0.5f, (float)Charge / 100f), Volume = 2f });
                    type = Main.rand.Next(new int[] { ModContent.ProjectileType<FusionBlastDart>(), ModContent.ProjectileType<FusionHighVelocityDart>(), ModContent.ProjectileType<FusionSplittingDart>(), ModContent.ProjectileType<FusionFlameSpawner>() });
					damage = (int)(damage * 1.8f);
                    //Main.NewText(Charge);
                    if (Charge > 0 && player.altFunctionUse != 2)
                    {
                        Charge--;
                    }
                }
			}
		}

		bool f1 = false;
        public override void UpdateInventory(Player player)
        {
			if (ChargeIncrementInterval > 0)
			{
				ChargeIncrementInterval--;
			}

			if (Charge >= 100)
			{
				if (!f1)
				{
					SoundEngine.PlaySound(DTAssetLib.Impacts.KCrystalConsume, player.Center);

                    BloomRingSharp Ring = new();
                    Ring.Prepare(player.MountedCenter, Vector2.Zero, ColorLib.CelestialGradient, 0.3f, 0.01f, 1f, BlendState.Additive);
                    ParticleEngine.Particles.Add(Ring);

                    for (int i = 0; i < 14; i++)
                    {
                        StarParticle star = new();
                        star.Initialize(player.MountedCenter, Main.rand.NextVector2Circular(2f, 2f), ColorLib.CelestialGradient, 1f);
                        ParticleEngine.Particles.Add(star);
                    }

					CanCharge = false;
                    f1 = true;
				}
			}
			else
			{

				if (Charge <= 0)
				{
					CanCharge = true;
				}
				f1 = false;
			}
            base.UpdateInventory(player);
        }

        public override bool? UseItem(Player player)
        {
			
            return base.UseItem(player);
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.LunarBar, 16)
				.AddIngredient(ItemID.FragmentVortex, 8)
				.AddIngredient(ItemID.FragmentStardust, 8)
				.AddIngredient(ItemID.FragmentSolar, 8)
				.AddIngredient(ItemID.FragmentNebula, 8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
} 