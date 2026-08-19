
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class ElementalPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemsThatElementalPendantCannotPairWith.AddRange(InfectedPendant.ItemsThatInfectedPendantCannotPairWith);
            ItemsThatElementalPendantCannotPairWith.AddRange(PendantofUnity.ItemsThatPendantofUnityCannotPairWith);
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public static List<int> ItemsThatElementalPendantCannotPairWith = new List<int>
        {
            ModContent.ItemType<InfectedPendant>(),
            ModContent.ItemType<PendantofUnity>(),
        };

        public float DMGBonus = 0.375f;
        public static readonly float CritBonus = 1.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"), CritBonus.ToString("F1") + "%");

        public void ActivatePlayer(Player player)
        {
            if (player.TryGetModPlayer<CrimsonPendantScepterUsePlayer>(out CrimsonPendantScepterUsePlayer Crim))
			{
				Crim.Active = true;
			}
            if (player.TryGetModPlayer<CorruptPendantScepterUsePlayer>(out CorruptPendantScepterUsePlayer Ebon))
			{
				Ebon.Active = true;
			}
            if (player.TryGetModPlayer<InfectedPendantScepterUsePlayer>(out InfectedPendantScepterUsePlayer Infected))
			{
				Infected.Active = true;
			}
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActivatePlayer(player);
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += DMGBonus;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += CritBonus;

            player.GetArmorPenetration<ScepterClass>() += 5f;
            if (Main.expertMode)
            {
                player.AddBuff(ModContent.BuffType<WeaponImbueFF>(), 60);
                player.AddBuff(BuffID.WeaponImbueFire, 60);
                player.AddBuff(BuffID.WeaponImbueCursedFlames, 60);
                player.AddBuff(BuffID.WeaponImbueIchor, 60);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<InfectedPendant>()
                .AddIngredient<PendantofUnity>()
                .AddIngredient<FrigidPendant>()
                .AddIngredient<SmolderingPendant>()
                .AddIngredient<HelicitePendant>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}