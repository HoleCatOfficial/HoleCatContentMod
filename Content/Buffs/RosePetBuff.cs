
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Pets;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{

	public class RosePetBuff : ModBuff
    {
     
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }
		public override void Update(Player player, ref int buffIndex) {
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RosePet>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}