using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class DivineServantSystem : ModSystem
    {
        public static bool[] IsServant = new bool[Main.maxPlayers];
        public static int[] Level = new int[Main.maxPlayers];

        public override void PreUpdatePlayers()
        {
            for (int j = 0; j < Main.maxPlayers; j++)
            {
                if (IsServant[j])
                {
                    for (int i = 0; i < Level.Length; i++)
                    {
                        Level[i] = (int)MathHelper.Clamp(Level[i], 1, 23);
                    }
                }
            }
        }
        
    }

    public class DivineServantPlayer : ModPlayer
    {

        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PreUpdate()
        {
            Active = DivineServantSystem.IsServant[Player.whoAmI];
            UpdateScaling();
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow != 0)
            {
                return;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            if (DTConfig.instance.EnableDebugMessages)
            {
                Utils.DrawBorderString(spriteBatch, $"Potion sickness time Multiplier: {PotionDelayReduction}", (drawInfo.drawPlayer.Center + new Vector2(0, 30)) - Main.screenPosition, Color.Green, 0.5f, 0.5f, 0.5f);
                Utils.DrawBorderString(spriteBatch, $"Current Level: {DivineServantSystem.Level[Player.whoAmI]}", (drawInfo.drawPlayer.Center + new Vector2(0, 40)) - Main.screenPosition, Color.Green, 0.5f, 0.5f, 0.5f);
            }
        }

        public int steps = 0;
        public int maxSteps = 24;

        public void UpdateScaling()
        {
            steps = DivineServantSystem.Level[Player.whoAmI];

            if (maxSteps <= 0)
                return;

            float t = steps / (float)maxSteps;
            t = MathHelper.Clamp(t, 0f, 1f);

            PotionDelayReduction = MathHelper.Lerp(MinPotionDelayReduction, MaxPotionDelayReduction, t);
        }

        public float MinPotionDelayReduction = 0.8f;
        public float MaxPotionDelayReduction = 0.2f;
        public float PotionDelayReduction = 1f;
        public override bool ApplyPotionDelay(Item item, int potionDelay)
        {
            if (Active)
            {
                potionDelay = (int)(potionDelay * PotionDelayReduction);
            }
            return true;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana);
            if (Active)
            {
                health /= 2;
            }
        }
    }
}
