using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Content.Entity;
using DestroyerTest.Common.Systems;

namespace DestroyerTest.Content.Magic.ScepterSubclass
{
	public class GhoulishScepter : ScepterItem
	{
		public override int Width => 62;
        public override int Height => 62;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 47;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<GhoulProjectile>();
            ThrowID = ModContent.ProjectileType<GhoulishScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/GhoulishScepter/Ghost", 6) with
            {
                PitchVariance = 1.0f
            };;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }

    public class GhoulScep_DROP_NPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {

			if (npc.type == ModContent.NPCType<PossessedScepter>() && DownedBossSystem.downedPlanteraBoss) {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GhoulishScepter>(), 5, 1, 1));
			}
		}
	}
} 