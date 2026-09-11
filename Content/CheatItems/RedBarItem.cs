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
    public class RedBarItem : ModItem
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
            ChargeBar RedBar = PlayerChargeBarManager.AddBar(player, new ChargeBar("RedBar", ModContent.Request<Texture2D>(Path + "/RedBarFrame"), ModContent.Request<Texture2D>(Path + "/RedBarFront"), ModContent.Request<Texture2D>(Path + "/RedBarBack"), Opus.Sine(0f, 15f, -0.05f), 15f));
            RedBar.CurrentValue = Opus.Sine(0f, 15f, -0.05f);
            //PlayerChargeBarManager.RemoveBar(player, RedBar);
        }
    }
}