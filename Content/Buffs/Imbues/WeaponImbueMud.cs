using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs.Imbues
{
	public class WeaponImbueMud : BaseImbueBuff
    {
        public override WeaponImbuePlayer.Imbues Imbue => WeaponImbuePlayer.Imbues.Mud;
    }
}