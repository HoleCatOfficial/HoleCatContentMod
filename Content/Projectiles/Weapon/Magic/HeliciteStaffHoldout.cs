using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HeliciteStaffHoldout : ModProjectile
    {
        Asset<Texture2D> Glowmask;

        public override void SetStaticDefaults()
        {
            Glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            else
            {
                origin = new Vector2(0, 0);
                effects = SpriteEffects.FlipVertically;
                rotationOffset = MathHelper.ToRadians(-45f);
            }


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation + rotationOffset), origin, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + rotationOffset), origin, Projectile.scale, effects, 0);


            return false;
        }

        int beamWhoAmI = -1;

        public HeliosBeam BeamProjectile
        {
            get
            {
                if (beamWhoAmI < 0 || beamWhoAmI >= Main.maxProjectiles)
                    return null;

                Projectile proj = Main.projectile[beamWhoAmI];

                if (!proj.active)
                    return null;

                if (proj.type != ModContent.ProjectileType<HeliosBeam>())
                    return null;

                if (proj.owner != Projectile.owner)
                    return null;

                return proj.ModProjectile as HeliosBeam;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 Dir = Main.MouseWorld - Projectile.Center;
            Dir.Normalize();

            var SpawnPT = Dir * 50;

            Projectile Beam = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + Dir, Vector2.Zero, ModContent.ProjectileType<HeliosBeam>(), Projectile.damage, 10, Owner.whoAmI, Projectile.rotation);

            beamWhoAmI = Beam.whoAmI;

            canMana = Owner.CheckMana(100, true, false);
        }

        bool canMana = false;
        public override void AI()
        {

            Projectile.ai[0]++;
            Vector2 Dir = Main.MouseWorld - Projectile.Center;
            Dir.Normalize();


            var PT = new Vector2(80, 0).RotatedBy(Projectile.rotation);

            float TargetRot = Dir.ToRotation();

            Projectile.rotation = Projectile.rotation.AngleLerp(TargetRot, 0.1f);



            Projectile.Center = Owner.Center;

            Projectile.spriteDirection = Math.Sign((Main.MouseWorld - Owner.MountedCenter).X);



            if (BeamProjectile != null)
            {
                BeamProjectile.Projectile.Center = Projectile.Center + PT;
                BeamProjectile.Projectile.ai[0] = Projectile.rotation;
            }

            

            if (Owner.controlUseItem && !Owner.dead && Owner.HeldItem.type == ModContent.ItemType<HeliciteStaff>())
            {
                Owner.SetDummyItemTime(60);
                Projectile.timeLeft = 60;

                if (BeamProjectile == null && canMana)
                {
                    Projectile Beam = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + Dir, Vector2.Zero, ModContent.ProjectileType<HeliosBeam>(), Projectile.damage, 10, Owner.whoAmI, Projectile.rotation);

                    beamWhoAmI = Beam.whoAmI;
                }

                

                if (Projectile.ai[0] % 30 == 0)
                {
                    canMana = Owner.CheckMana(60, true, false);
                }

                if (canMana)
                {
                    if (BeamProjectile != null)
                    {
                        BeamProjectile.GoodBeam = true;
                    }

                    if (Main.GameUpdateCount % 6 == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + PT, Dir.RotatedByRandom(2f) * Main.rand.NextFloat(6f, 12f), ModContent.ProjectileType<HeliosSpark>(), Projectile.damage / 6, 9, Owner.whoAmI);
                    }
                }
                else
                {
                    Color color = OpusColorUtils.MultiLerp(Opus.Sine(1f, 0f), ColorLib.HeliciteColorMap);
                    Dust D = Dust.NewDustPerfect(Projectile.Center + PT, ModContent.DustType<ColorableNeonDust>(), new Vector2(0, -4f), 0, color, 1f);
                    D.noGravity = true;
                    if (Projectile.ai[0] % 30 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item24, Projectile.Center);

                        CombatText.NewText(Utils.CenteredRectangle(Projectile.Center + PT, new Vector2(12, 12)), color, "NO MANA", false);

                        Utils.PoofOfSmoke(Projectile.Center + PT);

                    }

                    if (BeamProjectile != null)
                    {
                        BeamProjectile.GoodBeam = false;
                    }
                }
            }
            else
            {
                if (BeamProjectile != null)
                {
                    BeamProjectile.GoodBeam = false;
                }
            }
        }




        Player Owner => Main.player[Projectile.owner];

        public void SetPosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);

            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            Projectile.scale = 1f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }


    }
}
