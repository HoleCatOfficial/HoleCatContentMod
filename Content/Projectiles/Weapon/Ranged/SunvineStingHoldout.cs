using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.SummonItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public class SunvineStingHoldout : ModProjectile
    {
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
            Texture2D Tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects Fx = Math.Sign(Pointing.X) == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale, Fx);
            return false;

        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 Pointing => Main.MouseWorld - Owner.Center;

        SoundStyle Empty = new SoundStyle(DTAssetLib.AudioPath + "/ClickMetal") { PitchVariance = 0.1f, MaxInstances = 0 };
        SoundStyle Fire = new SoundStyle(DTAssetLib.AudioPath + "/BlitzFire") { PitchVariance = 0.1f, MaxInstances = 0 };
        SoundStyle HeavyFire = new SoundStyle(DTAssetLib.AudioPath + "/BlitzHeavyFire") { PitchVariance = 0.1f, MaxInstances = 0 };

        bool HeavyShot = false;

        Vector2 GetMuzzle()
        {
            return Projectile.Center + new Vector2(10, 4 * Math.Sign(Pointing.X)).RotatedBy(Pointing.ToRotation());
        }

        void SetPosition()
        {

            Owner.SetCompositeArmFront(Projectile.active, Player.CompositeArmStretchAmount.Full, Pointing.ToRotation() - MathHelper.PiOver2);
            Projectile.Center = Owner.MountedCenter + new Vector2(24, 0).RotatedBy(Projectile.rotation);
            Projectile.rotation = Pointing.ToRotation();
        }

        public bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }

        public override bool PreAI()
        {
            HeavyShot = JustPressed(Keys.X);
            return true;
        }

        public override void AI()
        {
            SetPosition();


            if (Owner.controlUseItem && !Owner.CCed && !Owner.dead && Owner.HeldItem.type == ModContent.ItemType<Blitz>())
            {
                Projectile.timeLeft = 60;
                if (!HeavyShot)
                {
                    Projectile.ai[0]++;

                    if (Projectile.ai[0] % 3 == 0)
                    {
                        if (Owner.PickAmmo(Owner.HeldItem, out int Shot, out float Speed, out int Dmg, out float KB, out int ammoID))
                        {
                            Vector2 Dir = Pointing;
                            Dir.Normalize();



                            SoundEngine.PlaySound(Fire, GetMuzzle());

                            Projectile PrimaryFire = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse_WithPotentialAmmo(Owner.HeldItem, Owner.FindAmmoDT(AmmoID.Bullet).type), GetMuzzle(), Dir * Speed, Owner.FindAmmoDT(AmmoID.Bullet).shoot, (int)Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(40), KB, Owner.whoAmI);
                        }
                        else
                        {
                            SoundEngine.PlaySound(Empty, GetMuzzle());
                        }
                    }
                }
                else
                {
                    Projectile.ai[1]++;

                    if (Projectile.ai[1] % 40 == 0)
                    {
                        if (Owner.PickAmmo(Owner.HeldItem, out int Shot, out float Speed, out int Dmg, out float KB, out int ammoID))
                        {
                            Vector2 Dir = Pointing;
                            Dir.Normalize();

                            SoundEngine.PlaySound(HeavyFire, GetMuzzle());

                            Utils.PoofOfSmoke(GetMuzzle());

                            Projectile SecondaryFire = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse_WithPotentialAmmo(Owner.HeldItem, Owner.FindAmmoDT(AmmoID.Bullet).type), GetMuzzle(), Dir * Speed, ModContent.ProjectileType<BlitzCrystalBullet>(), (int)Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(40) * 2, KB, Owner.whoAmI);
                        }
                        else
                        {
                            SoundEngine.PlaySound(Empty, GetMuzzle());
                        }
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}