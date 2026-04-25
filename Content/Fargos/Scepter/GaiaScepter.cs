using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Projectiles.Fargos;
using System.Collections.Generic;
using Terraria.UI.Chat;
using Terraria.GameContent;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots;
using DestroyerTest.Content.Entities;
using System.Collections.ObjectModel;

namespace DestroyerTest.Content.Fargos.Scepter
{
    [ExtendsFromMod(DTCrossMod.FargosSoulsName)]
    [JITWhenModsEnabled(DTCrossMod.FargosSoulsName)]
	public class GaiaScepter: ScepterItem
	{
		public override int Width => 28;
        public override int Height => 28;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

		public override void SetDefaults()
		{
			base.SetDefaults();

			ShootDMG = 72;
			ShootCrit = 2;
			ThrowCrit = 40;
			KB = 8;
			AdditiveValue = Item.sellPrice(silver: 80);
			Rarity = ModContent.RarityType<IncarnadineRarity>();

			ShootID = ProjectileID.BladeOfGrass;
			ThrowID = ModContent.ProjectileType<GaiaScepterThrown>();

			ShootSound = ConstitutionSounds.StellarVolley;
			ThrowSound = SoundID.Item169;

			base.SetDefaults();
		}

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 2f;
            Item.useTime = 20;
            Item.useAnimation = 60;
        }

        public static List<int> ElementalScepterOptions = new List<int>
        {
            ModContent.ProjectileType<CursedShot>(),
            ModContent.ProjectileType<IchorShot>(),
            ModContent.ProjectileType<FireShot>(),
            ModContent.ProjectileType<GalantineShot>(),
            ModContent.ProjectileType<IceShot>(),
            ModContent.ProjectileType<ElectricShot>(),
            ModContent.ProjectileType<RiftShot>(),
            ModContent.ProjectileType<RiftShot2>(),
            ModContent.ProjectileType<ShadowFireShot>(),
            ModContent.ProjectileType<LightShot2>(),
            ModContent.ProjectileType<NightShot>(),
            ModContent.ProjectileType<TenebrisShot>(),
            ModContent.ProjectileType<VenomShot>()
        };
        
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                type = ElementalScepterOptions[Main.rand.Next(ElementalScepterOptions.Count)];
                SoundEngine.PlaySound(Item.UseSound, position);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.3f), ElementalScepterOptions[Main.rand.Next(ElementalScepterOptions.Count)], damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity, ElementalScepterOptions[Main.rand.Next(ElementalScepterOptions.Count)], damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.3f), ElementalScepterOptions[Main.rand.Next(ElementalScepterOptions.Count)], damage, knockback, player.whoAmI);
                return false;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "GaiaScepterSpecialText", "MASTER OF THE HEAVENS AND THE WORLD"));
        }

        public override bool PreDrawTooltip(
        ReadOnlyCollection<TooltipLine> lines,
        ref int x,
        ref int y)
        {
            float drawY = y;

            for (int i = 0; i < lines.Count; i++)
            {
                TooltipLine line = lines[i];
                Vector2 size = FontAssets.MouseText.Value.MeasureString(line.Text);

                if (line.Mod == Mod.Name && line.Name == "GaiaScepterSpecialText")
                {
                    Color[] cl = new Color[]
                    {
                        Color.Red,
                        new Color(255, 30, 0),
                        new Color(255, 60, 0),
                        new Color(255, 90, 0),
                        Color.OrangeRed,
                        Color.Orange,
                        new Color(255, 150, 0),
                        Color.Gold,
                        new Color(255, 200, 0),
                        Color.Yellow,
                        new Color(220, 220, 0),
                        Color.YellowGreen,
                        new Color(150, 200, 0),
                        Color.GreenYellow,
                        new Color(100, 220, 0),
                        Color.Lime,
                        new Color(0, 240, 100),
                        new Color(0, 240, 150),
                        Color.SpringGreen,
                        new Color(0, 220, 200),
                        Color.Turquoise,
                        new Color(0, 200, 220),
                        Color.Cyan,
                        new Color(0, 180, 255),
                        new Color(0, 150, 255),
                        Color.DeepSkyBlue,
                        new Color(30, 150, 255),
                        Color.RoyalBlue,
                        new Color(100, 150, 255),
                        Color.Blue,
                        new Color(100, 100, 255),
                        new Color(120, 80, 255),
                        new Color(150, 50, 255),
                        Color.BlueViolet,
                        new Color(180, 0, 255),
                        Color.Purple,
                        new Color(200, 0, 255),
                        Color.Magenta,
                        new Color(255, 0, 220),
                        new Color(255, 0, 180),
                        new Color(255, 0, 150),
                        Color.Crimson,
                        new Color(255, 20, 100),
                        new Color(255, 40, 80),
                        new Color(255, 50, 70),
                        new Color(255, 80, 60),
                        new Color(255, 100, 50),
                        new Color(255, 120, 40),
                        new Color(255, 140, 20),
                        Color.Red,
                    };

                    Vector2 pos = new Vector2(x, drawY);
                    DTUtils.SweepColorOverString(line.Text, cl, pos, 16f);
                }
                else
                {
                    ChatManager.DrawColorCodedStringWithShadow(
                        Main.spriteBatch,
                        FontAssets.MouseText.Value,
                        line.Text,
                        new Vector2(x, drawY),
                        line.OverrideColor ?? Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One);
                }

                drawY += size.Y;
            }

            return false;
        }

        public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<ElementalScepter>()
				
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
} 