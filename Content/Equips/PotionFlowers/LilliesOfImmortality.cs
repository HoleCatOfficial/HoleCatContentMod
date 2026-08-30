using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            if(player.TryGetModPlayer<DjedPillarCharmPlayer>(out DjedPillarCharmPlayer modPlayer))
            {
                modPlayer.Active = true;
            }
            if(player.TryGetModPlayer<LilliesDash>(out LilliesDash Dash))
            {
                //Dash.Active = true;
            }
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EphemeralSolvent>(1)
                .AddIngredient<DjedPillarCharm>(1)
                .AddIngredient<Tenebris>(6)
                .Register();
        }
	}

    public class LilliesDash : ModPlayer
    {

        public bool Active = false;

        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 120; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int DashDuration = 10; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public const float DashVelocity = 70f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 6; // frames remaining in the dash

        public int EntryWindow = 600;



        public override void ResetEffects()
        {
            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
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
            EntryWindow = 600;
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {

            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }

                // start our dash
                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, ModContent.DustType<LilliesDashDust>(), Player.velocity.X * 0.75f, 1f, 0, default, 6f);
                    Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.GoldCoin, Player.velocity.X * 0.75f, 1f, 0, default, 4f);
                }
                SoundEngine.PlaySound(SoundID.Item84 with { Pitch = -0.4f, MaxInstances = 0, PitchVariance = 0.2f });
            }

            if (DashDelay > 0)
            {
                DashDelay--;
            }


            if (DashTimer > 0)
            {
                // dash is active
                // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
                // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
                // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
                Player.immune = true;
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                if (Player.miscCounter % 5 == 0 && Active)
                {
                    Projectile.NewProjectile(Player.GetSource_Misc("DjedPillar"), Main.rand.NextVector2FromRectangle(Player.Hitbox), (Player.velocity * 0.5f).RotatedByRandom(1), ModContent.ProjectileType<TenebrisStarFriendly>(), 25, 8, Player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item118, Player.position);
                }
                DashTimer--;
            }
        }

        private bool CanUseDash()
        {
            return Active
                && !Player.mount.Active
                && EntryWindow <= 0;
        }
    }
}