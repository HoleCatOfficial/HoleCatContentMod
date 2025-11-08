using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Pets;
using DestroyerTest.Content.Projectiles.ShadeThrasherFriendly;
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
	public class ShadeThrasherBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        }
		public override void Update(Player player, ref int buffIndex) { // This method gets called every frame your buff is active on your player.
			bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<ShadeThrasherFriendlyHead>());

            /*
            if (player.TryGetModPlayer<TenebrisScepterPlayer>(out TenebrisScepterPlayer Scepter))
            {
                if (!Scepter.Active)
                {
                    player.DelBuff(buffIndex);
                }
            }
            */
		}
	}
}