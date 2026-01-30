using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.Cards.RiftenDeck;
using DestroyerTest.Content.Equips.PetrifiedSet;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ShieldManager : ModSystem
    {
        // Singleton-style access
        public static ShieldManager Instance;

        public List<ShieldPlayer> shields = new List<ShieldPlayer>();

        public override void PostSetupContent()
        {
            Instance = this;
        }

        public override void OnWorldUnload()
        {
            Instance = null;
        }

    }
}
