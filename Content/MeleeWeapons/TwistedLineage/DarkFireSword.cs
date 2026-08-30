using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;  
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using System;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Terraria.GameContent.ItemDropRules;

namespace DestroyerTest.Content.MeleeWeapons.TwistedLineage
{
	public class DarkFireSword : ModItem
	{
        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1.15f;
        }

        public override void SetDefaults()
		{
			Item.width = 96;
			Item.height = 96;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.White;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.knockBack = 5;
			Item.autoReuse = true;
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.crit = 16;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<DarkFireSwordSwing>();
			Item.channel = true;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

    }

	public class DFS_DropNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.GoblinWarrior)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarkFireSword>(), 10, 1, 1));
			}
        }
	}
} 