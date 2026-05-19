using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.Cards.AstirDeck
{
    public class Depths : ModItem
    {

    }

    public class DepthsPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;

        }

        public int Heat = 0;
        public int MaxHeat = 400;

        public override void PostUpdateEquips()
        {
            bool Below = Heat < MaxHeat;
            if (Player.adjLava)
            {
                if (Player.miscCounter % 60 == 0 && Below)
                {
                    Heat++;
                }
            }
            else if (Player.lavaWet)
            {
                if (Player.miscCounter % 15 == 0 && Below)
                {
                    Heat++;
                }
            }
            else
            {
                if (Player.miscCounter % 120 == 0 && Heat > 0)
                {
                    Heat--;
                }
            }

            curDMGBonus = MathHelper.Lerp(0f, MaxDMGBonus, (float)Heat / (float)MaxHeat);
        }

        float MaxDMGBonus = 0.45f;
        float curDMGBonus = 0f;
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (Active)
            {
                damage += curDMGBonus;
            }
        }
    }

    public class DepthsGlobal : GlobalTile
    {
        List<int> HotTiles = new()
        {
            TileID.Hellstone,
            TileID.HellstoneBrick,
            TileID.AncientHellstoneBrick
        };
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (HotTiles.Contains(type))
            {
                closer = false;

                if (Main.LocalPlayer.TryGetModPlayer<DepthsPlayer>(out var Deep))
                {
                    if (Main.LocalPlayer.miscCounter % 60 == 0)
                    {
                        Deep.Heat += 1;
                    }
                }
            }
        }
    }
}
