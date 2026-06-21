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
    public class PureBowHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
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

        public SoundStyle FireSound = new SoundStyle("DestroyerTest/Assets/Audio/PoisonVerseBurst");

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
            

            if (Owner.HeldItem.type == ModContent.ItemType<PureBow>() && Owner.controlUseItem && !fireFlag)
            {
                Owner.SetDummyItemTime(2);
                Projectile.timeLeft = 60;
                Projectile.ai[0]++;

                UpdateLevel();
            }
            else
            {
                if (!fireFlag)
                {
                    Fire();
                    fireFlag = true;
                }
                else
                {
                    Level[0] = false;
                    Level[1] = false;
                    Level[2] = false;
                    Projectile.frame = 0;
                }
            }
        }

        private void UpdateLevel()
        {
            if (Projectile.ai[0] == 60)
            {
                if (!soundflag[0])
                {
                    SoundEngine.PlaySound(DTAssetLib.Charge.MetalTinkLight with { Pitch = 0f }, Projectile.Center);
                    soundflag[0] = true;
                    Projectile.frame = 0;
                }
                Level[0] = true;

                Level[1] = false;
                Level[2] = false;

            }
            if (Projectile.ai[0] == 120)
            {
                if (!soundflag[1])
                {
                    SoundEngine.PlaySound(DTAssetLib.Charge.MetalTinkLight with { Pitch = 0.33f }, Projectile.Center);
                    soundflag[1] = true;
                    Projectile.frame = 1;
                }
                Level[1] = true;

                Level[0] = false;
                Level[2] = false;

            }
            if (Projectile.ai[0] == 180)
            {
                if (!soundflag[2])
                {
                    SoundEngine.PlaySound(DTAssetLib.Charge.MetalTinkLight with { Pitch = 0.66f }, Projectile.Center);
                    soundflag[2] = true;
                    Projectile.frame = 2;
                }
                Level[2] = true;

                Level[0] = false;
                Level[1] = false;
            }
        }

        private void Fire()
        {
            if (Level[0])
            {
                SoundEngine.PlaySound(SoundID.Item25 with { Pitch = 0f }, Projectile.Center);
                Vector2 dir = Main.MouseWorld - Projectile.Center;
                dir.Normalize();
                Vector2 Vel = dir * 16;
                if (CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item B))
                {
                    projToShoot = Owner.FindAmmoDT(AmmoID.Arrow).shoot;

                    if (B != null)
                    {
                        var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(B, usedAmmoItemId, "PureBowFire");



                        Projectile Shot1 = Projectile.NewProjectileDirect(Source, Projectile.Center, Vel, projToShoot, damage, knockBack, Owner.whoAmI);
                    }
                }
            }
            else if (Level[1])
            {
                
                SoundEngine.PlaySound(SoundID.Item25 with { Pitch = 0.25f }, Projectile.Center);

                Vector2 dir = Main.MouseWorld - Projectile.Center;
                dir.Normalize();
                Vector2 Vel = dir * 24;
                if (CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item B))
                {
                    projToShoot = Owner.FindAmmoDT(AmmoID.Arrow).shoot;

                    if (B != null)
                    {
                        var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(B, usedAmmoItemId, "PureBowFire");
                        Projectile Shot2 = Projectile.NewProjectileDirect(Source, Projectile.Center, Vel, projToShoot, damage, knockBack, Owner.whoAmI);
                        Shot2.penetrate = 8;
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    Projectile Crys = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vel.RotatedByRandom(0.1f), ModContent.ProjectileType<BlessedNodeCrystalFriendly>(), damage / 2, knockBack, Owner.whoAmI);
                    Crys.penetrate = 8;
                }
            }
            else if (Level[2])
            {
                SoundEngine.PlaySound(FireSound with { Pitch = 0f }, Projectile.Center);

                Vector2 dir = Main.MouseWorld - Projectile.Center;
                dir.Normalize();
                Vector2 Vel = dir * 32;
                if(CheckAmmoForConsumption(Owner, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item B))
                {
                    projToShoot = Owner.FindAmmoDT(AmmoID.Arrow).shoot;

                    if (B != null)
                    {
                        var Source = Owner.GetSource_ItemUse_WithPotentialAmmo(B, usedAmmoItemId, "PureBowFire");
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile Shot3 = Projectile.NewProjectileDirect(Source, Projectile.Center, Vel.RotatedByRandom(0.1f), projToShoot, damage, knockBack, Owner.whoAmI);
                            Shot3.penetrate = -1;
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    Projectile Crys2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vel.RotatedByRandom(0.1f), ModContent.ProjectileType<BlessedNodeCrystalFriendly>(), damage / 2, knockBack, Owner.whoAmI);
                    Crys2.penetrate = -1;
                }

                for (int j = 0; j < 2; j++)
                {
                    Projectile Star = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vel.RotatedByRandom(0.1f), ModContent.ProjectileType<ContinuumStar>(), damage / 2, knockBack, Owner.whoAmI);
                }
            }
        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Beater)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is PureBow B)
                {
                    Beater = B.Item;
                    if (player.PickAmmo(B.Item, out projToShoot, out speed, out damage, out knockBack, out usedAmmoItemId))
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
