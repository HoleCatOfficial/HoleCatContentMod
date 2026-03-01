using System.Collections.Generic;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
	public class CursorLantern : ModItem
	{
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 10;

			Item.value = Item.buyPrice(silver: 12, copper: 4); 
			Item.rare = ItemRarityID.White;
			Item.accessory = true;
            Item.vanity = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<CursorLanternPlayer>(out var Lantern))
            {
                Lantern.Active = true;
            }
        }
        public override void UpdateVanity(Player player)
        {
            if (player.TryGetModPlayer<CursorLanternPlayer>(out var Lantern))
            {
                Lantern.Active = true;
            }
        }
	}

    public class CursorLanternPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        int CurrentFrame = 0;

        public override void PostUpdateEquips()
        {
            if (Player.miscCounter % 3 == 0)
            {
                CurrentFrame++;
            }

            if (CurrentFrame > 3)
            {
                CurrentFrame = 0;
            }
            if (Active/* && Player.unlockedBiomeTorches*/)
            {
                Vector2 LightPosition = Main.MouseWorld + new Vector2(0, 6);
                Tile t = Framing.GetTileSafely(LightPosition);
                if (!Main.tileSolid[t.TileType] || !t.HasTile) // <--- How do I index this when Tile.type is deprecated?
                {
                    Lighting.AddLight(LightPosition, TorchID.Torch); //Placeholder value.
                }
            }
        }

        public Vector2 prevMouseWorld;
        public float swayRotation;
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Active)
            {
                Texture2D val = DTAssetLib.CursorLanternTexture.Value;
                int frameHeight = val.Height / 4;
                
                Rectangle frame = new Rectangle(
                    0,
                    frameHeight * CurrentFrame,
                    val.Width,
                    frameHeight
                );
                Vector2 origin = new Vector2(val.Width / 2f, 0);

                Vector2 mouseDelta = Main.MouseWorld - prevMouseWorld;
                prevMouseWorld = Main.MouseWorld;

                float targetRotation = MathHelper.Clamp(mouseDelta.X * 0.5f, -2f, 2f);
                swayRotation = MathHelper.Lerp(swayRotation, targetRotation, 0.1f);

                if (drawInfo.shadow == 0)
                {
                    Main.EntitySpriteDraw(val, Main.MouseWorld - Main.screenPosition, frame, Color.White, swayRotation, origin, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }

    public class CursorLanternSpawnPlayer : ModPlayer
    {
        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            itemsByMod["DestroyerTest"].Add(ModContent.GetInstance<CursorLantern>().Item);
        }
    }
}
