using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class RoseGardenUnderground : ModBiome
	{

		public override ModWaterStyle WaterStyle => ModContent.GetInstance<RiftWaterStyle>();
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<RoseGardenUndergroundBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Corrupt;

		public override int Music
		{
			get
			{
                //return MusicLoader.GetMusicSlot(Mod, "Assets/Music/RiftDesertUnderground");
                return MusicID.Graveyard;
			}
		}

		public override void OnInBiome(Player player)
		{
            if (Main.rand.NextBool(4))
            {
                for (int t = 0; t < 5; t++)
                {
                    Dust.NewDust(Main.screenPosition, Main.screenWidth, Main.screenHeight, DustID.CursedTorch, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, -3));
                }
            }
        }


		public override int BiomeTorchItemType => TorchID.Demon;
		public override int BiomeCampfireItemType => ItemID.DemonCampfire;
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		public override string MapBackground => BackgroundPath;

		public override bool IsBiomeActive(Player player) {
			if (player.TryGetModPlayer<RoseGardenPlayer>(out RoseGardenPlayer Garden))
            {
                if (Garden.Active)
                {
                    return true;
                }
            }
			return false;
		}

		public override SceneEffectPriority Priority
		{
			get
			{
				return SceneEffectPriority.BiomeHigh;
			}
		}

        public override float GetWeight(Player player)
        {
            return 1f;
        }
	}
}