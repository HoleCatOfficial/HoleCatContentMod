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

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class Tile_RoseGardenEffectSource : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			//LocalizedText name = CreateMapEntryName();
			//AddMapEntry(new Color(111, 86, 101), name);

			DustType = DustID.CorruptionThorns;
			HitSound = SoundID.DD2_MonkStaffGroundImpact;
			MineResist = 16000f;
			MinPick = 255;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			for (int f = 0; f < Main.maxPlayers; f++)
			{
				Player player = Main.player[f];
				Vector2 tileWorldPos = new Vector2(i * 16 + 8, j * 16 + 8); // center of tile
				
				if (f < Main.maxPlayers && f > -1)
				{
					float distance = Vector2.Distance(player.Center, tileWorldPos);

					if (distance < 1200f)
					{
						if (player.TryGetModPlayer<RoseGardenPlayer>(out RoseGardenPlayer Garden))
                        {
                            Garden.Active = true;
                        }
					}
				}
			}
		}
	}

    public class RoseGardenPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
                RemoveBuffs(Player);
                Player.AddBuff(BuffID.NoBuilding, 4);
            }
        }
        
        public void RemoveBuffs(Player player)
        {
            if (player.HasBuff(BuffID.Sunflower))
            {
                player.DelBuff(BuffID.Sunflower);
            }
            if (player.HasBuff(BuffID.Titan))
            {
                player.DelBuff(BuffID.Titan);
            }
        }
    }
}