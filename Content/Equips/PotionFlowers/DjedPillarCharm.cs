
using System.Linq;
using System.Security.AccessControl;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles;
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
    public class DjedPillarCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 96;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if(player.TryGetModPlayer<DjedPillarCharmPlayer>(out DjedPillarCharmPlayer modPlayer))
            {
                modPlayer.Active = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AnkhCharm, 1)
                .AddIngredient<RiftenOverloader>(1)
                .AddIngredient(ItemID.SpiritFlame, 1)
                .AddIngredient(ItemID.OmegaBanner, 1)
                .AddIngredient(ItemID.AnkhBanner, 1)
                .AddIngredient(ItemID.SnakeBanner, 1)
                .AddTile(TileID.LihzahrdAltar)
                .Register();
        }
    }

    public class DjedPillarCharmPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.Darkness] = true;
                Player.buffImmune[BuffID.Cursed] = true;
                Player.buffImmune[BuffID.OnFire] = true;
                Player.buffImmune[BuffID.Bleeding] = true;
                Player.buffImmune[BuffID.Confused] = true;
                Player.buffImmune[BuffID.Slow] = true;
                Player.buffImmune[BuffID.Weak] = true;
                Player.buffImmune[BuffID.Silenced] = true;
                Player.buffImmune[BuffID.BrokenArmor] = true;
                Player.buffImmune[BuffID.CursedInferno] = true;
                Player.buffImmune[BuffID.Frostburn] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Burning] = true;
                Player.buffImmune[BuffID.Ichor] = true;
                Player.buffImmune[BuffID.Venom] = true;
                Player.buffImmune[BuffID.Blackout] = true;
                Player.buffImmune[BuffID.Electrified] = true;
                Player.buffImmune[BuffID.Rabies] = true;
                Player.buffImmune[BuffID.ShadowFlame] = true;
                Player.buffImmune[ModContent.BuffType<Brine>()] = true;
                Player.buffImmune[ModContent.BuffType<GalantineBurn>()] = true;
                Player.buffImmune[ModContent.BuffType<HeliouricShock>()] = true;
                Player.buffImmune[ModContent.BuffType<Muddy>()] = true;
            }
        }
    }

    public class SpiritFlameDash : ModPlayer
    {
        bool HasDjedPillarEquipped()
        {
            for (int i = 3; i < Player.armor.Length; i++)
            {
                if (Player.armor[i].type == ModContent.ItemType<DjedPillarCharm>() || Player.armor[i].type == ModContent.ItemType<LilliesOfImmortality>())
                {
                    return true;
                }
            }
            return false;
        }
        // These indicate what direction is what in the timer arrays used
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int DashDuration = 35; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public const float DashVelocity = 30f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public bool DashAccessoryEquipped;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 6; // frames remaining in the dash

        public override void ResetEffects()
        {
            // Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
            DashAccessoryEquipped = true;

            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 && HasDjedPillarEquipped())
            {
                DashDir = DashRight;
            }

            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 && HasDjedPillarEquipped())
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
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
                    Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.DemonTorch, Player.velocity.X * 0.75f, 1f, 0, default, 6f);
                    Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.GoldCoin, Player.velocity.X * 0.75f, 1f, 0, default, 4f);
                }
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/DjedDash"));
            }

            if (DashDelay > 0)
                DashDelay--;


            if (DashTimer > 0)
            { // dash is active
              // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
              // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
              // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;
                if (Main.GameUpdateCount % 5 == 0)
                {
                    Item Pillar = Player.armor.FirstOrDefault(item => item.type == ModContent.ItemType<DjedPillarCharm>());
                    Item Lillies = Player.armor.FirstOrDefault(item => item.type == ModContent.ItemType<LilliesOfImmortality>());
                    if (Pillar != null || Lillies != null)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(Pillar), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ModContent.ProjectileType<TenebrisFlamesFriendly_NoHoming>(), 30, 8, Main.LocalPlayer.whoAmI);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Player.position);
                    }

                }
                DashTimer--;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == DashID.None // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                && HasDjedPillarEquipped()
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }

        private Asset<Texture2D> Djed => ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/DjedDash");
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.direction == 1 && DashTimer > 0)
            {
                Main.EntitySpriteDraw(
                    Djed.Value,
                    Player.Center - Main.screenPosition + new Vector2(12, Player.gfxOffY),
                    null,
                    new Color(255, 255, 255) * ((float)DashTimer / DashDuration),
                    Player.bodyRotation,
                    Djed.Value.Size() / 2f,
                    1f,
                    SpriteEffects.None,
                    0);
            }
            if (Player.direction == -1 && DashTimer > 0)
            {
                Main.EntitySpriteDraw(
                    Djed.Value,
                    Player.Center - Main.screenPosition + new Vector2(-12, Player.gfxOffY),
                    null,
                    new Color(255, 255, 255) * ((float)DashTimer / DashDuration),
                    Player.bodyRotation,
                    Djed.Value.Size() / 2f,
                    1f,
                    SpriteEffects.FlipHorizontally,
                    0);
            }
        }
    }
}