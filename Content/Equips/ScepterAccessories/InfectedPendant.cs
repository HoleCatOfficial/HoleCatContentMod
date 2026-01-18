
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
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
    public class InfectedPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public static List<int> ItemsThatInfectedPendantCannotPairWith = new List<int>
        {
            ModContent.ItemType<DetritizedPendant>(),
            ModContent.ItemType<VisceraPendant>(),
            ModContent.ItemType<PendantofUnity>(),
            ModContent.ItemType<ElementalPendant>()
        };

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !ItemsThatInfectedPendantCannotPairWith.Contains(incomingItem.type);
        }

        public static readonly float CritBonus = 1.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBonus.ToString("F1") + "%");

        public override void UpdateAccessory(Player player, bool hideVisual)
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

            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += CritBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VisceraPendant>()
                .AddIngredient<DetritizedPendant>()
                .AddIngredient<WretchedShards>(8)
                .AddIngredient<PrimalShards>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class InfectedPendantScepterUsePlayer : ModPlayer
	{
		public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }
		public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Active)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 1)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 2; t++)
                        {
                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(0.3f),
                                ModContent.ProjectileType<IchorNodeCrystalFriendly>(),
                                16,
                                knockback,
                                Player.whoAmI
                            );

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                velocity.RotatedByRandom(0.3f),
                                ModContent.ProjectileType<CursedNodeCrystalFriendly>(),
                                16,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }
        }
	}
}