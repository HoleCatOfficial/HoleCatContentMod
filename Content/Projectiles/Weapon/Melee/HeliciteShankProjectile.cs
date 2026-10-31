using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class HeliciteShankProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
            ProjectileID.Sets.CanDistortWater[Type] = false;
        }

        float Mult = 1f;
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Mult = 1.3f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, (Projectile.Center + new Vector2(50f, 4 * Owner.direction).RotatedBy(Projectile.rotation - MathHelper.PiOver4)) - Main.screenPosition, null, color with { A = 0 }, Projectile.rotation - MathHelper.PiOver4, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.25f, 0.2f) * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, (Projectile.Center + new Vector2(50f, 4 * Owner.direction).RotatedBy(Projectile.rotation - MathHelper.PiOver4)) - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation - MathHelper.PiOver4, DTAssetLib.SparkSmoothThin.Value.Size() / 2, new Vector2(0.25f, 0.2f) * Projectile.scale * 0.6f, SpriteEffects.None);

            SpriteEffects Fx = Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float RotOffset = Owner.direction == 1 ? 0f : (MathHelper.PiOver2);
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor) with { effect = Fx, rotation = Projectile.rotation + RotOffset });
            return false;
        }
        int time = 0;
        int MaxTime => (int)(7 * Owner.GetAttackSpeed<DTTrueMeleeClass>());
        float progress => (float)time / (float)MaxTime;
        float visualProgress => 1f - MathF.Pow(2f, -7f * Owner.GetAttackSpeed<DTTrueMeleeClass>() * progress);
        Vector2 direction;
        Vector2 EndPoint;
        float EndPointRotation;
        public override void OnSpawn(IEntitySource source)
        {
            direction = Owner.MountedCenter.DirectionTo(Main.MouseWorld);
            EndPointRotation = direction.ToRotation() + Main.rand.NextFloat(-0.3f, 0.3f);
            EndPoint = Owner.MountedCenter + new Vector2(50 * Projectile.scale * Owner.GetAttackSpeed<DTTrueMeleeClass>(), 0).RotatedBy(EndPointRotation);
        }

        public override void AI()
        {
            time++;

            Mult = MathHelper.Lerp(1.3f, 1f, progress);
            Projectile.scale = Mult * Owner.GetAdjustedItemScale(Owner.HeldItem);

            EndPoint = Owner.MountedCenter + new Vector2(40 * Projectile.scale, 0).RotatedBy(EndPointRotation);
            Projectile.rotation = (Owner.MountedCenter.DirectionTo(EndPoint)).ToRotation() + MathHelper.PiOver4;

            Projectile.Center = Vector2.Lerp(Owner.MountedCenter, EndPoint, visualProgress);

            Lighting.AddLight(Projectile.Center, OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap).ToVector3());

            if (progress >= 1f)
            {
                Projectile.Kill();
            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale * 1.7f);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 40f * Projectile.scale, ref collisionPoint);
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly /*&& !OpusNPCDropHelper.IgnoreEnemies.Contains(target.type)*/;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, target.Center);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, direction * 10f, ModContent.ProjectileType<SunspotSpark>(), Projectile.damage, 4f, Owner.whoAmI);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X - Owner.Center.X) > 0 ? 1 : -1;
        }

    }
}
