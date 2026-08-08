using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike;
using DestroyerTest.Content.SummonItems;
using GlowmaskHelper.Content;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class UnionThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        Player Owner => Main.player[Projectile.owner];
        public float speedfactor => Owner.GetTotalAttackSpeed(DamageClass.Generic);


        public Vector2 a;
        public BezierCurve Path;


        public override void OnSpawn(IEntitySource source)
        {
            a = Main.MouseWorld - Projectile.Center;
            a.Normalize();
            Projectile.velocity = a * (20 + speedfactor);

            
        }

        public int timer = 0;
        public Vector2 toMouse;
        public Vector2 toOwner;
        public float offset = 0;
        public override void AI()
        {
            Projectile.rotation += 0.55f;

            offset += 0.4f;

            timer++;

            toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            toOwner = Owner.Center - Projectile.Center;
            toOwner.Normalize();

          


            float interval = 6f / speedfactor;
            if (timer >= interval)
            {
                timer = 0;
                SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0, PitchVariance = 0.9f }, Projectile.Center);
            }

            if (hitcount < 1)
            {
                Projectile.velocity.Y += 0.3f;
            }
            else
            {
                Projectile.velocity *= 0.94f;
            }
        }

        public int hitcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitcount++;
            target.AddBuff(BuffID.Bleeding, 600);

            SoundEngine.PlaySound(DTAssetLib.SwordSounds.ThinSlice with { MaxInstances = 0, PitchVariance = 0.9f }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled), Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            if (Owner.HeldItem.ModItem is Union union)
            {
                union.CurrentAttack = Union.Attacks.SwingDefault;
            }
        }
    }
}