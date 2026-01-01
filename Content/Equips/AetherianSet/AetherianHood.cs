using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Equips.AetherianSet
{
    [AutoloadEquip(EquipType.Head)]
    public class AetherianHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 8); // How many coins the item is worth
            Item.rare = ModContent.RarityType<PearlRarity>(); // The rarity of the item
            Item.defense = 2; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AetherianRegalia>();
        }

        public override void UpdateArmorSet(Player player)
        {
            if (player.TryGetModPlayer<AetherianScepterPlayer>(out AetherianScepterPlayer Scptr))
			{
				Scptr.Active = true;
			}
            player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.AetherianHood.SetBonus");
        }

        public static readonly float DMGBonus = 1.1f;
        public static readonly int PenetrationBonus = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"), PenetrationBonus);

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= DMGBonus;
            player.GetArmorPenetration(ModContent.GetInstance<ScepterClass>()) += PenetrationBonus;
        }

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.ShimmerBlock, 9)
				.AddTile(TileID.Anvils)
				.Register();
        }
    }
    
    public class AetherianScepterPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                foreach (Projectile Scepter in Main.projectile)
                {
                    if (Scepter.active && Scepter.owner == Player.whoAmI && Scepter.ModProjectile is ThrownScepter thrownScepter)
                    {
                        thrownScepter.ArmorSetHelper_AetherianShimmerEffects = true;
                    }
                }
            }
        }
    }
}