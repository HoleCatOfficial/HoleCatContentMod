
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tools
{
    [AutoloadHead]
    public class ShiningObelisk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 4));
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 168;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisImpact");
            Item.maxStack = 1;
        }
    }
}