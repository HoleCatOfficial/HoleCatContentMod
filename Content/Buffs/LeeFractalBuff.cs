
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.SummonItems.FractalSummon;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{

	public class LeeFractalBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) 
        {
            bool b1 = false;
            
            foreach (int t in LeeFractal.Swords)
            {
                if (player.ownedProjectileCounts[t] > 0)
                {
                    b1 = true;
                    break;
                }
            }
            if (b1) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}