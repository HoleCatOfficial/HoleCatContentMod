using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.UI.StatSheetUI;
using static FargowiltasSouls.FargoSoulsSets;
namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{

    public class EyeSwordProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;
        }

 

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor));
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;

        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] != 0 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit18, target.Center);
            target.AddBuff(BuffID.Bleeding, 240);

            if (!target.active)
            {
                SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerChargeScream with { PitchVariance = 0.2f, MaxInstances = 0 });
            }
            

        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(ModContent.BuffType<EyeSwordBuff>());
            }

            if (Owner.HasBuff(ModContent.BuffType<EyeSwordBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            int targetindex = Projectile.AutoTarget();

            if (targetindex == -1)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;
                Idle();
                return;
            }

            NPC Target = Main.npc[targetindex];
            Projectile.ai[0] = 1;
            Attack(Target);
        }

        public void Idle()
        {


            var organizer = Owner.GetModPlayer<EyeSwordOrganizer>();
            List<Projectile> siblings = organizer.Siblings;

            int myIndex = siblings.IndexOf(Projectile);

            if (myIndex < 0)
                return;

            side = myIndex % 2 == 0 ? 1 : -1;
            float phase = myIndex * 0.6f;
            float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + phase) * 5f;

            Projectile.rotation = MathHelper.PiOver4 + MathHelper.PiOver2;

            Vector2 targetPosition = Owner.MountedCenter + new Vector2((-10 * Owner.direction) + (-20 * myIndex * Owner.direction), bob);

            if (Projectile.Distance(targetPosition) < 2000)
            {
                Projectile.SmoothMoveToPoint(targetPosition, 30f, 90);
            }
            else
            {
                Projectile.Center = targetPosition;
            }


        }

        int side = -1;
        int dis = 0;
        Vector2 RememberedDirection = Vector2.Zero;

        public void Attack(NPC target)
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] < 30)
            {

                if (Projectile.ai[2] == 1)
                {
                    side = (side == 1) ? -1 : 1;
                    dis = 100;
                }

                Projectile.rotation = -MathHelper.PiOver4 + (0.2f * side);
                Projectile.SmoothMoveToPoint(target.Center + new Vector2(dis * side, 0), 30f, 50);
                RememberedDirection = Projectile.Center.DirectionTo(target.Center);
            }
            if (Projectile.ai[2] > 30 && Projectile.ai[2] < 40)
            {
                if (Projectile.ai[2] == 31)
                {
                    Projectile.velocity *= 0;
                    SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerHurt with { PitchVariance = 0.2f, MaxInstances = 0 });
                    SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.5f, PitchVariance = 0.2f, MaxInstances = 0 });
                }

                Projectile.rotation = -MathHelper.PiOver4;
                Projectile.velocity = RememberedDirection * 30f;


            }
            if (Projectile.ai[2] > 40 && Projectile.ai[2] < 50)
            {
                Projectile.rotation = -MathHelper.PiOver4 + (-0.2f * side);
                Projectile.velocity *= 0.5f;
            }
            if (Projectile.ai[2] > 50)
            {
                Projectile.ai[2] = 0;
            }
        }

    }

    public class EyeSwordOrganizer : ModPlayer
    {
        public List<Vector2> SiblingPositions = new();
        public List<Projectile> Siblings => Main.projectile.Where(n => n.type == ModContent.ProjectileType<EyeSwordProjectile>() && n.owner == Player.whoAmI && n.ai[0] == 0 && n.active).ToList();
        public override void PostUpdateBuffs()
        {

            int siblingCount = Siblings.Count;


        }
    }
}