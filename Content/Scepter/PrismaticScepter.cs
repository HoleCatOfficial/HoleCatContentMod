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
using Terraria.GameContent.ItemDropRules;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
    public class PrismaticScepter : ScepterItem
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
            ShootDMG = 40;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ProjectileID.FairyQueenMagicItemShot;
            ThrowID = ModContent.ProjectileType<PrismaticScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }
    
    public class PS_DROP_NPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {

			if (npc.type == NPCID.HallowBoss) {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrismaticScepter>(), 5, 1, 1));
			}
		}
	}
} 