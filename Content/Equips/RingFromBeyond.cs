using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;

namespace DestroyerTest.Content.Equips
{
    public class RingFromBeyond : ModItem
    {
        public Asset<Texture2D> Alts;
        int variant = 0;

        public override void SetStaticDefaults()
        {
            variant = Main.rand.Next(3);
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.masterOnly = true;
            Item.defense = 8;
            Item.accessory = true;

            Alts = ModContent.Request<Texture2D>(Texture + "Alts");
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameHeight = 22;

            Rectangle Frame = new Rectangle(0, frameHeight * variant, frameHeight, Alts.Value.Width);
            Vector2 orig = new Vector2(Alts.Value.Width / 2, (frameHeight * variant) / 2);

            Main.EntitySpriteDraw(Alts.Value, position + new Vector2(0f * Main.UIScale, -12f * Main.UIScale), Frame, drawColor, 0f, orig, scale, SpriteEffects.None);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.24f;
            player.GetDamage(DamageClass.Magic) += 0.24f;
            player.GetDamage<ScepterClass>() += 0.24f;
            player.GetCritChance(DamageClass.Generic) += 16;


        }
    }
}
