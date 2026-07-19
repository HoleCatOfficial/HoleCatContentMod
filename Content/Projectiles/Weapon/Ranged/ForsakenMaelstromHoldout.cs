using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
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
    public class ForsakenMaelstromHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
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
            overPlayers.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public Player Owner => Main.player[Projectile.owner];

        public SoundStyle FireSound = SoundID.DD2_BetsyFlameBreath with { Pitch = -0.7f };

        public bool[] Level = new bool[3];

        private bool[] soundflag = new bool[3];
        private bool fireFlag = false;

        public override void AI()
        {
            Vector2 dir = Main.MouseWorld - Projectile.Center;
            dir.Normalize();
            Projectile.rotation = dir.ToRotation();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dir.ToRotation() - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(15, 0).RotatedBy(Projectile.rotation);


            if (Owner.HeldItem.type == ModContent.ItemType<ForsakenMaelstrom>() && Owner.controlUseItem)
            {
                Owner.SetDummyItemTime(2);
                Projectile.timeLeft = 60;
                Projectile.ai[0]++;

                if (Projectile.ai[0] % 6 == 0)
                {
                    SoundEngine.PlaySound(FireSound);
                    Fire();
                }

            }
        }

        private void Fire()
        {
            Vector2 dir = Main.MouseWorld - Projectile.Center;
            dir.Normalize();
            Vector2 Vel = dir * 7;
            if (CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item B))
            {
                projToShoot = Owner.FindAmmoDT(AmmoID.Gel).shoot;

                if (B != null)
                {
                    var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(B, usedAmmoItemId, "ForsakenMaelstromFire");

                    Projectile.ai[2]++;
                    Projectile Shot1 = Projectile.NewProjectileDirect(Source, Projectile.Center, Vel, ModContent.ProjectileType<ForsakenMaelstromFire>(), damage, knockBack, Owner.whoAmI);
                    
                    if (Projectile.ai[2] % 5 == 0)
                    {
                        Projectile ExtraShot1 = Projectile.NewProjectileDirect(Source, Projectile.Center, (Vel * 2f).RotatedBy(-0.1f), ModContent.ProjectileType<ForsakenMaelstromHomingFireball>(), damage, knockBack, Owner.whoAmI);
                        Projectile ExtraShot2 = Projectile.NewProjectileDirect(Source, Projectile.Center, Vel * 2f, ModContent.ProjectileType<ForsakenMaelstromHomingFireball>(), damage, knockBack, Owner.whoAmI);
                        Projectile ExtraShot3 = Projectile.NewProjectileDirect(Source, Projectile.Center, (Vel * 2f).RotatedBy(0.1f), ModContent.ProjectileType<ForsakenMaelstromHomingFireball>(), damage, knockBack, Owner.whoAmI);
                    }
                }
            }
        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Beater)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is ForsakenMaelstrom F)
                {
                    Beater = F.Item;
                    if (player.PickAmmo(F.Item, out projToShoot, out speed, out damage, out knockBack, out usedAmmoItemId))
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
            Beater = null;

            return false;
        }

    }
}
