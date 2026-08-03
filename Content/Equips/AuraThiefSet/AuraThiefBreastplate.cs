
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
 
using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.AuraThiefSet
{
// This item is meant to mirror the effects of the Hallowed Plate Mail, which equips a Cape without needing a separate cape Item. 

	[AutoloadEquip(EquipType.Body)] // As usual, we must tell the game what part of the body the item will be equipped on.
    	public class AuraThiefBreastplate : ModItem
		{
        public int equipBack = -1; // It would be best not to tamper with this.
        
        public override void Load() // This fetches the texture we need

        { 
            if (Main.netMode != NetmodeID.Server) {
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
            }
        }

        public override void SetStaticDefaults() // These will display the texture we fetched, and are specifically for this purpose.
        {
            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;
        }
		public override void SetDefaults() // Simple item properties. Nothing new here.
		{
			Item.width = 30;
			Item.height = 20; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<LifeEchoRarity>(); // The rarity of the item
			Item.defense = 10;
			// Now, in case you might be asking "Why use that special default when you can just copy what the original Hallowed Plate Mail does?"
			// Unfortunately for you, while cloning the defaults does load a cape on the back, it loads the Hallowed Armor cape, and replaces your body armor textures with the Hallowed Plate Mail Textures.
			//Item.CloneDefaults(ItemID.HallowedPlateMail);
		}

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;

            Rectangle P = Utils.CenteredRectangle(player.Center, new Vector2(32, 48));
            if (Math.Abs(player.velocity.X) > 3.75f)
            {
                //Dust.NewDustDirect(player.Bottom, 2, 1, ModContent.DustType<SoulDust>(), 0, 0.02f, 100, new Microsoft.Xna.Framework.Color(184, 228, 242), 1);

                PointGlowPreMultiplied Glow = new PointGlowPreMultiplied();
                Glow.Initialize(Main.rand.NextVector2FromRectangle(P), Main.rand.NextVector2Circular(3, 3), new Color(184, 228, 242), 0.5f);
                ParticleEngine.ShaderParticles.Add(Glow);
            }
        }

		public override void AddRecipes() //Added to make the item obtainable without needing cheat mods, since many swear by never using cheats, ever.
		{
			CreateRecipe()
                .AddIngredient<LifeEcho>(15)
                .AddIngredient(ItemID.Wood, 20)
                .AddIngredient(ItemID.FlinxFur, 8)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}