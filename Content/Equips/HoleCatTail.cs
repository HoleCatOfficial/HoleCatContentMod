using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Back)]
    public class HoleCatTail : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 8;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<HoleCatDash>(out var Dash))
            {
                Dash.Active = true;
            }
        }
    }

    public class HoleCatDash : ModPlayer
    {

        public bool Active = false;

        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 120; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int DashDuration = 35; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public const float DashVelocity = 30f;

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

                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatDash") { MaxInstances = 0, PitchVariance = 0.4f });
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
