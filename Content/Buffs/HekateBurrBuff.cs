
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.player.Potion;
using Humanizer;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
	public class HekateBurrBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<HekateBurrPlayer>().ShouldUpdate = true;
		}
	}

	

	public class HekateBurrPlayer : ModPlayer
	{
        public const int SpawnNewWait = 600;
        public bool Init = false;
        private int spawnTimer = 0;

        public int CountFreeRingBurrs()
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.owner != Player.whoAmI)
                    continue;

                if (proj.type != ModContent.ProjectileType<HekateBurr>())
                    continue;

                HekateBurr burr = proj.ModProjectile as HekateBurr;
                if (burr != null && burr.attachedNPCIndex == -1)
                    count++;
            }
            return count;
        }


        public int GetFreeRingIndex()
        {
            bool[] occupied = new bool[12];

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.owner != Player.whoAmI)
                    continue;

                if (proj.type != ModContent.ProjectileType<HekateBurr>())
                    continue;

                HekateBurr burr = proj.ModProjectile as HekateBurr;
                if (burr == null || burr.attachedNPCIndex != -1)
                    continue;

                if (burr.RingIndex >= 0 && burr.RingIndex < 12)
                    occupied[burr.RingIndex] = true;
            }

            for (int i = 0; i < 12; i++)
                if (!occupied[i])
                    return i;

            return -1; // ring full
        }


        public override void PostUpdateBuffs()
        {
            if (!Player.HasBuff<HekateBurrBuff>())
            {
                Init = false;
            }
            if (!Init && Player.HasBuff<HekateBurrBuff>())
            {
                for (int i = 0; i < 12; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_Misc("HekateBurr"), Player.Center, Vector2.Zero, ModContent.ProjectileType<HekateBurr>(), 7, 1, Player.whoAmI);
                }
                Init = true;
            }
            else if (ShouldUpdate)
            {
                int currentCount = CountFreeRingBurrs();
                if (currentCount < 12)
                {
                    spawnTimer++;
                    if (spawnTimer >= SpawnNewWait)
                    {
                        Projectile.NewProjectile(Player.GetSource_Misc("HekateBurr"), Player.Center, Vector2.Zero, ModContent.ProjectileType<HekateBurr>(), 7, 1, Player.whoAmI);
                        spawnTimer = 0;
                    }
                }
                else
                {
                    spawnTimer = 0;
                }
            }
        }

		public bool ShouldUpdate;

		public override void ResetEffects() {
			ShouldUpdate = false;
		}
	}
}