
using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class ScepterImbueCF : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.persistentBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.TryGetGlobalProjectile<WeaponImbueScepter>(out var scepter))
                {
					if(!scepter.HasImbue)
					{
                    	scepter.CursedFlame = true;
					}
                }
            }
		}
	}
}