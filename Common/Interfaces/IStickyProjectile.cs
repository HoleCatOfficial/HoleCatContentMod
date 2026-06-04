using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Interfaces
{
    public interface IStickyProjectile
    {
        public bool IsStickingToTarget { get; set; }
        public bool CanStickToTargets { get; }

        public void OnStickToTarget(NPC target);

        public void DuringStick(NPC target);

        public bool CanBeUnstuck { get; }

        public void OnUnstick(NPC target, Projectile Replacing);

        public int MaxStuckProjectiles { get; }

        public bool DealsDamageWhileStuck { get; }

        public NPC.HitInfo StuckDamageInfo { get; }


    }

    public class StickyProjectileGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        NPC StuckNPC;

        public int StickTime;
        public override void AI(Projectile projectile)
        {
            

            

            if (projectile.ModProjectile is IStickyProjectile sticky)
            {
                if (sticky.IsStickingToTarget)
                {
                    projectile.ignoreWater = true;
                    projectile.tileCollide = false;

                    if (StuckNPC != null && StuckNPC.active)
                    {
                        projectile.Center = StuckNPC.Center - projectile.velocity * 2f;
                        projectile.gfxOffY = StuckNPC.gfxOffY;

                        if (sticky.DuringStick != null)
                        {
                            sticky.DuringStick(StuckNPC);
                        }
                        if (sticky.DealsDamageWhileStuck)
                        {
                            StuckNPC.HitEffect(sticky.StuckDamageInfo.HitDirection, sticky.StuckDamageInfo.Damage);
                        }
                    }
                    else
                    {
                        if (sticky.CanBeUnstuck)
                        {
                            sticky.OnUnstick(StuckNPC, projectile);
                            StuckNPC = null;
                        }
                        else
                        {
                            projectile.Kill();
                        }
                    }
                }
            }
        }

        bool flag1 = false;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            List<Projectile> stuckProjectiles = new();
            if (projectile.ModProjectile is IStickyProjectile sticky)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (!proj.active)
                        continue;

                    if (proj.type != projectile.type)
                        continue;

                    if (proj.ModProjectile is not IStickyProjectile stickyProj)
                        continue;

                    if (!stickyProj.IsStickingToTarget)
                        continue;

                    StickyProjectileGlobal global = proj.GetGlobalProjectile<StickyProjectileGlobal>();

                    if (global.StuckNPC == target)
                    {
                        stuckProjectiles.Add(proj);
                    }
                }

                if (stuckProjectiles.Count < sticky.MaxStuckProjectiles)
                {
                    if (sticky.CanStickToTargets)
                    {
                        StuckNPC = target;
                        if (sticky.OnStickToTarget != null && !flag1)
                        {
                            sticky.OnStickToTarget(target);
                            flag1 = true;
                        }
                        StickTime = (int)Main.GameUpdateCount;
                        sticky.IsStickingToTarget = true;
                    }
                }
                else
                {
                    if (sticky.CanBeUnstuck)
                    {
                        Projectile oldest = stuckProjectiles .OrderBy(p => p.GetGlobalProjectile<StickyProjectileGlobal>().StickTime).First();

                        if (oldest.ModProjectile is IStickyProjectile oldSticky)
                        {
                            oldSticky.OnUnstick(target, oldest);
                        }
                        StuckNPC = null;
                        return;
                    }
                    else
                    {
                        projectile.Kill();
                        return;
                    }
                }
            }
        }
    }
}
