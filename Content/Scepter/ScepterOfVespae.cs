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
	public class ScepterOfVespae : ScepterItem
	{
		public override int Width => 56;
        public override int Height => 52;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 27;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ProjectileID.Bee;
            ThrowID = ModContent.ProjectileType<ScepterOfVespaeThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.altFunctionUse != 2)
            {
                // Fire the first projectile (SoulOfLight_Projectile)
                Projectile.NewProjectile(source, position, velocity, ProjectileID.Bee, damage, knockback, player.whoAmI);

                // Fire the second projectile (SoulOfNight_Projectile)
                Projectile.NewProjectile(source, position, velocity, ProjectileID.HornetStinger, damage, knockback, player.whoAmI);
            }

            return true;
		}


    }

	public class SoV_DROP_NPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) 
        {
			if (npc.type == NPCID.QueenBee) {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScepterOfVespae>(), 5, 1, 1));
			}
		}
	}
} 