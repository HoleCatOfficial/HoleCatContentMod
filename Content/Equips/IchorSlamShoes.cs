using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace DestroyerTest.Content.Equips
{
    public class IchorSlamShoes : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;

            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 1);
        }

  

        

        int SlamCooldown = 0;
        bool Sound = true;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            if (SlamCooldown > 0)
            {
                SlamCooldown--;
            }
            if (SlamCooldown == 1)
            {
                SoundEngine.PlaySound(SoundID.Item20, player.Center);
                Sound = true;
            }

            Rectangle checkBox = new Rectangle(
                (int)(player.BottomLeft.X),
                (int)(player.BottomLeft.Y + 2),
                player.width,
                2
            );

            bool grounded = Collision.SolidCollision(
                checkBox.TopLeft(),
                checkBox.Width,
                checkBox.Height
            );

            bool ShiftKey = (Main.keyState.IsKeyDown(Keys.LeftShift) && Main.oldKeyState.IsKeyDown(Keys.LeftShift)) || (Main.keyState.IsKeyDown(Keys.RightShift) && Main.oldKeyState.IsKeyDown(Keys.RightShift));

            if (player.controlDownHold && ShiftKey)
            {
                if (Sound)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChimeIn"));
                    Sound = false;
                }
                if (grounded && player.velocity.Y > 1f && SlamCooldown <= 0)
                {
                    SlamCooldown = 300;
                    player.controlDownHold = false;

                    player.velocity = Vector2.Zero;

                    Projectile.NewProjectile(
                        player.GetSource_Accessory(Item),
                        player.Bottom,
                        Vector2.Zero,
                        ModContent.ProjectileType<IchorSlam>(),
                        100,
                        7,
                        player.whoAmI
                    );
                }
                else
                {
                    if (player.velocity.Y.NonZeroSign() == 1)
                    {
                        player.velocity.Y *= 1.02f;
                        player.maxFallSpeed = 30;
                    }
                }
            }




        }

        public override void UpdateVanity(Player player)
        {
            // This code is a copy of the visual effects code in UpdateAccessory above
            player.CancelAllBootRunVisualEffects();
            //player.vanityRocketBoots = 2;
            //player.hellfireTreads = true;
            if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat)
            {
                //Utils.TileActionAttempt WalkFX = new(SpawnRiftParticles);
                //player.DoBootsEffect(WalkFX);
            }
        }
    }
}