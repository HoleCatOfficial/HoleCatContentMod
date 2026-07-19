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

        public float swayRotation;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = TextureAssets.Projectile[Type].Value;

            Texture2D VineTex1 = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/SunvineStingVine1").Value;
            Texture2D VineTex2 = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/SunvineStingVine2").Value;

            SpriteEffects Fx = Math.Sign(Pointing.X) == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale, Fx);

            Main.EntitySpriteDraw(VineTex1, (Projectile.Center + new Vector2(-7f, -17f).RotatedBy(Projectile.rotation)) - Main.screenPosition, null, Color.White, 0f, new Vector2(VineTex1.Width / 2, 0f), Projectile.scale, Fx);
            Main.EntitySpriteDraw(VineTex2, (Projectile.Center + new Vector2(-20.5f, -20f).RotatedBy(Projectile.rotation)) - Main.screenPosition, null, Color.White, 0f, new Vector2(VineTex2.Width / 2, 0f), Projectile.scale, Fx);

            return false;

        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 Pointing => Main.MouseWorld - Owner.Center;

        SoundStyle Fire = SoundID.Item5;

        Vector2 GetMuzzle()
        {
            return Projectile.Center + new Vector2(4, 4).RotatedBy(Pointing.ToRotation());
        }

        void SetPosition()
        {

            Owner.SetCompositeArmFront(Projectile.active, Player.CompositeArmStretchAmount.Full, Pointing.ToRotation() - MathHelper.PiOver2);
            Projectile.Center = Owner.MountedCenter + new Vector2(20, 0).RotatedBy(Projectile.rotation);
            Projectile.rotation = Pointing.ToRotation();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            SetPosition();


            if (Owner.controlUseItem && !Owner.CCed && !Owner.dead && Owner.HeldItem.type == ModContent.ItemType<SunvineSting>())
            {
                Projectile.timeLeft = 60;
               
                Projectile.ai[0]++;
                int interval = (int)(15 * Owner.GetAttackSpeed(DamageClass.Ranged));

                if (Projectile.ai[0] % interval == 0)
                {
                    if (Owner.PickAmmo(Owner.HeldItem, out int Shot, out float Speed, out int Dmg, out float KB, out int ammoID))
                    {
                        Vector2 Dir = Pointing;
                        Dir.Normalize();



                        SoundEngine.PlaySound(Fire, GetMuzzle());

                        Projectile PrimaryFire = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse_WithPotentialAmmo(Owner.HeldItem, Owner.FindAmmoDT(AmmoID.Arrow).type), GetMuzzle(), Dir * Speed, ModContent.ProjectileType<SunvineStingArrow>(), (int)Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(40), KB, Owner.whoAmI);

                        if (Owner.HeldItem.ModItem is SunvineSting Sting)
                        {
                            if (Sting.HitCount >= 20)
                            {
                                SoundEngine.PlaySound(SoundID.Zombie104, GetMuzzle());
                                Projectile SecondaryFire = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse_WithPotentialAmmo(Owner.HeldItem, Owner.FindAmmoDT(AmmoID.Arrow).type), GetMuzzle(), Dir * 0.01f, ModContent.ProjectileType<SunvineStingBeam>(), (int)Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(180), KB, Owner.whoAmI, Dir.ToRotation());
                                Sting.HitCount = 0;
                            }
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