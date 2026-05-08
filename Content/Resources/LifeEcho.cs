using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
	public class LifeEcho : ModItem
	{
		public override void SetStaticDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000;
			Item.alpha = 100;
			Item.rare = ItemRarityID.White;
		}

		public override void PostUpdate() 
		{
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);

			PointGlowPreMultiplied FX = new();
			FX.Initialize(Main.rand.NextVector2FromRectangle(Item.Hitbox), Main.rand.NextVector2Circular(1f, 1f), new Color(184, 228, 242), 0.5f);
			ParticleEngine.ShaderParticles.Add(FX);
            
        }
	}

	public class LE_DROP_NPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
			if (OpusNPCDropHelper.Zombies.Contains(npc.type) || OpusNPCDropHelper.Skeletons.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeEcho>(), 1, 1, 5));
            }
        }
	}
}
