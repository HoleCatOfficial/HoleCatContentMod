
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
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.RiftBiome.RiftDesertResources;
using Microsoft.Xna.Framework;
using OpusLib;
using System.Linq;
using DestroyerTest.Content.Projectiles.player.Accessory;

namespace DestroyerTest.Content.Equips
{
	public class RiftCanister : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 132;
			Item.height = 94;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ItemRarityID.Red;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            if(player.TryGetModPlayer<RiftCanisterPlayer>(out var rift))
            {
                rift.Active = true;
            }
		}
    }

    public class RCDROPNPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) 
        {
			if (DTUtils.RiftSurfaceEnemies.Contains(npc.type)) 
            {
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RiftCanister>(), 20, 1, 1));
			}

		}
	}

    public class RiftCanisterPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }
        public Projectile[] orbs = new Projectile[6];

        public override void PostUpdateMiscEffects()
        {
            if (!Active)
            {
                return;
            }

            // Clear dead refs
            for (int i = 0; i < orbs.Length; i++)
            {
                if (orbs[i] != null && !orbs[i].active)
                {
                    orbs[i] = null;
                }
            }

            Vector2[] orbits = Opus.GetEquidistantOrbitVectors(
                orbs.Length,
                Player.Center,
                0.1f,
                Opus.Sine(200f, 210f)
            );

            for (int i = 0; i < orbs.Length; i++)
            {
                if (Main.GameUpdateCount % 300 != 0)
                {
                    continue;
                }

                for (int ih = 0; ih < orbs.Length; ih++)
                {
                    if (orbs[ih] == null)
                    {
                        Projectile neworb = Projectile.NewProjectileDirect(
                            Player.GetSource_None(),
                            orbits[ih],
                            Vector2.Zero,
                            ModContent.ProjectileType<RiftOrb>(),
                            40,
                            10,
                            Player.whoAmI
                        );

                        orbs[ih] = neworb;
                        break; // only spawn ONE per tick
                    }
                }

            }

            // Rebuild slots
            foreach (Projectile p in Main.projectile)
            {
                if (!p.active)
                    continue;

                if (p.type != ModContent.ProjectileType<RiftOrb>())
                    continue;

                if (p.owner != Player.whoAmI)
                    continue;

                if (orbs.Contains(p))
                    continue;

                for (int i = 0; i < orbs.Length; i++)
                {
                    if (orbs[i] == null)
                    {
                        orbs[i] = p;
                        break;
                    }
                }
            }

            

            for (int i = 0; i < orbs.Length; i++)
            {
                if (orbs[i] != null)
                {
                    orbs[i].Center = orbits[i];
                }
            }
        }
    }
}