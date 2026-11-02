using DestroyerTest.Common;
using DestroyerTest.Common.DropRules;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.AncientSword
{
    [AutoloadGlowmask]
    public class LostWonder : ModItem
    {
        public override void SetStaticDefaults()
        {
            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.WallofFlesh, ItemDropRule.ByCondition(new ExpertOnlyDropRuleCondition(), Type, 5));
            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 300;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.shoot = ModContent.ProjectileType<LostWonderSwing>();
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.WGIIsLoaded)
            {
                if (DTCrossMod.WGIMod.TryFind("WallOfFleshArtifact", out ModItem Artifact))
                {
                    CreateRecipe()
                        .AddIngredient(Artifact, 4)
                        .Register();
                }
            }
        }
    }
}