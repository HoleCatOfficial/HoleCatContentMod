using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class SavageSprayHoldout : ModProjectile
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
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0.6f;
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

        public SoundStyle FireSound = SoundID.DeerclopsIceAttack with { Volume = 1.15f };

        Vector2 ShootPoint;

        public override void AI()
        {
            Projectile.velocity *= 0;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);

            Projectile.rotation = Owner.MountedCenter.DirectionTo(Main.MouseWorld).ToRotation();
            ShootPoint = Owner.MountedCenter + new Vector2(100, 0).RotatedBy(Projectile.rotation);

            if (Owner.HeldItem.type == ModContent.ItemType<SavageSpray>() && Owner.controlUseItem && !Owner.CCed && !Owner.dead)
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
                    SoundEngine.PlaySound(SoundID.Item38, Projectile.Center);
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
                Pixel.Initialize(ShootPoint, dir.RotatedByRandom(0.05f) * Main.rand.NextFloat(2f, 7f), ColorLib.IchorCrystal1, 4f);
                ParticleEngine.Particles.Add(Pixel);
            }

            BlossomBeaterFire Flare = new();
            Flare.Initiate(ShootPoint, dir.ToRotation() + MathHelper.PiOver2, ColorLib.IchorCrystal3, 0.5f, 30);
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
                    var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(S, usedAmmoItemId, "SavageSprayFire");

                    for (int i = 0; i < 5; i++)
                    {
                        Projectile Shot1 = Projectile.NewProjectileDirect(Source, ShootPoint, dir.RotatedByRandom(0.1f) * speed, projToShoot, damage, knockBack, Owner.whoAmI);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Projectile Shot2 = Projectile.NewProjectileDirect(Source, ShootPoint, dir.RotatedByRandom(0.1f) * speed, ModContent.ProjectileType<IchorNodeCrystalFriendly>(), damage, knockBack, Owner.whoAmI);
                    }
                }
            }



        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Spray)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is SavageSpray S)
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
}
