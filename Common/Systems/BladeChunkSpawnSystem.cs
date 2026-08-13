using DestroyerTest.Content.BossSummons;
using DestroyerTest.Content.CheatItems;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RogueItems;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class BladeChunkSpawnSystem : ModSystem
    {
        public override void PostUpdateTime()
        {
            if (Main.rand.NextBool(3500) && !Main.dayTime && !Main.dedServ && !Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                Main.NewText("Something zips down from the skies...");
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), Main.LocalPlayer.Center + new Vector2(Main.rand.Next(-600, 600), -2000), Vector2.Zero, ModContent.ProjectileType<BladeChunkProjectile>(), 50, 10);
            }
        }
    }
}
