using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
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
    // This example attempts to showcase most of the common boot accessory effects.
    // Of particular note is a showcase of the correct approaches to various movement speed modifications.
    [AutoloadEquip(EquipType.Shoes)]
    public class HeatseekerSilohs : ModItem
    {
        public override void Load()
        {
            IL_Player.RocketBootVisuals += il =>
            {
                try
                {
                    var c = new ILCursor(il);

                    ILLabel continueLabel = il.DefineLabel();

                    // Load Player self ("this")
                    c.Emit(OpCodes.Ldarg_0);

                    // Call delegate
                    c.EmitDelegate<Func<Player, bool>>(BootEffects);

                    // If false, continue vanilla
                    c.Emit(OpCodes.Brfalse_S, continueLabel);

                    // Otherwise return early
                    c.Emit(OpCodes.Ret);

                    c.MarkLabel(continueLabel);
                }
                catch (Exception e)
                {
                    // If there are any failures with the IL editing, this method will dump the IL to Logs/ILDumps/{Mod Name}/{Method Name}.txt
                    MonoModHooks.DumpIL(ModContent.GetInstance<DestroyerTestMod>(), il);

                    // If the mod cannot run without the IL hook, throw an exception instead. The exception will call DumpIL internally
                    // throw new ILPatchFailureException(ModContent.GetInstance<ExampleMod>(), il, e);
                }

            };
        }

        Player P;

        private static bool BootEffects(Player self)
        {
            if (self.vanityRocketBoots != 6)
                return false;

            if (self.miscCounter % 2 == 0 &&
                self.velocity.Y == 0f &&
                self.grappling[0] == -1 &&
                self.velocity.X != 0f)
            {
                int x = (int)self.Center.X / 16;
                int y = (int)(self.position.Y + self.height - 1f) / 16;

                SpawnRiftParticles(self, x, y);
            }

            return true;
        }

        public static readonly int MoveSpeedBonus = 8;
        public static readonly int LavaImmunityTime = 12;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus, LavaImmunityTime);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;

            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 1); // Equivalent to Item.buyPrice(0, 1, 0, 0);
        }

        static bool SpawnRiftParticles(Player player, int X, int Y)
        {
            Point P = new Point(X, Y);
            Tile tile = Main.tile[P];
            if (tile == null || !tile.HasTile || tile.CheckingLiquid || !WorldGen.SolidTileAllowBottomSlope(X, Y + 1))
                return false;


            Dust D = Dust.NewDustPerfect(P.ToWorldCoordinates(), DustID.FireworksRGB, player.velocity + new Vector2(0, -3f), Main.rand.Next(40, 240), ColorLib.Rift, 1.2f);
            //D.noGravity = true;

            return true;
        }

        void RocketFX(Player player)
        {
            int H = player.height;
            if (player.gravDir == -1f)
                H = 4;

            for (int i = 0; i < 2; i++)
            {
                int Dir = ((i == 0) ? 2 : (-2));
                Rectangle r = ((i != 0) ? new Rectangle((int)player.position.X + player.width - 4, (int)player.position.Y + H - 10, 8, 8) : new Rectangle((int)player.position.X - 4, (int)player.position.Y + H - 10, 8, 8));
                if (player.direction == -1)
                    r.X -= 4;


                int type = ModContent.DustType<RiftDust>();
                float scale = 1.5f;
                int alpha = 100;
                float num3 = 1f;
                Vector2 vector = new Vector2((float)(-Dir) - player.velocity.X * 0.3f, 2f * player.gravDir - player.velocity.Y * 0.3f);
                Dust dust;

                dust = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, type, 0f, 0f, alpha, ColorLib.Rift, scale);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
                dust.velocity += vector;
                dust.velocity *= num3;
            }
        }

        int SlamCooldown = 0;
        bool Sound = true;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            P = player;

            if (SlamCooldown > 0)
            {
                SlamCooldown--;
            }
            if (SlamCooldown == 1)
            {
                SoundEngine.PlaySound(SoundID.Item20, player.Center);
                Sound = true;
            }


            // player.maxRunSpeed and player.runAcceleration are usually not set by boots and should not be changed in UpdateAccessory due to the logic order. See ExampleStatBonusAccessoryPlayer.PostUpdateRunSpeeds for an example of adjusting those speed stats.
            // These 2 stat changes are equal to the Lightning Boots
            player.moveSpeed += 0.12f; // Modifies the player movement speed bonus.
            player.accRunSpeed = 12.2f; // Sets the players sprint speed in boots.

            /*
            if (player.rocketBoots == 0)
            {
                player.rocketTime = 0;
            }
            
            if (player.rocketDelay > 0)
            {
                player.rocketFrame = true;
                RocketFX(player);

                if (player.rocketDelay == 0)
                    player.releaseJump = true;

                player.rocketDelay--;
                player.velocity.Y -= 0.1f * player.gravDir;
                if (player.gravDir == 1f)
                {
                    if (player.velocity.Y > 0f)
                        player.velocity.Y -= 0.5f;
                    else if ((double)player.velocity.Y > (double)(0f - Player.jumpSpeed) * 0.5)
                        player.velocity.Y -= 0.1f;

                    if (player.velocity.Y < (0f - Player.jumpSpeed) * 1.5f)
                        player.velocity.Y = (0f - Player.jumpSpeed) * 1.5f;
                }
                else
                {
                    if (player.velocity.Y < 0f)
                        player.velocity.Y += 0.5f;
                    else if ((double)player.velocity.Y < (double)Player.jumpSpeed * 0.5)
                        player.velocity.Y += 0.1f;

                    if (player.velocity.Y > Player.jumpSpeed * 1.5f)
                        player.velocity.Y = Player.jumpSpeed * 1.5f;
                }
            }
            */

            // Determines whether the boots count as rocket boots
            // 0 - These are not rocket boots
            // Anything else - These are rocket boots
            player.rocketBoots = 5;

            // Sets which dust and sound to use for the rocket flight
            // 1 - Rocket Boots
            // 2 - Fairy Boots, Spectre Boots, Lightning Boots
            // 3 - Frostspark Boots
            // 4 - Terrraspark Boots
            // 5 - Hellfire Treads
            player.vanityRocketBoots = 5;

            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.waterWalk = true; // Allows walking on water, honey, and shimmer without falling into it
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.desertBoots = true; // Grants the player increased movement speed while running on sand
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            player.lavaRose = true; // Grants the Lava Rose effect
            player.lavaMax += LavaImmunityTime * 60; // Grants the player 2 additional seconds of lava immunity

            // player.DoBootsEffect(player.DoBootsEffect_PlaceFlowersOnTile); // Spawns flowers when walking on normal or Hallowed grass

            // These effects are visual only. These are replicated in UpdateVanity below so they apply for vanity equipment.
            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects(); // This ensures that boot visual effects don't overlap if multiple are equipped

                // Hellfire Treads sprint dust. For more info on sprint dusts see Player.SpawnFastRunParticles() method in Player.cs
                //player.hellfireTreads = true;
                // Other boot run visual effects include: sailDash, coldDash, desertDash, fairyBoots

                if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat)
                {
                    // Spawns flames when walking, like Flame Waker Boots. We also check the Goat Skull mount so the effects don't overlap.


                    //Utils.TileActionAttempt WalkFX = new(SpawnRiftParticles);
                    //player.DoBootsEffect(WalkFX);
                }
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

            if (player.controlDownHold && !player.mount.Active)
            {
                if (Sound)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HeatseekerSilohWoosh"));
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
                        ModContent.ProjectileType<HeatseekerSilohExplosion>(),
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient(ItemID.ExplosivePowder, 3)
                .AddIngredient<Living_Shadow>(12)
                .AddIngredient<SunscorchedCinder>(6)
                .AddIngredient<CarbonizedFlesh>(6)
                .Register();
        }
    }

    public class HeatseekerSilohPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Active)
            {
                
            }
        }
    }
}