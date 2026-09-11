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
    public class BlueBarItem : ModItem
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
            ChargeBar BlueBar = PlayerChargeBarManager.AddBar(player, new ChargeBar("BlueBar", ModContent.Request<Texture2D>(Path + "/BlueBarFrame"), ModContent.Request<Texture2D>(Path + "/BlueBarFront"), ModContent.Request<Texture2D>(Path + "/BlueBarBack"), Opus.Sine(0f, 15f), 15f));
            BlueBar.CurrentValue = Opus.Sine(0f, 15f);

            //PlayerChargeBarManager.RemoveBar(player, BlueBar);


        }
    }
}