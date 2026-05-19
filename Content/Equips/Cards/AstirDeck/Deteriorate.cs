
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
	public class Deteriorate : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 1;
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage<DTRogueClass>() += 0.3f;

            if (player.TryGetModPlayer<DeterioratePlayer>(out var deteriorate))
            {
                deteriorate.Active = true;
            }
        }
    }

    public class DeteriorateDropNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.CorruptSlime)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Deteriorate>(), 16, 1, 1));
            }

        }
    }

    public class  DeterioratePlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }
    }

    internal class DeteriorateOwnedProjectiles : GlobalProjectile
	{
        public override bool InstancePerEntity => true;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[projectile.owner];
            if (Owner.TryGetModPlayer<DeterioratePlayer>(out var deterioratePlayer))
            {
                if (deterioratePlayer.Active && Main.rand.NextBool() && projectile.DamageType == ModContent.GetInstance<DTRogueClass>())
                {
                    Projectile.NewProjectile(Projectile.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<DeteriorateBurst>(), projectile.damage / 2, 15, projectile.owner);
                }
            }
        }
    }
    
}