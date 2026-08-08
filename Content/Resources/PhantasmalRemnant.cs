using DestroyerTest.Content.Particles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Resources
{
    public class PhantasmalRemnant : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.ItemIconPulse[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            //This max stack is for arbitrary reasons. Don't change this.
            Item.maxStack = 99;
            Item.value = 100;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
        }

        public static Color DrawColor => Opus.Sine(Color.Pink, Color.DarkSalmon, 0.03f);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D Tex = TextureAssets.Item[Type].Value;

            Main.EntitySpriteDraw(Tex, position, frame, Color.White * 0.5f, 0f, origin, scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Tex, position + new Vector2(Opus.Sine(-2f, 2f), 0), frame, DrawColor with { A = 0 }, 0f, origin, scale * 1.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(Tex, position + new Vector2(0f, Opus.Sine(-2f, 2f)), frame, DrawColor with { A = 0 }, 0f, origin, scale * 1.15f, SpriteEffects.None);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D Tex = TextureAssets.Item[Type].Value;

            Main.EntitySpriteDraw(Tex, Item.Center - Main.screenPosition, null, Color.White * 0.5f, rotation, Tex.Size() / 2, scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Tex, (Item.Center + new Vector2(Opus.Sine(-2f, 2f), 0)) - Main.screenPosition, null, DrawColor with { A = 0 }, rotation, Tex.Size() / 2, scale * 1.15f, SpriteEffects.None);
            Main.EntitySpriteDraw(Tex, (Item.Center + new Vector2(0f, Opus.Sine(-2f, 2f))) - Main.screenPosition, null, DrawColor with { A = 0 }, rotation, Tex.Size() / 2, scale * 1.15f, SpriteEffects.None);
            return false;
        }
    }
}
