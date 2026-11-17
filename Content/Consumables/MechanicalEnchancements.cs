using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Consumables
{
    public class MechanicalEnhancements : OneTimeConsumable
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override bool Consumed()
        {
            if (Main.LocalPlayer.whoAmI == Main.myPlayer)
            {
                if (Main.LocalPlayer.TryGetModPlayer<MechanicalEnhancementsPlayer>(out var Enchancement))
                {
                    return Enchancement.EnhancedJorkingMethods = true;
                }
            }
            return false;
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

        public override void OnConsume(Player player)
        {
            if (player.TryGetModPlayer<MechanicalEnhancementsPlayer>(out var Enchancement))
            {
                Enchancement.EnhancedJorkingMethods = true;
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

        public override void SaveData(TagCompound tag)
        {
            tag.Add("ME_consumed", EnhancedJorkingMethods);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("ME_consumed"))
            {
                EnhancedJorkingMethods = tag.GetBool("ME_consumed");
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (EnhancedJorkingMethods)
            {
                Player.ScepterClass().ThrowSpeedModifier += 0.4f;
            }

            if (Effects)
            {

            }
        }
    }
}