
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    public class InfernalDress : ModItem
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            // By passing this (the ModItem) into the item parameter we can reference it later in GetEquipSlot with just the item's name
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Highlight", EquipType.Legs, null, $"{Name}_Legs_Highlight");

            /* Here is example code for supporting a female-specifig legs equip texture. See SetMatch as well.
			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}_Female", EquipType.Legs, this, Name + "_Female");
			*/
        }

        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 16;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            // By changing the equipSlot to the leg equip texture slot, the leg texture will now be drawn on the player
            // We're changing the leg slot so we set this to true
            robes = true;
            // Here we can get the equip slot by name since we referenced the item when adding the texture
            // You can also cache the equip slot in a variable when you add it so this way you don't have to call GetEquipSlot
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            /* Here is example code for supporting a female-specifig legs equip texture. See Load as well.
			if (!male) {
				equipSlot = EquipLoader.GetEquipSlot(Mod, Name + "_Female", EquipType.Legs);
			}
			*/
        }

        public override void UpdateEquip(Player player)
        {
            int legHeight = player.height / 2;
            Vector2 legPos = player.position + new Vector2(0, legHeight);
            Dust.NewDust(legPos, player.width, legHeight, DustID.TintableDustLighted, 0f, 0f, 100, Color.OrangeRed, 0.6f);
            Lighting.AddLight(player.Center, Color.OrangeRed.ToVector3() * 0.5f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<BlackCloth>(4)
            .AddIngredient(ItemID.HellstoneBar, 6)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddTile(TileID.Loom)
            .Register();
        }
    }
    
    public class LegGlowmaskLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Leggings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.legs == EquipLoader.GetEquipSlot(Mod, "InfernalDress", EquipType.Legs))
            {
                Texture2D glowTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Equips/InfernalDress_Legs_Highlight").Value;
                Rectangle frame = player.legFrame;
                Vector2 position = drawInfo.Position - Main.screenPosition + player.legPosition + new Vector2(player.width / 2, player.height / 2);

                drawInfo.DrawDataCache.Add(new DrawData(
                    glowTex,
                    position,
                    frame,
                    Color.White,
                    player.legRotation,
                    frame.Size() / 2f,
                    1f,
                    drawInfo.playerEffect,
                    0
                ));
            }
        }
    }

}