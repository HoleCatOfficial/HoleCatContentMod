using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Audio;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Buffs;
using Terraria.DataStructures;
using OpusLib;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Tile_RoseGardenEffectSource : ModTile
	{
		public override void SetStaticDefaults()
		{
			DustType = DustID.CorruptionThorns;
			HitSound = SoundID.DD2_MonkStaffGroundImpact;
			MineResist = 16000f;
			MinPick = 255;
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Vector2[] Points = Opus.GetEquidistantOrbitVectors(36, new Vector2(i * 16, j * 16), 0.01f, 1200f);

            for (int ind = 0; ind < Points.Length; ind++)
            {
                Dust.NewDustPerfect(Points[ind], DustID.TintableDustLighted, Vector2.Zero, 0, Color.White, 3f);
            }
        }

		public override void NearbyEffects(int i, int j, bool closer)
		{
            for (int f = 0; f < Main.maxPlayers; f++)
			{
				Player player = Main.player[f];
				if (player == null || !player.active)
					continue;

				Vector2 tileWorldPos = new Vector2(i * 16 + 8, j * 16 + 8); // center of tile
				float distance = Vector2.Distance(player.Center, tileWorldPos);

				if (distance < 1200f)
					{
						if (player.TryGetModPlayer<RoseGardenPlayer>(out RoseGardenPlayer Garden))
                        {
                            Garden.Active = true;
                        }
					}
                else
                    {
                        if (player.TryGetModPlayer<RoseGardenPlayer>(out RoseGardenPlayer Garden))
                        {
                            Garden.Active = false;
                        }
                    }
			}
		}
	}

    public class RoseGardenPlayer : ModPlayer
    {
        public bool Active = false;

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
                RemoveBuffs(Player);
                Player.AddBuff(BuffID.NoBuilding, 4);
                Player.AddBuff(BuffID.Blackout, 4);

                Player.noBuilding = true;
            }
        }
        
        public void RemoveBuffs(Player player)
        {
            for (int k = 0; k < Player.MaxBuffs; k++)
                {
                    if (player.buffType[k] == BuffID.Sunflower ||
                    player.buffType[k] == BuffID.Shine ||
                    player.buffType[k] == BuffID.NightOwl ||
                    player.buffType[k] == BuffID.Spelunker)
                    {
                        player.DelBuff(k);
                        break;
                    }
                }
        }
    }
}