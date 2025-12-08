using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RiftBiome
{
    public class RiftNPCSpawnControl : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        
    }
}