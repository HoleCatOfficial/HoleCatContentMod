using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class LagWarnSystem : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            foreach (NPC N in Main.ActiveNPCs)
            {
                if (N.type == ModContent.NPCType<WyvernCorpseHead>())
                {
                    if (Main.FrameSkipMode == Terraria.Enums.FrameSkipMode.On)
                    {
                        Utils.DrawBorderStringBig(spriteBatch, "You may experience framrate drops when using Frame Skip mode: ON. Use subtle to avoid this.", new Vector2(Main.screenWidth / 2, (Main.screenHeight / 2) + 400), Color.Orange, 1f, 0.5f, 0.5f);
                    }
                }
            }
        }
    }
}
