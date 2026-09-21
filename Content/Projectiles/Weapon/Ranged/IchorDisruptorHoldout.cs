using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
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
    public class IchorDisruptorHoldout : ModProjectile
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
                Projectile.direction = -1;
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                Projectile.direction = 1;
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

        public SoundStyle FireSound = new SoundStyle(DTAssetLib.AudioPath + "/OB_Shot");

        Vector2 ShootPoint;

        public override void AI()
        {
            Projectile.velocity *= 0;
            Vector2 dir = Main.MouseWorld - Projectile.Center;
            dir.Normalize();
            Projectile.rotation = dir.ToRotation();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dir.ToRotation() - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);
            ShootPoint = Projectile.Center + new Vector2(70, -10 * Projectile.direction).RotatedBy(Projectile.rotation);

            if (Owner.HeldItem.type == ModContent.ItemType<IchorDisruptor>() && Owner.controlUseItem && !Owner.CCed && !Owner.dead)
            {
                Owner.SetDummyItemTime(2);
                Projectile.timeLeft = 60;
                Projectile.ai[0]++;

                if (Projectile.ai[0] == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.5f }, Projectile.Center);
                }
                if (Projectile.ai[0] >= 120)
                {
                    Projectile.ai[0] = 0;
                    if (DTConfig.instance.WeaponKickback)
                    {
                        Owner.velocity += -dir * 5f;
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

            BlossomBeaterFire Flare = new();
            Flare.Initiate(ShootPoint, dir.ToRotation() + MathHelper.PiOver2, ColorLib.IchorCrystal1, 0.2f, 30);
            ParticleEngine.Particles.Add(Flare);
        }

        private void Fire()
        {
            Vector2 dir = ShootPoint.DirectionTo(Main.MouseWorld);

            if (CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item D))
            {
                projToShoot = Owner.FindAmmoDT(AmmoID.Bullet).shoot;


                if (D != null)
                {
                    var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(D, usedAmmoItemId, "IchorDisruptorFire");

                    for (int i = 0; i < 3; i++)
                    {
                        Projectile Shot1 = Projectile.NewProjectileDirect(Source, ShootPoint, dir.RotatedByRandom(0.1f) * speed * 1000f, projToShoot, damage, knockBack, Owner.whoAmI);
                    }

                    Projectile Shot2 = Projectile.NewProjectileDirect(Source, ShootPoint, dir * speed * 1000f, ProjectileID.GoldenShowerFriendly, damage, knockBack, Owner.whoAmI);
                    
                }
            }



        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Disruptor)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is IchorDisruptor D)
                {
                    Disruptor = D.Item;
                    if (player.PickAmmo(D.Item, out projToShoot, out speed, out damage, out knockBack, out usedAmmoItemId))
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
            Disruptor = null;

            return false;
        }

    }
}
