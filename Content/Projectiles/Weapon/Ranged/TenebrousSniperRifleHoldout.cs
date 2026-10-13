using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class TenebrousSniperRifleHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D PT = TextureAssets.Projectile[Type].Value;

            int frameHeight = PT.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                PT.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(PT.Width / 2f, frameHeight / 2f);

            SpriteEffects FX = SpriteEffects.None;

            float rot = Projectile.rotation;

            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
            {
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                FX = SpriteEffects.None;
            }

            Main.EntitySpriteDraw(PT, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, FX);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {

        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public Player Owner => Main.player[Projectile.owner];

        public SoundStyle FireSound = new SoundStyle(DTAssetLib.AudioPath + "/TenebrisSniperRifleFire") with { MaxInstances = 0 };

        Vector2 ShootPoint;

        public override void AI()
        {
            Projectile.velocity *= 0;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(90, -10 * Math.Sign(Owner.MountedCenter.DirectionTo(Main.MouseWorld).X)).RotatedBy(Projectile.rotation);

            Projectile.rotation = Owner.MountedCenter.DirectionTo(Main.MouseWorld).ToRotation();
            ShootPoint = Owner.MountedCenter + new Vector2(190, -10 * Math.Sign(Owner.MountedCenter.DirectionTo(Main.MouseWorld).X)).RotatedBy(Projectile.rotation);

            if (Owner.HeldItem.type == ModContent.ItemType<TenebrousSniperRifle>() && Owner.controlUseItem && !Owner.CCed && !Owner.dead)
            {
                Owner.SetDummyItemTime(2);
                Projectile.timeLeft = 60;
                Projectile.ai[0]++;

                if (Projectile.ai[0] == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.5f }, Projectile.Center);
                }
                if (Projectile.ai[0] >= 90)
                {
                    Projectile.ai[0] = 0;
                    if (DTConfig.instance.WeaponKickback)
                    {
                        Owner.velocity += -Owner.MountedCenter.DirectionTo(Main.MouseWorld) * 10f;
                    }
                    SoundEngine.PlaySound(FireSound, Projectile.Center);
                    Fire();
                    Effects();
                }

            }
        }

        private void Effects()
        {
            Vector2 dir = ShootPoint.DirectionTo(Main.MouseWorld);
            for (int i = 0; i < 4; i++)
            {
                PixelParticle Pixel = new();
                Pixel.Initialize(ShootPoint, dir.RotatedByRandom(0.05f) * Main.rand.NextFloat(2f, 7f), ColorLib.TenebrisGradient, 2f);
                ParticleEngine.Particles.Add(Pixel);
            }

            BlossomBeaterFire Flare = new();
            Flare.Initiate(ShootPoint, dir.ToRotation() + MathHelper.PiOver2, ColorLib.TenebrisGradient, 0.5f, 30);
            ParticleEngine.Particles.Add(Flare);
        }

        private void Fire()
        {
            Vector2 dir = ShootPoint.DirectionTo(Main.MouseWorld);

            if (CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item S))
            {
                projToShoot = Owner.FindAmmoDT(AmmoID.Bullet).shoot;


                if (S != null)
                {
                    var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(S, usedAmmoItemId, "TSRFire");

                    Projectile Shot1 = Projectile.NewProjectileDirect(Source, ShootPoint, dir * speed, projToShoot, damage, knockBack, Owner.whoAmI);
                    
                }
            }



        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Spray)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is TenebrousSniperRifle S)
                {
                    Spray = S.Item;
                    if (player.PickAmmo(S.Item, out projToShoot, out speed, out damage, out knockBack, out usedAmmoItemId))
                    {
                        return true;
                    }
                }
            }

            projToShoot = -1;
            speed = 0f;
            damage = 0;
            knockBack = 0f;
            usedAmmoItemId = -1;
            Spray = null;

            return false;
        }

    }

    public class TSRExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/TenebrisSniperRifleExplosion"), Projectile.Center);

            FireRing fire1 = new();
            fire1.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.01f, 0.001f, 0.12f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire1);

            FireRing fire2 = new();
            fire2.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.01f, 0.001f, 0.11f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire2);

            FireRing fire3 = new();
            fire3.Prepare(Projectile.Center, Vector2.Zero, ColorLib.TenebrisGradient, 0.01f, 0.001f, 0.1f, BlendState.Additive);
            ParticleEngine.Particles.Add(fire3);
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 5 == 0)
            {
                Vector2 Outer = Projectile.Center + new Vector2(Main.rand.NextFloat(-500, 500), Main.rand.NextBool() ? 1000 : -1000);

                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/TenebrisSniperRifleFire") { Pitch = 1.6f, PitchVariance = 0.1f, MaxInstances = 0 }, Outer);

                FireRing fire1 = new();
                fire1.Prepare(Outer, Vector2.Zero, ColorLib.TenebrisGradient, 0.01f, 0.0005f, 0.1f, BlendState.Additive);
                ParticleEngine.Particles.Add(fire1);

                Projectile clone = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Outer, Outer.DirectionTo(Projectile.Center) * 18f, (int)Projectile.ai[1], (int)(Projectile.damage * 0.5f), 10, Projectile.owner);
                clone.tileCollide = false;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShimmeringFlames.ShimmerBurn(target, false);
        }
    }

    public class TSRGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;


        bool Explodes = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source != null && source.Context == "TSRFire")
            {
                Explodes = true;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Explodes)
            {

                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<TSRExplosion>(), (int)(projectile.damage * 0.5f), 10, projectile.owner, ai1: projectile.type);
            }
        }
    }
}
