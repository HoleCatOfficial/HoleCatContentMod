
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class TheBotanistsCurse : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.maxStack = 20;
            Item.value = 1000;
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                return !NPC.AnyNPCs(ModContent.NPCType<NightmareRoseBoss>()) && player.ZoneCorrupt;
            }
            else
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<NightmareRoseArenaDisplay>()] < 1;
            }
        }

        public override bool CanShoot(Player player)
        {
            return player.altFunctionUse == 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    Main.MouseWorld,
                    Vector2.Zero,
                    ModContent.ProjectileType<NightmareRoseArenaDisplay>(),
                    0,
                    0,
                    player.whoAmI);

                return true;
            }

            if (player.whoAmI == Main.myPlayer && player.altFunctionUse != 2)
            {
                SoundStyle Summon = new SoundStyle("DestroyerTest/Assets/Audio/NightmareRose/SoulSummon");
                SoundEngine.PlaySound(Summon, player.position);

                int type = ModContent.NPCType<NightmareRoseBoss>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(player.GetSource_ItemUse(Item), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, type, 0);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(3)
                .AddIngredient(ItemID.DemoniteOre, 8)
                .AddIngredient(ItemID.CorruptSeeds, 1)
                .AddIngredient(ItemID.Book)
				.Register();
		}

    }
    
    
    
}