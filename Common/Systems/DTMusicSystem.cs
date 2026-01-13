using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems;

internal class MusicDisplayCalls : ModSystem
{
	public override void PostAddRecipes()
	{
		if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
			return;

        LocalizedText modName = Language.GetText("Mods.DestroyerTest.ModName");

        Color[] FranciumColors = new Color[4]
        {
            Color.White,
            ColorLib.HoleCatFireGradient,
            Color.White,
            Color.White
        };

        Color[] MiscColors = new Color[4]
        {
            Color.White,
            Color.White,
            Color.White,
            Color.White
        };

        Color[] ZantasColors = new Color[4]
        {
            Color.Gold,
            Color.Gold,
            Color.Gold,
            Color.White
        };

        Color[] JokeColors = new Color[4]
        {
            Main.DiscoColor,
            Main.DiscoColor,
            Main.DiscoColor,
            Main.DiscoColor
        };

		void AddMusic(string path, string name, string AuthorName, Color[] colors)
        {
            LocalizedText author = Language.GetText($"Mods.DestroyerTest.Music.{AuthorName}");
            LocalizedText displayName = Language.GetText($"Mods.DestroyerTest.Music.{name}");

            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), displayName, author, modName, null, colors);
        }

        void AddMusic_Rift(string path, string name, string AuthorName)
        {
            LocalizedText author = Language.GetText($"Mods.DestroyerTest.Music.{AuthorName}");
            LocalizedText displayName = Language.GetText($"Mods.DestroyerTest.Music.{name}");
            Color[] FranciumColorsRift = new Color[4]
            {
                Color.White,
                ColorLib.HoleCatFireGradient,
                ColorLib.Rift,
                Color.White
            };

            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), displayName, author, modName, null, FranciumColorsRift);
        }


        //Boss Tracks
		AddMusic("Assets/Music/Tribulation", "EvilBoss.Default", "Author1", FranciumColors);
        AddMusic("Assets/Music/Placeholder4", "EvilBoss.Eternity", "Author2", ZantasColors);
        AddMusic("Assets/Music/EvilBossSecretSeed", "EvilBoss.SecretSeed", "Author4", JokeColors);
        AddMusic("Assets/Music/ConstitutionBoss", "ConstitutionBoss.Default", "Author1", FranciumColors);
        AddMusic("Assets/Music/ConstitutionDespiration", "ConstitutionBoss.Desperate", "Author1", FranciumColors);
        AddMusic("Assets/Music/Placeholder5", "ConstitutionBoss.Eternity", "Author3", MiscColors);
        AddMusic("Assets/Music/NodeBoss", "NodeBoss.Fight", "Author1", FranciumColors);
        AddMusic("Assets/Music/NodeIdle", "NodeBoss.Idle", "Author1", FranciumColors);
        AddMusic("Assets/Music/TenebrousConstruct", "TenebrousConstruct", "Author1", FranciumColors);
        AddMusic("Assets/Music/RoseSoulAmbience", "RoseSoulAmbience", "Author1", FranciumColors);
        AddMusic("Assets/Music/WyvernSoulAmbience", "WyvernSoulAmbience", "Author1", FranciumColors);
        AddMusic("Assets/Music/HekateGarden", "HekateGarden", "Author1", FranciumColors);

        //Rift Biome
        AddMusic_Rift("Assets/Music/RiftV2", "Rift.Surface", "Author1");
        AddMusic_Rift("Assets/Music/RiftUnderground", "Rift.Underground", "Author1");
        AddMusic_Rift("Assets/Music/RiftCaverns", "Rift.Caverns", "Author1");
        AddMusic_Rift("Assets/Music/RiftDesert", "Rift.Desert.Surface", "Author1");
        AddMusic_Rift("Assets/Music/RiftDesertUnderground", "Rift.Desert.Underground", "Author1");
        AddMusic_Rift("Assets/Music/RiftSandstorm", "Rift.Desert.Sandstorm", "Author1");
        AddMusic_Rift("Assets/Music/RiftRain", "Rift.Rain", "Author1");
        AddMusic_Rift("Assets/Music/RiftThunderstorm", "Rift.Thunderstorm", "Author1");
        AddMusic_Rift("Assets/Music/RiftEvent", "Rift.Event", "Author1");
        AddMusic_Rift("Assets/Music/RiftIce", "Rift.Ice.Surface", "Author1");

        //Other
	}
}