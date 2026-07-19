using System.Security.Cryptography.X509Certificates;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class MechanicalEnhancements : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.useTurn = true;
            Item.useAnimation = 120;
            Item.useTime = 120;
            Item.consumable = true;
            Item.width = 54;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 8, 50);
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.maxStack = 1;
        }

        public override bool ConsumeItem(Player player)
        {
            if (player.TryGetModPlayer<MechanicalEnhancementsPlayer>(out var Enchancement))
            {
                Enchancement.EnhancedJorkingMethods = true;
            }
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer<MechanicalEnhancementsPlayer>(out var Enchancement))
            {
                return !Enchancement.EnhancedJorkingMethods;
            }
            else
            {
                return false;
            }
        }
    }

    public class MechanicalEnhancementsPlayer : ModPlayer
    {
        public bool EnhancedJorkingMethods = false;
        public bool Effects = false;
        public override void ResetEffects()
        {
            Effects = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (EnhancedJorkingMethods)
            {
                Effects = true;
            }

            if (Effects)
            {
                Player.ScepterClass().ThrowSpeedModifier += 1.4f;
            }
        }
    }
}