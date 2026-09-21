
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    [AutoloadGlowmask]
    public class InfernalDress : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            GlowmaskLoader.QueueGlowmaskRegistration($"{Texture}_Legs_Glow");
        }

        public override void SetStaticDefaults()
        {
            GlowmaskLoader.AssignGlowmaskTexture_Equip(Item.glowMask, EquipType.Legs, EquipLoader.GetEquipSlot(Mod, "InfernalDress_Legs", EquipType.Legs));
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
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void UpdateEquip(Player player)
        {
            Rectangle DustBox = Utils.CenteredRectangle(player.Bottom + new Vector2(0, -4), new Vector2(((int)player.width).WrapToTwo(), 12));
            if (Main.rand.NextBool(4))
            {
                PixelParticlePlayer Pixel = new(player);
                Pixel.Initialize((new Vector2(((int)DustBox.TopLeft().X).WrapToTwo(), ((int)DustBox.TopLeft().Y).WrapToTwo()) + new Vector2(Main.rand.Next(DustBox.Width).WrapToTwo(), Main.rand.Next(DustBox.Height).WrapToTwo())) + new Vector2(1f, 1f), new Vector2(0f, 0.2f), new Color(255, 49, 32), 2f, 30);
                ParticleEngine.Particles.Add(Pixel);
            }
            
            Lighting.AddLight(player.Center, new Color(255, 49, 32).ToVector3() * 0.5f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 4)
            .AddIngredient(ItemID.HellstoneBar, 6)
            .AddIngredient(ItemID.Obsidian, 4)
            .AddTile(TileID.Anvils)
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