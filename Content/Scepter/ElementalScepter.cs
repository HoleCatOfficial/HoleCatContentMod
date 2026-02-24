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
using System.Collections.Generic;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots;
using DestroyerTest.Content.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace DestroyerTest.Content.Scepter
{
	public class ElementalScepter : ScepterItem
	{
		public override int Width => 48;
        public override int Height => 48;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 70;
            ShootCrit = 30;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<LightShot>();
            ThrowID = ModContent.ProjectileType<ElementalScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item60;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
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
            ModContent.ProjectileType<NightShot>(),
            ModContent.ProjectileType<LightShot2>(),
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ElementalScepterSpecialText", "MASTER OF THE ELEMENTS"));
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

                if (line.Mod == Mod.Name && line.Name == "ElementalScepterSpecialText")
                {
                    Color[] cl = new Color[]
                    {
                        new Color(5, 62, 80),
                        new Color(24, 67, 97),
                        new Color(26, 73, 107),
                        new Color(15, 84, 125),
                        new Color(25, 102, 148),
                        new Color(28, 138, 204),
                        new Color(0, 162, 232),
                        new Color(0, 168, 218),
                        new Color(0, 190, 164),
                        new Color(34, 177, 76),
                        new Color(22, 158, 69),
                        new Color(24, 153, 135),
                        new Color(20, 120, 118),
                        new Color(14, 93, 82),
                        new Color(12, 83, 67),
                        new Color(6, 79, 57),
                        new Color(6, 79, 76),
                        new Color(6, 69, 79)
                    };

                    Vector2 pos = new Vector2(x, drawY);
                    DTUtils.SweepColorOverString(line.Text, cl, pos, 16f);
                }
                else
                {
                    // Let vanilla draw everything else
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

            return false; // we handled ALL drawing
        }

        

        public static void RegisterScepterLine(Mod mod, string text, List<TooltipLine> tooltips)
        {
            // Find insertion point: just before the summary line
            int insertIndex = tooltips.FindIndex(t =>
                t.Mod != "Terraria" &&
                t.Text.Contains("All shots will inflict their respective debuff.")
            );

            if (insertIndex == -1)
                insertIndex = tooltips.Count;

            tooltips.Insert(insertIndex,
                new TooltipLine(mod, "ElementalScepterRegisteredText", text)
            );
        }



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EmberCane>()
                .AddIngredient<FrostScepter>()
                .AddIngredient<NatureScepter>()
                .AddIngredient<ThunderScepter>()
                .AddIngredient<ShadowScepter>()
                .AddIngredient<StellarFoxScepter>()
                .AddIngredient<BalanceScepter>()
                .AddIngredient<InfectedScepter>()
                .AddIngredient<HeliciteScepter>()
                .AddIngredient<Vesper>(16)
                .AddIngredient(ItemID.GoldBar, 18)
                .AddIngredient<LifeEcho>(100)
                .AddIngredient(ItemID.SoulofFright, 12)
                .AddIngredient(ItemID.SoulofSight, 12)
                .AddIngredient(ItemID.SoulofMight, 12)
                .AddIngredient(ItemID.SoulofLight, 12)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
} 