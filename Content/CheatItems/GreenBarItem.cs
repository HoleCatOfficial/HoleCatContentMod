using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using OpusLib;


namespace DestroyerTest.Content.CheatItems
{
    public class GreenBarItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            string Path = DTAssetLib.ExtrasPath + "/Test";
            ChargeBar GreenBar = PlayerChargeBarManager.AddBar(player, new ChargeBar("GreenBar", ModContent.Request<Texture2D>(Path + "/GreenBarFrame"), ModContent.Request<Texture2D>(Path + "/GreenBarFront"), ModContent.Request<Texture2D>(Path + "/GreenBarBack"), Opus.Sine(0f, 15f, 0.01f), 15f));
            GreenBar.CurrentValue = Opus.Sine(0f, 15f, 0.01f);
            //PlayerChargeBarManager.RemoveBar(player, GreenBar);
        }
    }
}