using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Remnants
{
    public class NightmareRoseArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 20;
            Item.maxStack = 99;
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
        }
    }
}
