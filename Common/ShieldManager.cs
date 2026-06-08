using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.Cards.RiftenDeck;
using DestroyerTest.Content.Equips.PetrifiedSet;
using Humanizer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ShieldManager : ModSystem
    {
        public static ShieldManager Instance;
        public static ShieldDrawer Drawer;

        public override void Load()
        {
            Instance = ModContent.GetInstance<ShieldManager>();
            
            ActiveShields = new List<Shield>[Main.maxPlayers];

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (ActiveShields != null)
                {
                    ActiveShields[i] = new List<Shield>();
                }
            }

            if (!Main.dedServ)
            {
                Drawer = new ShieldDrawer();
                PlayerPixelRegistry.Register(Drawer);
            }
        }

        public override void Unload()
        {
            for (int i = 0; i < ActiveShields?.Length; i++)
            {
                ActiveShields[i]?.Clear();
            }
            
            ActiveShields = null;
            LoadedShields.Clear();
            LoadedShields = null;
            Instance = null;

            if (!Main.dedServ && Drawer is not null)
            { 
                PlayerPixelRegistry.Unregister(Drawer);
            }
            Drawer = null;
        }

        public static List<Shield> LoadedShields = new List<Shield>();

        public static List<Shield>[] ActiveShields;

        public static void ActivateShield(Shield shield, Player player)
        {
            ActiveShields[player.whoAmI].Add(shield);
        }

        public static void ActivateShield(Shield shield, int Index)
        {
            Index = (int)MathHelper.Clamp(Index, 0, Main.maxPlayers - 1);
            ActiveShields[Index].Add(shield);
        }


    }
}
