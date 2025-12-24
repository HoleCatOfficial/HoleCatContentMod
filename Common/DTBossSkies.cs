using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using Terraria.Graphics.Effects;
using DestroyerTest.Content.Entities;

namespace DetroyerTest.Common
{
    //Uses Calamity Mod's Astral Sky Setup
    public class WyvernCorpseSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }


        public override void Deactivate(params object[] args)
        {
            foreach(NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<WyvernCorpseHead>() && npc.active)
                {
                    skyActive = true;
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }
    
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
             // Small worlds, default draw height.
            int AstralBiomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 2;

            // Medium worlds.
            if (Main.maxTilesX >= 6400 && Main.maxTilesX < 8400)
            {
                AstralBiomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 4;
            }

            if (Main.maxTilesX >= 8400)
            {
                AstralBiomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 140;
            }

            float whateverTheFuckThisVariableIsSupposedToBe = 3.40282347E+38f;
            if (maxDepth >= whateverTheFuckThisVariableIsSupposedToBe && minDepth < whateverTheFuckThisVariableIsSupposedToBe)
            {
                spriteBatch.Draw(DTAssetLib.WyvernCorpseSky.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), ColorLib.IchorCrystalGradient * opacity);
            }
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            
            if (skyActive && opacity < 1f)
            {
                opacity += 0.02f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.02f;
            }

            Opacity = opacity;

            foreach(NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<WyvernCorpseHead>() && npc.active)
                {
                    skyActive = true;
                }
                else
                {
                    skyActive = false;
                }
            }
        }
    }
}