using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class ModdedDebuffInfliction : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

      

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int Type = projectile.type;
            
        }

        List<int> HallowedBossProjectiles = new()
        {
            ProjectileID.HallowBossSplitShotCore,
            ProjectileID.HallowBossLastingRainbow,
            ProjectileID.HallowBossRainbowStreak,
            ProjectileID.FairyQueenLance,
            ProjectileID.FairyQueenSunDance,
            ProjectileID.FairyQueenHymn,
            ModContent.ProjectileType<BlessedLaser>(),
            ModContent.ProjectileType<BlessedLaser2>(),
            ModContent.ProjectileType<BlessedNodeCrystal>(),
            ModContent.ProjectileType<BlessedNodeCrystal2>(),
            ModContent.ProjectileType<HallowBolt>(),
        };

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            int Type = projectile.type;

            if (HallowedBossProjectiles.Contains(Type))
            {
                target.AddBuff(ModContent.BuffType<LightInferno>(), 300);
            }
        }
    }
}
