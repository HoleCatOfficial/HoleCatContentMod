using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Tiles.RoseGarden
{
	public class RoseGardenUnderground : ModBiome
	{

        public override ModWaterStyle WaterStyle => ModContent.GetModWaterStyle(WaterStyleID.Corrupt);  
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<RoseGardenUndergroundBackgroundStyle>();
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Corrupt;

		public override int Music
		{
			get
			{
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/HekateGarden");
			}
		}

        public override void OnEnter(Player player)
        {
			//Main.NewText("IMPORTANT: The Song Used here is a placeholder. 'I, Am the First Flower' from Desolo Zantas's 'Omniphobia'. I claim no ownership of this track. It will be replaced in due time.");
			SoundStyle Entry = new SoundStyle("DestroyerTest/Assets/Audio/EnterRoseGarden") with { PitchVariance = 0.5f, MaxInstances = 0 };
            SoundEngine.PlaySound(Entry);
        }

		public override void OnLeave(Player player)
        {
			SoundStyle Exit = new SoundStyle("DestroyerTest/Assets/Audio/ExitRoseGarden") with { PitchVariance = 0.5f, MaxInstances = 0 };
            SoundEngine.PlaySound(Exit);
        }

		public override void OnInBiome(Player player)
		{
            player.ZoneCorrupt = true;
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
					return Garden.Active;
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