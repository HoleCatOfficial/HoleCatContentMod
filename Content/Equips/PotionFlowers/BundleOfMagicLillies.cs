using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class BundleOfMagicLillies : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 86;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<LilliesDash>(out LilliesDash Dash))
            {
                Dash.Active = true;
            }
        }
    }

    public class LilliesDash : ModPlayer
    {

        public bool Active = false;

        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int ImmunityDuration = 10;
        public int DashCooldown = 30; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 10; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public float DashVelocity = 30f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 10; // frames remaining in the dash

        public int ImmunityTimer = 0;

        public int EntryWindow = 120;

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            ImmunityDuration = 10;
            if (DashTimer > 0)
            {

            }
        }

        List<Vector2> Positions = new();

        public override void ResetEffects()
        {
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && Active)
            {
                DashDir = DashRight;
            }

            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && Active)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }

            if (EntryWindow > 0)
            {
                EntryWindow--;
            }

            Active = false;
        }

        public override void OnEnterWorld()
        {
            EntryWindow = 120;
        }
        public override void PreUpdateMovement()
        {

            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                ImmunityTimer = ImmunityDuration;

                Player.velocity = newVelocity;


                BloomRingSharp Ring = new();
                Ring.Prepare(Player.Center, Vector2.Zero, new Color(182, 82, 240), 0.1f, 0.01f, 0.7f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring);

                BloomRingSharp Ring2 = new();
                Ring2.Prepare(Player.Center, Vector2.Zero, new Color(182, 82, 240), 0.03f, 0.01f, 0.5f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring2);

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Player.position);

                for (int i = 0; i < 10; i++)
                {


                }
            }

            if (ImmunityTimer > 0)
            {

                ImmunityTimer--;
            }


            if (DashDelay > 0)
            {
                DashDelay--;

                if (DashDelay == 1)
                {
                    Positions.Clear();
                }
            }



            if (DashTimer > 0)
            {
                Positions.Add(Player.MountedCenter);

                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;


                if (Active)
                {
                  
                    Spark spark = new();
                    spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity * 0.5f, -Player.velocity.ToRotation() + MathHelper.PiOver2, new Color(182, 82, 240), 0.5f, false, 20, SparkDrawMode.Additive, 2f);
                    ParticleEngine.Particles.Add(spark);

                    Spark spark2 = new();
                    spark2.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity * 0.3f, -Player.velocity.ToRotation() + MathHelper.PiOver2, new Color(182, 82, 240), 0.2f, false, 20, SparkDrawMode.Additive, 3f);
                    ParticleEngine.Particles.Add(spark2);

                }
                DashTimer--;

                if (DashTimer == 1)
                {
                    Player.velocity *= 0.1f;


                    SoundEngine.PlaySound(SoundID.LiquidsWaterLava, Player.position);

                }
            }
        }

        private bool CanUseDash()
        {
            return Active
                && !Player.mount.Active
                && EntryWindow <= 0;
        }


        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {

        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {

        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return ImmunityTimer <= 0;
        }
    }
}
