using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
		public int Timer;
		public override void SetStaticDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults() {
			Item.Size = new(45);
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 1000;
			Item.alpha = 100;
			Item.rare = ItemRarityID.White;
		}
		
		public override void PostUpdate()	
		{

			float LightMulti = MathHelper.SmoothStep(0.5f, 3, Item.stack / 60f);
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() *LightMulti * Main.essScale);
			PointGlowPreMultiplied FX = new();
			var a = Color.Lerp(Color.LightSkyBlue, Color.LightSlateGray, 0.9f);
			Color color = Color.Lerp(a, Color.LightGreen, MathF.Sin(Timer));
			Vector2 Velocity = Main.rand.NextVector2Circular(0.1f, 0) - Vector2.UnitY.RotatedBy(MathF.Sin(Timer) * 0.1f)*Main.rand.NextFloat(0.2f, 2);

            FX.Initialize(Item.Center + Main.rand.NextVector2Circular(1f, 1) * 2, Velocity, color, 0.75f, 60);
			ParticleEngine.ShaderParticles.Add(FX);

			Timer++;
            
        }
		
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
			//hide the item so that it just looks like the particle
			return false;
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
