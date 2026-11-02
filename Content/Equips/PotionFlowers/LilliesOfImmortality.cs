using System;
using System.Collections.Generic;
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
    public class LilliesOfImmortality : ModItem
    {
        List<int> blocked;
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
            blocked = [ItemID.AnkhCharm, ItemID.AnkhShield, ItemID.BandofRegeneration, ItemID.CharmofMyths];


            if (DTCrossMod.FargosSoulsIsLoaded)
            {
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("ConcentratedRainbowMatter", out ModItem CRM))
                {
                    blocked.Add(CRM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("BionomicCluster", out ModItem BC))
                {
                    blocked.Add(BC.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("MasochistSoul", out ModItem SM))
                {
                    blocked.Add(SM.Type);
                }
                if (DTCrossMod.FargosSoulsMod.TryFind<ModItem>("EternitySoul", out ModItem SE))
                {
                    blocked.Add(SE.Type);
                }
            }

            DTUtils.IncompatibleWith(Type, blocked.ToArray());
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 98;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<PotionFlowerPlayer>(out PotionFlowerPlayer flower))
            {
                flower.Lillies = true;
            }
            if (player.TryGetModPlayer<DjedPillarCharmPlayer>(out DjedPillarCharmPlayer modPlayer))
            {
                modPlayer.Active = true;
            }
            if (player.TryGetModPlayer<ShadeLilliesDash>(out ShadeLilliesDash Dash))
            {
                Dash.Active = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EphemeralSolvent>(1)
                .AddIngredient<DjedPillarCharm>(1)
                .AddIngredient<BundleOfMagicLillies>(1)
                .AddIngredient<Tenebris>(6)
                .Register();
        }
    }

    public class ShadeLilliesDash : ModPlayer
    {

        public bool Active = false;

        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int ImmunityDuration = 10;
        public int DashCooldown = 30; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 10; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public float DashVelocity = 70f;

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

        Color Choice = ColorLib.TenebrisBeige;

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
                Choice = Main.rand.NextFromCollection([ColorLib.TenebrisBeige, ColorLib.TenebrisBlue, ColorLib.TenebrisMagenta]);

                Player.velocity = newVelocity;


                BloomRingSharp Ring = new();
                Ring.Prepare(Player.Center, Vector2.Zero, Choice, 0.1f, 0.01f, 0.7f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring);

                BloomRingSharp Ring2 = new();
                Ring2.Prepare(Player.Center, Vector2.Zero, Choice, 0.03f, 0.01f, 0.5f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring2);

                ImpactCracks cracks = new();
                cracks.Prepare(Player.MountedCenter, Choice, 1.7f);
                ParticleEngine.BehindProjectiles.Add(cracks);

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Player.position);
                SoundEngine.PlaySound(DTAssetLib.Impacts.HeavyCrit with { Pitch = -0.6f, PitchVariance = 0.3f }, Player.position);
                SoundEngine.PlaySound(DTAssetLib.Impacts.HeatseekerSilohSlam with { Pitch = -0.6f, PitchVariance = 0.3f }, Player.position);

                for (int i = 0; i < 10; i++)
                {
                    PixelParticle pixel = new();
                    pixel.Initialize(Main.rand.NextVector2FromRectangle(Player.Hitbox), Player.velocity.RotatedByRandom(0.3f) * 0.05f, Choice, 2f);
                    ParticleEngine.Particles.Add(pixel);

                    PixelParticle pixel2 = new();
                    pixel2.Initialize(Player.MountedCenter, Main.rand.NextVector2Circular(2f, 2f), Choice, 2f);
                    ParticleEngine.Particles.Add(pixel2);

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
                    for (int i = 0; i < Positions.Count; i++)
                    {
                        Spark trail1 = new();
                        trail1.PrepareSpark(Positions[i] + new Vector2(0, -20), -Player.velocity.RotatedBy(0.1f * Math.Sign(Player.velocity.X)) * 0.2f, -Player.velocity.ToRotation() + MathHelper.PiOver2, Choice * 0.5f, 0.5f, false, 10, SparkDrawMode.Additive, 3f);
                        ParticleEngine.Particles.Add(trail1);

                        Spark trail2 = new();
                        trail2.PrepareSpark(Positions[i] + new Vector2(0, 20), -Player.velocity.RotatedBy(-0.1f * Math.Sign(Player.velocity.X)) * 0.2f, -Player.velocity.ToRotation() + MathHelper.PiOver2, Choice * 0.5f, 0.5f, false, 10, SparkDrawMode.Additive, 3f);
                        ParticleEngine.Particles.Add(trail2);
                    }

                    Spark spark = new();
                    spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity * 0.5f, -Player.velocity.ToRotation() + MathHelper.PiOver2, Choice, 0.5f, false, 20, SparkDrawMode.Additive, 2f);
                    ParticleEngine.Particles.Add(spark);

                    Spark spark2 = new();
                    spark2.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity * 0.3f, -Player.velocity.ToRotation() + MathHelper.PiOver2, Choice, 0.2f, false, 20, SparkDrawMode.Additive, 3f);
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