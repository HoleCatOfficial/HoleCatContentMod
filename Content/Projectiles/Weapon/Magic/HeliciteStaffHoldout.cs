using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Projectile.width = Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.netImportant = true;

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
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
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


            var PT = Dir * 110;

            

            Projectile.rotation = Dir.ToRotation();
            Projectile.Center = Owner.Center;



            if (BeamProjectile != null)
            {
                BeamProjectile.Projectile.Center = Projectile.Center + PT;
                BeamProjectile.Projectile.ai[0] = Projectile.rotation;
            }

            

            if (Owner.controlUseItem && !Owner.dead && Owner.HeldItem.type == ModContent.ItemType<HeliciteStaff>())
            {
                Owner.SetDummyItemTime(60);
                Projectile.timeLeft = 120;

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
                    Dust D = Dust.NewDustPerfect(Projectile.Center + PT, ModContent.DustType<ColorableNeonDust>(), new Vector2(0, -4f), 0, ColorLib.Rift, 1f);
                    D.noGravity = true;
                    if (Projectile.ai[0] % 30 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item108, Projectile.Center);

                        CombatText.NewText(Utils.CenteredRectangle(Projectile.Center + PT, new Vector2(12, 12)), ColorLib.Rift, "NO MANA", false);

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
