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

		void AddMusic(string path, string name, string AuthorName)
        {
            LocalizedText author = Language.GetText($"Mods.DestroyerTest.Music.{AuthorName}");
            LocalizedText displayName = Language.GetText($"Mods.DestroyerTest.Music.{name}");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), displayName, author, modName);
        }


        //Boss Tracks
		AddMusic("Assets/Music/Tribulation", "EvilBoss.Default", "Author1");
        AddMusic("Assets/Music/Placeholder4", "EvilBoss.Eternity", "Author2");
        AddMusic("Assets/Music/EvilBossSecretSeed", "EvilBoss.SecretSeed", "Author4");
        AddMusic("Assets/Music/ConstitutionBoss", "ConstitutionBoss.Default", "Author1");
        AddMusic("Assets/Music/ConstitutionDespiration", "ConstitutionBoss.Desperate", "Author1");
        AddMusic("Assets/Music/Placeholder5", "ConstitutionBoss.Eternity", "Author3");
        AddMusic("Assets/Music/NodeBoss", "NodeBoss.Fight", "Author1");
        AddMusic("Assets/Music/NodeIdle", "NodeBoss.Idle", "Author1");
        AddMusic("Assets/Music/TenebrousConstruct", "TenebrousConstruct", "Author1");
        AddMusic("Assets/Music/RoseSoulAmbience", "RoseSoulAmbience", "Author1");
        AddMusic("Assets/Music/WyvernSoulAmbience", "WyvernSoulAmbience", "Author1");
        AddMusic("Assets/Music/Placeholder6", "HekateGarden", "Author2");

        //Rift Biome
        AddMusic("Assets/Music/RiftV2", "Rift.Surface", "Author1");
        AddMusic("Assets/Music/RiftUnderground", "Rift.Underground", "Author1");
        AddMusic("Assets/Music/RiftCaverns", "Rift.Caverns", "Author1");
        AddMusic("Assets/Music/RiftDesert", "Rift.Desert.Surface", "Author1");
        AddMusic("Assets/Music/RiftDesertUnderground", "Rift.Desert.Underground", "Author1");
        AddMusic("Assets/Music/RiftSandstorm", "Rift.Desert.Sandstorm", "Author1");
        AddMusic("Assets/Music/RiftRain", "Rift.Rain", "Author1");
        AddMusic("Assets/Music/RiftThunderstorm", "Rift.Thunderstorm", "Author1");
        AddMusic("Assets/Music/RiftEvent", "Rift.Event", "Author1");
        AddMusic("Assets/Music/RiftIce", "Rift.Ice.Surface", "Author1");

        //Other
	}
}