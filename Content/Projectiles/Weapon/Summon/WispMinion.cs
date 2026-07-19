using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using SteelSeries.GameSense;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class WispMinion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 0.5f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ManualCanHitFriendly(target);
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = i / (float)Projectile.oldPos.Length;
                float scale = MathHelper.Lerp(Projectile.scale, 0.0005f, progress);

                Main.EntitySpriteDraw(
                    DTAssetLib.TinyBloom.Value,
                    Projectile.OldCenter()[i] - Main.screenPosition,
                    null,
                    ColorLib.LifeEcho with { A = 0 },
                    Projectile.rotation,
                    DTAssetLib.TinyBloom.Value.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, Projectile.Center - Main.screenPosition, null, ColorLib.LifeEcho with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None, 0f);
            return false;
        }

        int TargetIndex = -1;
        public override void AI()
        {
            if (Owner.HasBuff<WispMinionBuff>())
            {
                TargetIndex = Projectile.AutoTarget();

                if (TargetIndex < 0)
                {
                    IdleAI();
                }
                else
                {
                    AttackAI();
                }
            }
        }

        Player Owner => Main.player[Projectile.owner];

        float AmplitudeX = 50f;
        float AmplitudeY = 35f;
        float Frequency = Main.rand.NextFloat(0.05f, 0.2f);
        float Phase = Main.rand.NextFloat(1f, 10f);

        void IdleAI()
        {
            Projectile.timeLeft = 2;
            Vector2 Base = Owner.MountedCenter;

            float t = Main.GameUpdateCount * Frequency;

            Vector2 offset = new(
                MathF.Sin(t + Phase) * AmplitudeX,
                MathF.Cos(t * 0.8f + Phase) * AmplitudeY
            );

            Vector2 Ideal = Base + offset;


            Projectile.SmoothMoveToPoint(Ideal, 10f);
        }

        bool DashFlag = false;
        int Interval = 60;
        float off = MathHelper.TwoPi;
        void AttackAI()
        {
            Projectile.timeLeft = 2;
            NPC Target = Main.npc[TargetIndex];
            Vector2 Targ = Target.Center + new Vector2(100, 0).RotatedBy(off);

            Dust.NewDustPerfect(Targ, DustID.WhiteTorch).noGravity = true;

            if (Target.active)
            {

                if (Projectile.Distance(Target.Center) < 100)
                {
                    Vector2 ToNPC = Target.Center - Projectile.Center;
                    ToNPC.Normalize();

                    if (!DashFlag)
                    {
                        off = Main.rand.NextFloat(MathHelper.TwoPi);
                        Interval = Main.rand.Next(4, 11) * 10;
                        Projectile.localAI[0] = 0;
                        Projectile.velocity += ToNPC * 10;
                        SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                        DashFlag = true;
                    }
                }

                if (DashFlag)
                {
                    Projectile.rotation += 0.1f * Projectile.direction;
                    if (Projectile.localAI[0] < Interval)
                    {
                        Projectile.localAI[0]++;
                        Projectile.velocity *= 0.95f;
                    }
                    else
                    {
                        DashFlag = false;
                    }
                }
                else
                {
                    Projectile.rotation += 0.1f * Projectile.direction;
                    Projectile.SmoothMoveToPoint(Targ, 10f);

                    if (Projectile.Distance(Targ) < 1f || !Target.Hitbox.IntersectsConeFastInaccurate(Projectile.Center, 100f, (Targ - Projectile.Center).ToRotation(), 10f))
                    {
                        off = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                }
                
            }
        }

    }
}
