using System;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.Cards.RiftenDeck;
using DestroyerTest.Content.Equips.PetrifiedSet;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ShieldRegistrationSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            ShieldManager.Instance.shields.Add(ModContent.GetInstance<InfernalShieldPlayer>());
            ShieldManager.Instance.shields.Add(ModContent.GetInstance<PetrifiedShieldPlayer>());
            ShieldManager.Instance.shields.Add(ModContent.GetInstance<HollowShield>());
        }
    }
}