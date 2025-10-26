using DestroyerTest.Content.Consumables;
using Microsoft.Xna.Framework;
using Stubble.Core;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ScholarHelperPlayer : ModPlayer
    {
        public bool LifeEcho = false;
        public bool Daybloom = false;
        public bool Blinkroot = false;
        public bool Dalmon = false;
        public bool SynergyWrap = false;

        public override void PostUpdate()
        {
            if (Player.HeldItem.type == ModContent.ItemType<Dalmon>())
            {
                Dalmon = true;
            }
            if (Player.HeldItem.type == ModContent.ItemType<SynergyWrap>())
            {
                SynergyWrap = true;
            }
        }

    }
}