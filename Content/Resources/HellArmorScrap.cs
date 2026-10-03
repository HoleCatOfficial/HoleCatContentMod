using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
    public class HellArmorScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 4;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.value = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Lighting.AddLight(Item.Center, Color.OrangeRed.ToVector3() * 0.7f);

            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.Torch, Main.rand.NextFloat(-0.1f, 0.1f), -17f, 100, default(Color), 2f);
                dust.noGravity = true;
            }
        }
    }

    public class HellArmorScrapDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (OpusNPCDropHelper.MoltenLegionEnemiesExclusive.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HellArmorScrap>(), 1, 3, 5));
            }
        }
    }
}