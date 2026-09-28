
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
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
	[AutoloadEquip(EquipType.Wings)]
	public class LunarInsignia : ModItem
	{

		public override void SetStaticDefaults() {
			// These wings use the same values as the solar wings
			// Fly time: 180 ticks = 3 seconds
			// Fly speed: 9
			// Acceleration multiplier: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(600, 9.2f, 2.75f, true);
            DTUtils.NoUpgradeStack[Type] = true;
        }

		public override void SetDefaults() {
			Item.width = 80;
			Item.height = 56;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.accessory = true;
		}

        public bool Precision = false;
        public int SwitchCooldown = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.empressBrooch = true;
            player.wingTime = player.wingTimeMax;

			if (DTCrossMod.CalamityIsLoaded)
			{
				DTCrossMod.CalamityMod.Call("ToggleInfiniteFlight", true);
			}

            if (Precision)
            {
                player.moveSpeed *= 0.6f;
            }

            if (SwitchCooldown <= 0 && DestroyerTestMod.LunarInsigniaModeSwitchKeybind.JustPressed)
            {
                if (!Precision)
                {
                    Precision = true;
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, player.Center);
                    SwitchCooldown = 10;
                }
                else
                {
                    Precision = false;
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { Pitch = -0.7f}, player.Center);
                    SwitchCooldown = 10;
                }
            }
            else
            {
                SwitchCooldown--;
            }
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) 
        {
			ascentWhenFalling = Precision ? 1f : 1f;
			ascentWhenRising = Precision ? 0.1f : 1f;
			maxCanAscendMultiplier = Precision ? 0.4f : 1.3f;
			maxAscentMultiplier = Precision ? 0.6f : 1.3f;
			constantAscend = Precision ? 0.03f : 0.09f;
		}

		public int FrameWidth = 224;
		public int FrameHeight = 160;

        public bool WingRetractFlag = false;
        public bool WingOpenFlag = false;
        public bool HasFlown = false;
        public override bool WingUpdate(Player player, bool inUse)
        {
            //Using Wings of Rebirth as a guide.
           

            int frameRate = 5;

            int numFlyingFrames = 9;
            int numSuckFrames = 5;

            //turns out aside from this check's specificity, the actual frame code isnt bad. the drawcode on the other hand....
            if (player.controlJump && player.wingTime > 0 && player.velocity.Y != 0)
			{
                HasFlown = true;
                WingRetractFlag = false;
                if (!WingOpenFlag)
                {
                    if (!Precision)
                    {
                        SoundEngine.PlaySound(SoundID.Item154, player.Center);
                    }
                    WingOpenFlag = true;
                }

                player.wingFrameCounter++;

                if (player.wingFrameCounter % frameRate == 0)
                {
                    player.wingFrame++;
                }

                if (player.wingFrame == 3)
                {
                    SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -1.6f, PitchVariance = 0.4f, MaxInstances = 1 }, player.Center);
                }

                if (player.wingFrame >= numFlyingFrames)
                {
                    player.wingFrame = 1;
                }
            }
			else
			{
                WingOpenFlag = false;
                if (HasFlown)
                {
                    if (!WingRetractFlag)
                    {
                        if (!Precision)
                        {
                            SoundEngine.PlaySound(SoundID.Item154, player.Center);
                        }
                        WingRetractFlag = true;
                    }

                    if (player.wingFrame < 9)
                    {
                        player.wingFrame = 9;
                    }
                    else
                    {
                        if (player.wingFrame >= numFlyingFrames + numSuckFrames)
                        {
                            player.wingFrameCounter = 0;
                        }
                        else
                        {
                            player.wingFrameCounter++;

                            if (player.wingFrameCounter % frameRate == 0)
                            {
                                player.wingFrame++;
                            }

                        }
                    }
                }
                else
                {
                    player.wingFrame = numFlyingFrames + numSuckFrames;
                    player.wingFrameCounter = 0;
                }
            }


            return true;
        }

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.EmpressFlightBooster)
				.AddIngredient(ItemID.LunarBar, 6)
				.AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
				.Register();
		}
	}

    public class LunarInsigniaWingDrawLayer : PlayerDrawLayer
    {
        public static Asset<Texture2D> Texture;

        public override void Load()
        {
            Texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Equips/LunarInsignia_Wings_Real");
        }

        public override Position GetDefaultPosition()
        {
			return new AfterParent(PlayerDrawLayers.Wings);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.wings == EquipLoader.GetEquipSlot(Mod, "LunarInsignia", EquipType.Wings);

        //I gotta be completely honest. I gave up with the drawcode because wtf is HeightOffsetVisual and why is it hardcoded?
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Player.dead)
                return;

            Texture2D texture = Texture.Value;
            Vector2 Position = drawInfo.Position;
            Vector2 pos = new Vector2((int)(Position.X + (Player.width / 2) - (2 * Player.direction)), (int)(Position.Y + (Player.height / 2 + Player.HeightOffsetVisual / 2f) + 45f * Player.gravDir));
            Color lightColor = Lighting.GetColor((int)Player.Center.X / 16, (int)Player.Center.Y / 16, Color.White);
            Color color = lightColor * (1 - drawInfo.shadow);
            DrawData d = new DrawData(texture, pos - Main.screenPosition, texture.Frame(1, 15, 0, drawInfo.drawPlayer.wingFrame), color, 0f, new Vector2(texture.Width / 2, texture.Height / 18), 1f, drawInfo.playerEffect, 0);
            d.shader = drawInfo.drawPlayer.cWings;
            drawInfo.DrawDataCache.Add(d);
        }
    }
}